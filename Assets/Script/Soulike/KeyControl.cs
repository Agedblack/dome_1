using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyControl : MonoBehaviour
{
    /// <summary>
    /// ==== key settings ====
    /// </summary>
    public string keyA;
    public string keyB;
    public string keyC;
    public string keyD;

    public PlayerMove pm;
 
    private void Start()
    {
        pm = GetComponent<PlayerMove>();
       
        
    }
    private void Update()
    {
        KeyWalk();
        KeyJump();
        KeyRoll();
        KeyAttack();
    }

    private void FixedUpdate()
    {
        
        pm.MoveSpeed();
    }
    void KeyWalk()
    {
        pm.run = Input.GetKey(keyA);
        pm.PlayerWalk();
        pm.anim.SetFloat("forward",pm.Dmag* Mathf.Lerp(pm.anim.GetFloat("forward"), ((pm.run) ? 2.0f : 1.5f),0.5f));//forward值 Lerp run值
        if (pm.Dmag > 0.1f)
        {
            pm.player.transform.forward = Vector3.Slerp(pm.player.transform.forward, pm.Dvec, 0.25f);//使转向有渐变效果
        }

        if (pm.lockPlanar == false)
        {
            pm.movingVec = pm.Dmag * pm.player.transform.forward * pm.walkSpeed * ((pm.run) ? 4.0f : 2.5f);
        }
        
    }

    void KeyJump()
    {
        bool newJump = Input.GetKey(keyB);
        if (newJump != pm.lastJump&&newJump==true)
        {
            pm.jump = true;
        }
        else
        {
            pm.jump = false;
        }
        pm.lastJump = newJump;
        if (pm.jump)
        {
            pm.anim.SetTrigger("jump");
            pm.canAttack = false;
        }

    }

    void KeyRoll()
    {
        if (pm.rigid.velocity.magnitude > 0f)
        {
            pm.anim.SetTrigger("roll");
        }
    }
    void KeyAttack()
    {
        bool newAttack = Input.GetKey(keyC);
        if (newAttack != pm.lastJump && newAttack == true)
        {
            pm.attack = true;
        }
        else
        {
            pm.attack = false;
        }
        pm.lastAttack = newAttack;
        if (pm.attack&&CheckState("Ground")&&pm.canAttack)
        {
            pm.anim.SetTrigger("attack");
        }
    }
    private bool CheckState(string stateName, string layerName = "Base Layer")
    {
        return pm.anim.GetCurrentAnimatorStateInfo(pm.anim.GetLayerIndex(layerName)).IsName(stateName);
    }
}
