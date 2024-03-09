
using UnityEngine;


public class Grunt : Enemy
{
    public enum State { Idle, Walk, Trace, Return, Attack, Died }

    [SerializeField] Animator animator;
    [SerializeField] float TraceSpeed;
    [SerializeField] float findRange;
    [SerializeField] float WalkSpeed;
    [SerializeField] Transform WalkPosStart;
    [SerializeField] Transform WalkPosPosEnd;
    [SerializeField] SpriteRenderer render;
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] float idleDuration = 3f; // 타겟을 놓친 후 대기할 시간


    private StateMachine stateMachine;
    private Transform target;
    private Vector2 startPos;
   // private Vector2 moveDir;
    private static bool turn;





    private void Awake()
    {
        stateMachine = gameObject.AddComponent<StateMachine>();
        stateMachine.AddState(State.Idle, new IdleState(this));
        stateMachine.AddState(State.Trace, new TraceState(this));
        stateMachine.AddState(State.Walk, new WalkState(this));
        stateMachine.AddState(State.Return, new ReturnState(this));
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
        //Vector2 moveDirection = rigid.velocity.normalized;

        //if (moveDirection.x < 0)
        //{
        //    render.flipX = true;
        //}
        //else if (moveDirection.x > 0)
        //{
        //    render.flipX = false;
        //}
    }

    private class GruntState : BaseState
    {
        protected Grunt owner;
        protected Transform transform => owner.transform;

        protected float WalkSpeed => owner.WalkSpeed;
        protected float TraceSpeed => owner.TraceSpeed;
        protected float findRange => owner.findRange;
        protected Transform target => owner.target;
        protected Vector2 startPos => owner.startPos;
        protected Animator animator => owner.animator;
        protected Transform WalkPosStart => owner.WalkPosStart;
        protected Transform WalkPosPosEnd => owner.WalkPosPosEnd;
        protected SpriteRenderer render => owner.render;
        protected Rigidbody2D rigid => owner.rigid;

        //public  Vector2 moveDir => owner.moveDir;



        public GruntState(Grunt owner)
        {
            this.owner = owner;
        }
    }



    private class IdleState : GruntState
    {
        public IdleState(Grunt owner) : base(owner) { }


        public override void Transition()
        {
            //3초 정도 기다리기 구현 필요

            if (Vector2.Distance(target.position, transform.position) < findRange)
            {
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
            if (Vector2.Distance(target.position, transform.position) < findRange)
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
            if (Vector2.Distance(target.position, transform.position) > findRange)
            {
                //animator.SetBool("IsReturn", false);
                animator.SetBool("IsFollow", false);
                ChangeState(State.Idle);
            }
        }


    }

    private class ReturnState : GruntState
    {
        public ReturnState(Grunt owner) : base(owner) { }

        //public override void Update()
        //{
        //    Vector2 dir = ((Vector3)startPos - transform.position).normalized;
        //    transform.Translate(dir * WalkSpeed * Time.deltaTime, Space.World);
        //}

        //public override void Transition()
        //{
        //    if (Vector2.Distance(startPos, transform.position) < 1)
        //    {
        //        ChangeState(State.Walk);
        //    }

        //    if (Vector2.Distance(target.position, transform.position) < findRange)
        //    {
        //        animator.SetBool("IsFollow", true);
        //        ChangeState(State.Trace);

        //    }
        //}
    }

    private class DiedState : GruntState
    {
        public DiedState(Grunt owner) : base(owner) { }
    }


    private class AttackState : GruntState
    {
        public AttackState(Grunt owner) : base(owner) { }
    }
}