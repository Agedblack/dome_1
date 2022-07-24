using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public string KeyUp = "w";
    public string KeyDown = "s";
    public string KeyLeft = "a";
    public string KeyRight = "d";
    public float Dup;
    public float Dright;
    public float Dmag;
    public float walkSpeed=2.0f;
    private float lerpTarget;
    public bool inputEnabled = true;
    public bool run;
    public bool jump;
    public bool lastJump;
    public bool lockPlanar=false;
    public bool attack;
    public bool lastAttack;
    public bool canAttack;

    public Vector3 Dvec;
    public Vector3 movingVec;
    public Vector3 thrustVec;

    private float targetDup;
    private float targetDright;
    private float velocityDup;
    private float velocityDright;

    public Rigidbody rigid;
    public GameObject player;
    [SerializeField]
    public Animator anim;
   
    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        anim = player.GetComponent<Animator>();
    }

    private Vector2 SquareToCircle(Vector2 input)
    {

        Vector2 output = Vector2.zero;
        output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
        output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);
        return output;
    }

    /// <summary>
    /// 移动
    /// 转向
    /// </summary>
    public void PlayerWalk()
    {
        targetDup = (Input.GetKey(KeyUp) ? 1.0f : 0) - (Input.GetKey(KeyDown) ? 1.0f : 0);
        targetDright = (Input.GetKey(KeyRight) ? 1.0f : 0) - (Input.GetKey(KeyLeft) ? 1.0f : 0);
        if (inputEnabled == false)
        {
            targetDup = 0;
            targetDright = 0;
        }
        Dup = Mathf.SmoothDamp(Dup, targetDup, ref velocityDup, 0.1f);//类似弹簧-阻尼的函数进行平滑处理
        Dright = Mathf.SmoothDamp(Dright, targetDright, ref velocityDright, 0.1f);

        Vector2 tempDAxis = SquareToCircle(new Vector2(Dright, Dup));
        float Dright2 = tempDAxis.x;
        float Dup2 = tempDAxis.y;

        Dmag = Mathf.Sqrt((Dup2 * Dup2) + (Dright2 * Dright2));
        Dvec = Dright2 * transform.right + Dup2 * transform.forward; 
    }

    public void MoveSpeed()
    {
        rigid.velocity = new Vector3(movingVec.x, rigid.velocity.y, movingVec.z)+thrustVec;
        thrustVec = Vector3.zero;
    }
    public void OnJumpEnter()
    {
        inputEnabled = false;
        lockPlanar = true;
        thrustVec = new Vector3(0, 3.5f, 0);
    }

    public void OnGroundEnter()
    {
        inputEnabled = true;
        lockPlanar = false;
        canAttack = true;
    }

    public void OnFallEnter()
    {
        inputEnabled = false;
        lockPlanar = true;
    }

    public void OnRollEnter()
    {
        inputEnabled = false;
        lockPlanar = true;
        thrustVec = new Vector3(0, 2.0f, 0);
    }
    public void IsGround()
    {
        anim.SetBool("isGround", true);
    }
    public void IsNotGround()
    {
        anim.SetBool("isGround", false);
    }
    public void OnJabEnter()
    {
        inputEnabled = false;
        lockPlanar = true;
    }
    public void OnJabUpdate()
    {
        thrustVec = player.transform.forward * anim.GetFloat("jabVelocity");
    }
    public void OnAttack1hAEnter()
    {
        inputEnabled = false;
        lerpTarget = 1.0f;

    }
    public void OnAttack1hAUpdate()
    {
        thrustVec = player.transform.forward * anim.GetFloat("attack1hAvelocity");
        anim.SetLayerWeight(anim.GetLayerIndex("attack"), Mathf.Lerp(anim.GetLayerWeight(anim.GetLayerIndex("attack")), lerpTarget, 0.4f));
    }
    public void OnAttackIdleEnter()
    {
        inputEnabled = true;
        lerpTarget = 0f;
    }
    public void OnAttackIdleUpdate()
    {
        anim.SetLayerWeight(anim.GetLayerIndex("attack"), Mathf.Lerp(anim.GetLayerWeight(anim.GetLayerIndex("attack")), lerpTarget, 0.4f));
    }
}
