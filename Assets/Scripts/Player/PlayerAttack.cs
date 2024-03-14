using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerAttack : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] Animator WeaponAnimator;
    [SerializeField] Animator PlayerAnimator;
    [SerializeField] SpriteRenderer PlayerRenderer;
    [SerializeField] Transform WeaponTransform;
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] ObjectPool pooler;
    [SerializeField] Transform firePoint;


    [Header("Property")]
    [SerializeField] float AttackPower;
    [SerializeField] float XmaxSpeed;
    [SerializeField] float YmaxSpeed;
    [SerializeField] float ClickCoolTime;

    [Header("Vector")]
    [SerializeField] float range;
    [SerializeField] LayerMask layerMask;

    private Vector2 dirVec;
    public Vector2 DirVec { get { return dirVec; } }


    private bool ableClick = true;

    private void Awake()
    {
        pooler = PoolManager.Instance.GetObjectPool();
    }

    //private void Awake()
    //{

    //    pooler = PoolManager.Instance.GetObjectPool(); // pooler를 PoolManager.Instance로 초기화

    //    PoolManager의 CreatePool 메서드를 호출하여 ObjectPool을 생성 및 초기화
    //    PoolManager.Instance.CreatePool(prefab, size, capacity);
    //}

    //private void Awake()
    //{

    //// PoolManager의 CreatePool 메서드를 호출하여 ObjectPool을 생성 및 초기화합니다.
    //PoolManager.Instance.CreatePool(pooler.prefab, pooler.size, pooler.capacity);

    //// pooler를 PoolManager.Instance로 초기화
    //pooler = PoolManager.Instance.GetObjectPool();
    //}



    private void OnClickAttack(InputValue value)
    {
        ClickAttack();
    }

    private void ClickAttack()
    {

        if (ableClick)
        {
            WeaponAnimator.SetTrigger("IsSlash");


            PlayerAnimator.SetBool("IsAttack", true);


            Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dirVec = mousePos - (Vector2)transform.position; //마우스가 바라보는 방향을 나타내는 벡터


            //마우스를 따라다니는 공격 이펙트
            WeaponTransform.transform.right = (Vector3)dirVec.normalized;
            Vector2 scale = transform.localScale;
            if (dirVec.x < 0)
            {
                scale.y = -1;
            }
            else if (dirVec.x > 0)
            {
                scale.y = 1;
            }
            WeaponTransform.transform.localScale = scale;


            ableClick = false;
            StartCoroutine(StopClick());


            // x최대 속력
            rigid.AddForce(dirVec.normalized * AttackPower, ForceMode2D.Impulse);
            if (rigid.velocity.x < -XmaxSpeed)
            {
                rigid.velocity = new Vector2(-XmaxSpeed, rigid.velocity.y);
            }
            else if (rigid.velocity.x > XmaxSpeed)
            {
                rigid.velocity = new Vector2(XmaxSpeed, rigid.velocity.y);
            }


            // y최대 속력
            rigid.AddForce(dirVec.normalized * AttackPower, ForceMode2D.Impulse);
            if (rigid.velocity.y < -YmaxSpeed)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, -YmaxSpeed);
            }
            else if (rigid.velocity.y > YmaxSpeed)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, YmaxSpeed);
            }


            // 공격 이펙트 이미지 방향 전환
            if (dirVec.x < 0)
            {
                PlayerRenderer.flipX = true;
            }
            else if (dirVec.x > 0)
            {
                PlayerRenderer.flipX = false;
            }

        }


    }
    

    IEnumerator StopClick()
    {
        yield return new WaitForSeconds(ClickCoolTime);
        ableClick = true;



    }

    public void NotIsAttack()
    {
        PlayerAnimator.SetBool("IsAttack", false);
    }


    //오버랩
    Collider2D[] colliders = new Collider2D[20];

    private void AttackTiming()
    {
        int size = Physics2D.OverlapCircleNonAlloc(transform.position, range, colliders, layerMask);
        for (int i = 0; i < size; i++) 
        {
            //시야각
            Vector2 dirToTarget = (colliders[i].transform.position - transform.position).normalized;
            //if (Vector2.Angle(transform.forward, dirToTarget) > 90)
            //    continue;

            //내적 이용
            if (Vector2.Dot(transform.forward, dirToTarget) > 90)
                continue;

            IDamagable damagable = colliders[i].GetComponent<IDamagable>();
            damagable?.Died();

            //Rigidbody2D rigid = colliders[i].GetComponent<Rigidbody2D>();
            //if (rigid != null)
            //{
            //    rigid.AddForce(dirVec * 10, ForceMode2D.Impulse);
            //}


        }
    
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);   

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

}
