using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerAttack : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip gruntAttackSound;

    [Header("Component")]
    [SerializeField] Animator WeaponAnimator;
    [SerializeField] Animator PlayerAnimator;
    [SerializeField] SpriteRenderer PlayerRenderer;
    [SerializeField] Transform WeaponTransform;
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] ObjectPool pooler;



    [Header("Property")]
    [SerializeField] float AttackPower;
    [SerializeField] float XmaxSpeed;
    [SerializeField] float YmaxSpeed;
    [SerializeField] float ClickCoolTime;

    //[Header("AttackCameraProperty")]
    //[SerializeField] float shakeTime;
    //[SerializeField] float intensity;

    [Header("Vector")]
    [SerializeField] float range;
    [SerializeField] LayerMask layerMask;

    [SerializeField] Grunt grunt;
    [SerializeField] ShakeCamera shakeCamera;

    private Vector2 dirVec;
    public Vector2 DirVec { get { return dirVec; } }


    private bool ableClick = true;

    private void Awake()
    {
        pooler = PoolManager.Instance.GetObjectPool();

        shakeCamera = FindObjectOfType<ShakeCamera>(); // FindObjectOfType�� �˻��� �����ϹǷ� ��뿡 �����ؾ� �մϴ�.
        if (shakeCamera == null)
        {
            Debug.LogError("ShakeCamera�� ã�� �� �����ϴ�!");
        }
    }

    


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
            dirVec = mousePos - (Vector2)transform.position; //���콺�� �ٶ󺸴� ������ ��Ÿ���� ����


            //���콺�� ����ٴϴ� ���� ����Ʈ
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


            // x�ִ� �ӷ�
            rigid.AddForce(dirVec.normalized * AttackPower, ForceMode2D.Impulse);
            if (rigid.velocity.x < -XmaxSpeed)
            {
                rigid.velocity = new Vector2(-XmaxSpeed, rigid.velocity.y);
            }
            else if (rigid.velocity.x > XmaxSpeed)
            {
                rigid.velocity = new Vector2(XmaxSpeed, rigid.velocity.y);
            }


            // y�ִ� �ӷ�
            rigid.AddForce(dirVec.normalized * AttackPower, ForceMode2D.Impulse);
            if (rigid.velocity.y < -YmaxSpeed)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, -YmaxSpeed);
            }
            else if (rigid.velocity.y > YmaxSpeed)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, YmaxSpeed);
            }


            // ���� ����Ʈ �̹��� ���� ��ȯ
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


    //������
    Collider2D[] colliders = new Collider2D[20];

    private void AttackTiming()
    {
        int size = Physics2D.OverlapCircleNonAlloc(transform.position, range, colliders, layerMask);
        for (int i = 0; i < size; i++) 
        {

            if (!grunt.IsDied)
            {
                shakeCamera.Shake();
            }

            //�þ߰�
            Vector2 dirToTarget = (colliders[i].transform.position - transform.position).normalized;
            //if (Vector2.Angle(transform.forward, dirToTarget) > 90)
            //    continue;

            //���� �̿�
            if (Vector2.Dot(transform.forward, dirToTarget) > 90)
                continue;

            IDamagable damagable = colliders[i].GetComponent<IDamagable>();
            damagable?.Died();

            // �׷�Ʈ�� ���� �� ���� ���
            if (damagable is Grunt)
            {
                if (gruntAttackSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(gruntAttackSound);
                }
            }

        }
    
    }




    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);   
    }

}
