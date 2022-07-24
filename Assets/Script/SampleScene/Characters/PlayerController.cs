using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private CharacterStats characterStats;
    private GameObject attackTarget;//攻击对象
    private float lastAttackTime;//攻击间隔
    private bool isDead;
    private float stopDistance;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        stopDistance = agent.stoppingDistance;//默认停止距离
    }
    private void Start()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;

        GameManager.Instance.RigisterPlayer(characterStats);
    }
    private void Update()
    {
        isDead = characterStats.CurrentHealth == 0;

        if (isDead)
        {
            GameManager.Instance.NotifyObservers();
        }
        SwitchAnimation();

        lastAttackTime -= Time.deltaTime;
    }
    private void SwitchAnimation()
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
        anim.SetBool("Death", isDead);
    }
    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();//停止所有协程
        if (isDead) return;

        agent.stoppingDistance = stopDistance;
        agent.isStopped = false;
        agent.destination = target;
    }
    private void EventAttack(GameObject target)
    {
        if (isDead) return;

        if (target != null)
        {//暴击
            attackTarget = target;
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;
            StartCoroutine(MoveToAttackTarget());
        }
    }
    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;
        agent.stoppingDistance = characterStats.attackData.atttackRange;

        transform.LookAt(attackTarget.transform);//将player转向攻击目标

        //攻击范围 Distance判断两个点的距离
        while (Vector3.Distance(attackTarget.transform.position, transform.position) > characterStats.attackData.atttackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;//空循环做下一帧判断
        }

        agent.isStopped = true;
        //Attack
        if (lastAttackTime < 0)
        {
            anim.SetTrigger("Attack");
            anim.SetBool("Critical", characterStats.isCritical);
            //重置冷却时间
            lastAttackTime = characterStats.attackData.coolDown;
        }
    }

    //Animation Event
    void Hit()
    {
        if (attackTarget.CompareTag("Attackable"))
        {
            if (attackTarget.GetComponent<Rock>()&& attackTarget.GetComponent<Rock>().rockStates == Rock.RockStates.HitNothing)
            {
                attackTarget.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemy;
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
            }
        }
        else
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }
}
