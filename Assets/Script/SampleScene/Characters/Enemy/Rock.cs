using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    public enum RockStates { HitPlayer,HitEnemy,HitNothing}//伤害player 伤害enemy 没伤害
    private Rigidbody rb;
    public RockStates rockStates;

    [Header("Basic Settings")]
    public float force;
    public int damage;
    public GameObject target;
    private Vector3 direction;
    public GameObject breakEffect;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;
        rockStates = RockStates.HitPlayer;
        FlyToTarget();
    }
    private void FixedUpdate()
    {
        //石头速度＜1时，切换到无伤害状态
        if (rb.velocity.sqrMagnitude < 1f)
        {
            rockStates = RockStates.HitNothing;
        }
        
    }
    public void FlyToTarget()
    {
        if (target == null)
            target = FindObjectOfType<PlayerController>().gameObject;
        direction = (target.transform.position - transform.position+Vector3.up).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);
    }
    private void OnCollisionEnter(Collision other)
    {
        switch (rockStates)
        {
            case RockStates.HitPlayer:
                if (other.gameObject.CompareTag("Player"))
                {
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;//player停止移动
                    other.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;//击飞

                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");//眩晕
                    other.gameObject.GetComponent<CharacterStats>().TakeDamage(damage, other.gameObject.GetComponent<CharacterStats>());//产生伤害
                    rockStates = RockStates.HitNothing;
                }
                break;
            case RockStates.HitEnemy:
                if (other.gameObject.GetComponent<Golem>())
                {
                    var otherStats = other.gameObject.GetComponent<CharacterStats>();
                    otherStats.TakeDamage(damage, otherStats);
                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
        }
    }
}
