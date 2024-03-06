using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;

    public Transform groundChkFront; //�ٴ� üũ Position
    public Transform groundChkBack;  //�ٴ� üũ Position

    public Transform walkChk;
    public float walkchkDistance;
    public LayerMask w_Layer;

    bool isWall;
    public float slidingSpeed;
    public float wallJumpPower;

    public float runSpeed;
    float isRight = 1;

    float input_x;
    bool isGround;
    public float chkDistance;
    public float jumpPower = 1;
    public LayerMask g_Layer;

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();    
        anim = GetComponent<Animator>();

    }

    private void Update()
    {
        input_x = Input.GetAxis("Horizontal");

        // ĳ������ ���ʰ� ������ �ٴ� üũ�� ����
        bool ground_frount = Physics2D.Raycast(groundChkFront.position, Vector2.down,chkDistance, g_Layer);
        bool ground_back = Physics2D.Raycast(groundChkBack.position, Vector2.down, chkDistance, g_Layer);

        // ���� ���¿��� �� �Ǵ� ���ʿ� �ٴ��� �����Ǹ� �ٴڿ� �پ �̵��ϰ� ����
        if (!isGround && (ground_frount || ground_back))
            rigid.velocity = new Vector2(rigid.velocity.x, 0);

        // �� �Ǵ� ������ �ٴ��� �����Ǹ� ifGround ������ ������!
        if(ground_frount || ground_back)
            isGround = true;
        else
            isGround = false;

        anim.SetBool("isGround", isGround);

        isWall = Physics2D.Raycast(walkChk.position, Vector2.right * isRight, walkchkDistance, w_Layer);
        anim.SetBool("isSliding", isWall);

        // �����̽��ٰ� ������ ���� �ִϸ��̼��� ����
        if(Input.GetAxis("Jump")!=0)
        {
            anim.SetTrigger("jump");
        }

        // ����Ű�� ������ ����� ĳ���Ͱ� �ٶ󺸴� ������ �ٸ��ٸ� ĳ������ ������ ��ȯ
        if((input_x > 0 && isRight < 0 ) || (input_x < 0 && isRight > 0 ) )
        {
            FlipPlayer();
            anim.SetBool("run", true);
        }
        else if (input_x == 0)
        {
            anim.SetBool("run", false);
        }


    }

    private void FixedUpdate()
    {
        //ĳ���� �̵�
        rigid.velocity =(new Vector2((input_x) * runSpeed, rigid.velocity.y));

        if(isGround == true)
        {
            // ĳ���� ����
            if(Input.GetAxis("Jump")!=0)
            {
                rigid.velocity = Vector2.up * jumpPower;
            }

        }

        if(isWall)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y * slidingSpeed);
            
            if(Input.GetAxis("Jump")!=0)
            {
                rigid.velocity = new Vector2(-isRight * wallJumpPower, 0.9f * wallJumpPower);
                FlipPlayer() ;
            }
        }
    }

    void FlipPlayer()
    {
        // ������ ��ȯ
        transform.eulerAngles = new Vector3(0, Mathf.Abs(transform.eulerAngles.y - 180), 0);
        isRight = isRight * -1;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(groundChkFront.position, Vector2.down * chkDistance);
        Gizmos.DrawRay(groundChkBack.position, Vector2.down * chkDistance);
    }
}


