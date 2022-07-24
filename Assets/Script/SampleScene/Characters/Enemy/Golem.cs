using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyController
{
    [Header("Skill")]

    public float kickForce = 30;
    public GameObject rockPrefab;
    public Transform handPos;
    //Animation Event
    public void KickOff()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();

            Vector3 direction = attackTarget.transform.position - transform.position;
            targetStats.GetComponent<NavMeshAgent>().isStopped = true;
            targetStats.GetComponent<NavMeshAgent>().velocity = direction * kickForce;//石头人击飞
            targetStats.GetComponent<Animator>().SetTrigger("Dizzy");//使玩家眩晕
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }
    //Animation Event 扔石头
    public void ThrowRock()
    {
        var rock = Instantiate(rockPrefab, handPos.position, Quaternion.identity);
        rock.GetComponent<Rock>().target = attackTarget;
    }
}
