using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Grunt : Enemy
{
    public enum State { Idle, Walk, Trace, Return, Attack, Died }

    [Header("Component")]
    [SerializeField] Animator animator;
    [SerializeField] Transform WalkPosStart;
    [SerializeField] Transform WalkPosPosEnd;
    [SerializeField] SpriteRenderer render;
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] LayerMask layerMask;
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] PlayerAttack playerAttack;
    [SerializeField] HitEffect hitEffect;
    [SerializeField] ShakeCamera shakeCamera;
    [SerializeField] Transform WeaponTransform;
    [SerializeField] Animator WeaponAnimator;
    [SerializeField] PlayerMover playerMover;
    [SerializeField] ObjectPool pooler;

    [Header("Property")]
    [SerializeField] float TraceSpeed;
    [SerializeField] float WalkSpeed;
    [SerializeField] float range;
    [SerializeField] float angle;
    [SerializeField] float hitPower;
    [SerializeField] float xFallDownPower;
    [SerializeField] float yFallDownPower;

    [Header("AttackProperty")]
    [SerializeField] float AttackRange;

    private StateMachine stateMachine;
    private Transform target;
    private Vector2 startPos;

    private bool turn;
    private bool AttackAbleMove = false;
    private float currentAngle;
    private Vector2 gruntDirVec;
    public Vector2 GruntDirVec { get { return gruntDirVec; } }
    private  bool isDied;
    public bool IsDied { get { return isDied; } }



    private void Awake()
    {
        pooler = PoolManager.Instance.GetObjectPool();

        stateMachine = gameObject.AddComponent<StateMachine>();
        stateMachine.AddState(State.Idle, new IdleState(this));
        stateMachine.AddState(State.Trace, new TraceState(this));
        stateMachine.AddState(State.Walk, new WalkState(this));
        stateMachine.AddState(State.Attack, new AttackState(this));
        stateMachine.AddState(State.Died, new DiedState(this));
        stateMachine.InitState(State.Walk);
    }

    private void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
        startPos = transform.position;
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player"); // "Player" 태그로 플레이어를 찾음

    }



    private void AttackStop()
    {
        animator.SetBool("IsAttack", false);
    }

    private bool IsObstacleBlocking()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, target.position - transform.position, range, obstacleMask);
        return hit.collider != null;
    }

    private void AttackEffectTiming()
    {
        WeaponAnimator.SetTrigger("IsSlash");
    }

    private void AttackTiming()
    {
        hitEffect.GrundCreateHitEffect();

        shakeCamera.Shake();



        GameObject playerObject = GameObject.FindGameObjectWithTag("Player"); // "Player" 태그로 플레이어를 찾음

        if (playerObject != null)
        {
            IDamagable damagable = playerObject.GetComponent<IDamagable>(); // 플레이어의 IDamagable 컴포넌트를 가져옴

            damagable?.Died();

        }

    }


    private bool FindTarget()
    {

        Collider2D[] targetsInView = Physics2D.OverlapCircleAll(transform.position, range, layerMask);


        foreach (Collider2D targetInView in targetsInView)
        {
            Vector2 dirToTarget = (targetInView.transform.position - transform.position).normalized;
            //그런트가 플립되었을때 시야각도 플립
            Vector2 referenceDir = render.flipX ? -transform.right : transform.right;

            float angleToTarget = Vector2.Angle(referenceDir, dirToTarget);

            if (angleToTarget <= angle) // 시야각 내에 있는지 확인
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToTarget, range, obstacleMask);
                if (hit.collider == null || hit.collider.gameObject == targetInView.gameObject) // 장애물이 없는지 확인
                {
                    Debug.DrawRay(transform.position, dirToTarget * range, Color.red);
                    return true;
                }
            }

        }




        // 그런트의 시야각을 계산한 후 필드에 저장
        currentAngle = angle;
        if (render.flipX)
        {
            currentAngle *= -1;
        }

        return false;
    }


    private bool AttackTarget()
    {

        Collider2D[] targetsInView = Physics2D.OverlapCircleAll(transform.position, AttackRange, layerMask);


        foreach (Collider2D targetInView in targetsInView)
        {
            Vector2 dirToTarget = (targetInView.transform.position - transform.position).normalized;
            //그런트가 플립되었을때 시야각도 플립
            Vector2 referenceDir = render.flipX ? -transform.right : transform.right;

            float angleToTarget = Vector2.Angle(referenceDir, dirToTarget);

            if (angleToTarget <= angle) // 시야각 내에 있는지 확인
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToTarget, AttackRange, obstacleMask);
                if (hit.collider == null || hit.collider.gameObject == targetInView.gameObject) // 장애물이 없는지 확인
                {
                    Debug.DrawRay(transform.position, dirToTarget * range, Color.red);
                    return true;
                }
            }

        }



        // 그런트의 시야각을 계산한 후 필드에 저장
        currentAngle = angle;
        if (render.flipX)
        {
            currentAngle *= -1;
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        //시야범위
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, range);

        // 시작 각도와 끝 각도를 계산합니다.
        float startAngle = transform.eulerAngles.z - currentAngle; // 시작 각도
        float endAngle = transform.eulerAngles.z + currentAngle; // 끝 각도

        // 호(arc)의 시작 및 끝점을 계산합니다.
        Vector3 startDir = AngleToDir(startAngle);
        Vector3 endDir = AngleToDir(endAngle);

        // 호(arc)를 그립니다.
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + startDir * range);
        Gizmos.DrawLine(transform.position, transform.position + endDir * range);




        //공격범위
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, AttackRange);

        // 시작 각도와 끝 각도를 계산합니다.
        float AttackStartAngle = transform.eulerAngles.z - currentAngle; // 시작 각도
        float AttackEndAngle = transform.eulerAngles.z + currentAngle; // 끝 각도

        // 호(arc)의 시작 및 끝점을 계산합니다.
        Vector3 AttackStartDir = AngleToDir(AttackStartAngle);
        Vector3 AttackEndDir = AngleToDir(AttackEndAngle);

        // 호(arc)를 그립니다.
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + AttackStartDir * AttackRange);
        Gizmos.DrawLine(transform.position, transform.position + AttackEndDir * AttackRange);


        //공격범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);

    }

    private Vector3 AngleToDir(float angle)
    {


        // 그런트가 왼쪽을 보면서 왼쪽으로 시야각을 조정합니다.
        if (render.flipX)
        {
            angle += 180f;
        }

        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0); // 2D 환경에서는 z 값은 0으로 설정합니다.

    }








    public override void Died()
    {
        stateMachine.ChangeState(State.Died);
    }

    private class GruntState : BaseState
    {
        protected Grunt owner;
        protected Transform transform => owner.transform;

        protected float WalkSpeed => owner.WalkSpeed;
        protected float TraceSpeed => owner.TraceSpeed;
        protected float TraceRange => owner.range;
        protected Transform target => owner.target;
        protected Vector2 startPos => owner.startPos;
        protected Animator animator => owner.animator;
        protected Transform WalkPosStart => owner.WalkPosStart;
        protected Transform WalkPosPosEnd => owner.WalkPosPosEnd;
        protected SpriteRenderer render => owner.render;
        protected Rigidbody2D rigid => owner.rigid;
        protected PlayerAttack playerAttack => owner.playerAttack;
        protected float hitPower => owner.hitPower;
        protected float XmaxHitPower => owner.xFallDownPower;
        protected float YmaxHitPower => owner.yFallDownPower;
        protected float AttackRange => owner.AttackRange;
        protected HitEffect hitEffect => owner.hitEffect;
        protected Transform WeaponTransform => owner.WeaponTransform;
        protected Animator WeaponAnimator => owner.WeaponAnimator;
        protected PlayerMover playerMover => owner.playerMover;


        //protected LayerMask layerMask => owner.layerMask;



        public GruntState(Grunt owner)
        {
            this.owner = owner;
        }
    }



    private class IdleState : GruntState
    {
        public IdleState(Grunt owner) : base(owner) { }



        public override void Enter()
        {
            animator.SetBool("IsIdle", true);
        }

        public override void Update()
        {
            owner.FindTarget();

        }

        public override void Transition()
        {
            if (!owner.IsObstacleBlocking())
            {

                animator.SetBool("IsIdle", false);
                animator.SetBool("IsFollow", true);
                ChangeState(State.Trace);
            }

            else if (owner.AttackTarget())
            {
                animator.SetBool("IsIdle", false);
                ChangeState(State.Attack);

            }
        }
    }





    private class WalkState : GruntState
    {
        public WalkState(Grunt owner) : base(owner) { }

        public override void Update()
        {
            owner.FindTarget();

            //Debug.Log(Vector2.Distance(target.position, transform.position));

            // 적의 이동 방향을 가져옵니다.
            Vector2 dir;
            if (owner.turn)
                dir = (WalkPosStart.position - transform.position).normalized;
            else
                dir = (WalkPosPosEnd.position - transform.position).normalized;

            // 이동 방향에 따라 이미지를 Flip합니다.
            render.flipX = dir.x < 0;

            // 적을 이동시킵니다.
            Vector2 movement = dir * WalkSpeed * Time.deltaTime;
            transform.Translate(movement, Space.World);

            // 적이 도착지점에 도달하면 방향을 변경합니다.
            if ((owner.turn && Vector2.Distance(WalkPosStart.position, transform.position) < 1) ||
                (!owner.turn && Vector2.Distance(WalkPosPosEnd.position, transform.position) < 1))
            {
                owner.turn = !owner.turn;
            }

        }

        public override void Transition()
        {
            if (owner.FindTarget() && Vector2.Distance(target.position, transform.position) < TraceRange)
            {

                animator.SetBool("IsFollow", true);
                ChangeState(State.Trace);

            }
        }
    }


    private class TraceState : GruntState
    {
        public TraceState(Grunt owner) : base(owner) { }


        public override void Update()
        {

            Vector2 dir = (target.position - transform.position).normalized;
            transform.Translate(dir * TraceSpeed * Time.deltaTime, Space.World);
            if (dir.x < 0)
            {
                render.flipX = true;
            }
            else if (dir.x > 0)
            {
                render.flipX = false;
            }



        }


        public override void Transition()
        {
            if (Vector2.Distance(target.position, transform.position) > TraceRange + 1)
            {
                //시야 범위
                animator.SetBool("IsFollow", false);
                ChangeState(State.Idle);

            }
            else if (owner.AttackTarget())
            {
                //공격 범위
                animator.SetBool("IsFollow", false);
                ChangeState(State.Attack);

            }
        }


    }



    private class DiedState : GruntState
    {
        public DiedState(Grunt owner) : base(owner) { }


        public override void Enter()
        {

            Debug.Log("그런트 다이");

            animator.SetBool("Dead", true);



            if (!owner.isDied)
            {
                rigid.AddForce(playerAttack.DirVec * hitPower, ForceMode2D.Impulse);
                hitEffect?.CreateHitEffect();
                owner.isDied = true;
            }

            //공격받아서 날라가는 y 최대 속력

            // x최대 속력
            if (rigid.velocity.x < -XmaxHitPower)
            {
                rigid.velocity = new Vector2(-XmaxHitPower, rigid.velocity.y);
            }
            else if (rigid.velocity.x > XmaxHitPower)
            {
                rigid.velocity = new Vector2(XmaxHitPower, rigid.velocity.y);
            }


            // y최대 속력
            if (rigid.velocity.y < -YmaxHitPower)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, -YmaxHitPower);
            }
            else if (rigid.velocity.y > YmaxHitPower)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, YmaxHitPower);
            }

            animator.SetBool("DiedGround", true);

        }

        public override void Update()
        {
            //Debug.Log("");
            animator.SetFloat("YSpeed", rigid.velocity.y);

            if (rigid.velocity.y > 5f)
            {
                animator.SetBool("DiedGround", false);
            }
        }
    }



    private IEnumerator AttackCoroutine()
    {


        Debug.Log("코루틴");
        Vector2 dirToTarget = (Vector2)(target.transform.position - transform.position).normalized;
        gruntDirVec = dirToTarget - (Vector2)transform.position;

        WeaponTransform.transform.right = (Vector3)gruntDirVec.normalized;

        Vector3 scale = transform.localScale;
        Vector3 position = transform.localPosition;
        Vector3 rotation = transform.localEulerAngles;

        if (render.flipX)
        {
            scale.y = -1;

            position.x = (float)-0.5;
            position.y = 0;
            position.z = 0;

            rotation.x = 0;
            rotation.y = 0;
            rotation.z = -130;



        }
        else
        {
            scale.y = 1;

            position.x = (float)0.5;
            position.y = 0;
            position.z = 0;

            rotation.x = 0;
            rotation.y = 0;
            rotation.z = -30;

        }
        WeaponTransform.transform.localScale = scale;
        WeaponTransform.transform.localPosition = position;
        WeaponTransform.transform.localEulerAngles = rotation;



        animator.SetBool("IsAttack", true);
        AttackAbleMove = false;
        yield return new WaitForSeconds(0.8f);

        //// 코루틴이 끝나면 살아있는 상태인 경우에만 공격 실행
        //if (!isDied)
        //{
        //    stateMachine.ChangeState(State.Attack);
        //}
        //else
        //{
        //    // 죽은 상태이므로 코루틴을 중지
        //    StopCoroutine(AttackCoroutine());
        //}
    }


    private class AttackState : GruntState
    {
        public AttackState(Grunt owner) : base(owner) { }

        //private bool AttackRangeOut; // 범위 안에 있는가
        private bool CoroutineStarted; // Coroutine이 이미 시작되었는지 여부

        //AttackAbleMove //공격할때는 이동못하게 막기


        public override void Enter()
        {


            if (!playerMover.PlayerDied)
            {
                animator.SetBool("IsIdle", false);
                owner.AttackAbleMove = true;

                CoroutineStarted = false; // Coroutine이 시작되지 않았음을 초기화

                // Coroutine 시작 조건 확인
                CheckCoroutineStart();
            }
        }

        public override void Update()
        {

            owner.AttackTarget();

            // Coroutine 시작 조건 확인
            CheckCoroutineStart();
        }

        private void CheckCoroutineStart()

        {
            // Coroutine이 시작되지 않았고, 공격 가능 상태이며, 공격 범위 안에 있을 때
            if (!CoroutineStarted && owner.AttackAbleMove && owner.AttackTarget())
            {
                owner.StartCoroutine(owner.AttackCoroutine());
                CoroutineStarted = true; // Coroutine이 시작되었음을 표시
            }
        }


        public override void Transition()
        {
            if (!owner.AttackTarget() && owner.AttackAbleMove)
            {
                animator.SetBool("IsAttack", false);
                animator.SetBool("IsFollow", true);
                WeaponAnimator.ResetTrigger("IsSlash");
                ChangeState(State.Trace);

            }
        }
    }


}
