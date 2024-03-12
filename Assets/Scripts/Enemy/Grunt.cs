
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

//[SerializeField] float idleDuration = 3f; // 타겟을 놓친 후 대기할 시간

    [Header("Property")]
    [SerializeField] float TraceSpeed;
    [SerializeField] float WalkSpeed;
    [SerializeField] float range;
    [SerializeField] float angle;

    private StateMachine stateMachine;
    private Transform target;
    private Vector2 startPos;

    private static bool turn;
    private bool IsFindTarget;





    private void Awake()
    {
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
    }

    private void Update()
    {

    }

    Collider2D[] colliders = new Collider2D[20];

    private void FindTarget()
    {


        IsFindTarget = false;

        Collider2D[] targetsInView = Physics2D.OverlapCircleAll(transform.position, range, layerMask);

        foreach (Collider2D targetInView in targetsInView)
        {
            Vector2 dirToTarget = (targetInView.transform.position - transform.position).normalized;
            float angleToTarget = Vector2.Angle(transform.right, dirToTarget);


            if (angleToTarget <= angle) // 시야각 내에 있는지 확인
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToTarget, range, obstacleMask);
                if (hit.collider == null) // 장애물이 없는지 확인
                {
                    Debug.DrawRay(transform.position, dirToTarget * range, Color.red);
                    IsFindTarget = true;
                }
            }
        }




    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, range);

        // 시작 각도와 끝 각도를 계산합니다.
        float startAngle = transform.eulerAngles.z - angle; // 시작 각도
        float endAngle = transform.eulerAngles.z + angle; // 끝 각도

        // 호(arc)의 시작 및 끝점을 계산합니다.
        Vector3 startDir = AngleToDir(startAngle);
        Vector3 endDir = AngleToDir(endAngle);

        // 호(arc)를 그립니다.
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + startDir * range);
        Gizmos.DrawLine(transform.position, transform.position + endDir * range);

    }

    private Vector3 AngleToDir(float angle)
    {

        // 그런트가 왼쪽을 보면서 왼쪽으로 시야각을 조정합니다.
        if (render.flipX)
        {
            float radian = (angle + 180f) * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0); // 2D 환경에서는 z 값은 0으로 설정합니다.
        }
        // 그런트가 오른쪽을 보면서 오른쪽으로 시야각을 조정합니다.
        else
        {
            float radian = angle * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0); // 2D 환경에서는 z 값은 0으로 설정합니다.
        }
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
        protected float range => owner.range;
        protected Transform target => owner.target;
        protected Vector2 startPos => owner.startPos;
        protected Animator animator => owner.animator;
        protected Transform WalkPosStart => owner.WalkPosStart;
        protected Transform WalkPosPosEnd => owner.WalkPosPosEnd;
        protected SpriteRenderer render => owner.render;
        protected Rigidbody2D rigid => owner.rigid;
        protected bool IsFindTarget => owner.IsFindTarget;

        //public  Vector2 moveDir => owner.moveDir;



        public GruntState(Grunt owner)
        {
            this.owner = owner;
        }
    }



    private class IdleState : GruntState
    {
        public IdleState(Grunt owner) : base(owner) { }




        public override void Update()
        {
            owner.FindTarget();
        }

        public override void Transition()
        {
            //3초 정도 기다리기 구현 필요


            if (IsFindTarget)
            {
                animator.SetBool("IsFollow", true);
                ChangeState(State.Trace);
            }
            else
            {
                ChangeState(State.Walk);
            }

        }
    }





    private class WalkState : GruntState
    {
        public WalkState(Grunt owner) : base(owner) { }

        public override void Update()
        {
            owner.FindTarget();

            // 적의 이동 방향을 가져옵니다.
            Vector2 dir;
            if (turn)
                dir = (WalkPosStart.position - transform.position).normalized;
            else
                dir = (WalkPosPosEnd.position - transform.position).normalized;

            // 이동 방향에 따라 이미지를 Flip합니다.
            render.flipX = dir.x < 0;

            // 적을 이동시킵니다.
            Vector2 movement = dir * WalkSpeed * Time.deltaTime;
            transform.Translate(movement, Space.World);

            // 적이 도착지점에 도달하면 방향을 변경합니다.
            if ((turn && Vector2.Distance(WalkPosStart.position, transform.position) < 1) ||
                (!turn && Vector2.Distance(WalkPosPosEnd.position, transform.position) < 1))
            {
                turn = !turn;
            }
            
        }

        public override void Transition()
        {
            if (IsFindTarget)
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
            if (Vector2.Distance(target.position, transform.position) > range)
            {
                animator.SetBool("IsFollow", false);
                ChangeState(State.Idle);
            }
        }


    }



    private class DiedState : GruntState
    {
        public DiedState(Grunt owner) : base(owner) { }

        public override void Enter()
        {
            Debug.Log("다이 스테이트");
        }

    }


    private class AttackState : GruntState
    {
        public AttackState(Grunt owner) : base(owner) { }
    }
}