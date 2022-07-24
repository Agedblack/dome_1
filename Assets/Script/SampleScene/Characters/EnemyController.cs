using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { GUARD,PATROL,CHASE,DEAD}//警戒 巡逻 追逐 死亡
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStats))]
public class EnemyController : MonoBehaviour,IEndGameObserver
{
    private EnemyStates enemyStates;
    private NavMeshAgent agent;
    private Animator anim;
    protected CharacterStats characterStats;
    private Collider coll;

    [Header("Basic Settings")]
    public float sightRadius;//可视范围
    public bool isGuard;//是否站桩
    private float speed;
    protected GameObject attackTarget;//攻击目标
    public float loolAtTime;
    private float remainLookAtTime;
    private float lastAttackTime;
    private Quaternion guarRotation;
    [Header("Patrol State")]
    public float patrolRange;//活动范围
    private Vector3 wayPoint;//巡逻随机点
    private Vector3 guardPos;

    //boll转换动画
    bool isWalk;
    bool isChase;//警戒
    bool isFollow;//追
    bool isDead;
    bool playerDead=false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        coll = GetComponent<Collider>();

        speed = agent.speed;
        guardPos = transform.position;
        guarRotation = transform.rotation;
        remainLookAtTime = loolAtTime;
    }
    private void Start()
    {
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates = EnemyStates.PATROL;
            GetNewWayPoint();
        }
        //场景切换时改掉
        GameManager.Instance.AddObserver(this);
    }
    //切换场景时启用
    //void OnEnable()
    //{
    //    GameManager.Instance.AddObserver(this);
    //}
    void OnDisable()
    {
        if (!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveObserver(this);
    }
    private void Update()
    {
        if (characterStats.CurrentHealth == 0)
            isDead = true;

        if (!playerDead)
        {
            SwitchStates();
            SwitchAniimation();
            lastAttackTime -= Time.deltaTime;
        }
    }

    void SwitchAniimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("Death", isDead);
    }
    void SwitchStates()
    {
        if (isDead)
        {
            enemyStates = EnemyStates.DEAD;
        }
        //如果发现player 切换到CHASE（追击）
        else if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
        }
        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                isChase = false;
                if (transform.position != guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;

                    if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guarRotation, 0.01f);
                    }    
                }
                break;
            case EnemyStates.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;

                //判断是否走到随机巡逻点
                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainLookAtTime > 0)
                        remainLookAtTime -= Time.deltaTime;
                    else
                    GetNewWayPoint();//寻找新的点
                }
                else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }
                break;
            case EnemyStates.CHASE:
                //追player
                //配合动画
                isWalk = false;
                isChase = true;
                agent.speed = speed;
                if (!FoundPlayer())
                {
                    //拉脱回上一状态
                    isFollow = false;
                    if (remainLookAtTime > 0)
                    {
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else if (isGuard)
                    {
                        enemyStates = EnemyStates.GUARD;
                    }
                    else
                    {
                        enemyStates=EnemyStates.PATROL;
                    }
                }
                else
                {
                    isFollow = true;
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;
                }
                //在攻击范围内攻击
                if (TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;

                    if (lastAttackTime < 0)
                    {
                        //攻击间隔
                        lastAttackTime = characterStats.attackData.coolDown;

                        //暴击判断
                        characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
                        //执行攻击
                        Attack();
                    }
                }

                break;
            case EnemyStates.DEAD:
                coll.enabled = false;
                //agent.enabled = false;
                agent.radius = 0;
                Destroy(gameObject, 2.0f);
                break;
        }
    }

    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if (TargetInAttackRange())
        {
            //近战攻击
            anim.SetTrigger("Attack");
        }
        if (TargetInSkillRange())
        {
            //技能攻击
            anim.SetTrigger("Skill");
        }
    }
    /// <summary>
    /// 寻找player
    /// </summary>
    /// <returns></returns>
    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);//中心点 半径
        foreach(var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }

    bool TargetInAttackRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.atttackRange;
        else
            return false;
    }
    bool TargetInSkillRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange;
        else
            return false;
    }
    void GetNewWayPoint()
    {
        remainLookAtTime = loolAtTime;
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);
        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);//获得随机点
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
    }
    /// <summary>
    /// 画出范围
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }

    //Animation Event
    void Hit()
    {
        if (attackTarget != null&&transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }

    public void EndNotify()
    {
        //获胜
        //停止所以移动
        //停止Agent
        anim.SetBool("Win",true);
        playerDead = true;
        isChase = false;
        isWalk = false;
        attackTarget = null;

    }
}
