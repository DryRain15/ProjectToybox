using System;
using Proto;
using Proto.Behaviours;
using UnityEngine;
using UnityEngine.Serialization;

namespace Proto.Behaviours.Impl
{
    public class ImplEnemy : AbstractInteractableObject, IDamaged, IAutoBehaviour, IMovable
    {
        private Collider2D _col;
        private Vector3 _collisionPoint;
        protected Animator _anim;
        protected IPooledObject _showRange;

        [FormerlySerializedAs("AttackRange")] public float attackRange = 2f;
        [FormerlySerializedAs("SightRange")] public float sightRange = 3f;
        public float attackTime = 1f;

        public float DamageTimer = 0f;

        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
            HitType = HitType.Enemy;
            _anim = GetComponentInChildren<Animator>();
            _col = GetComponent<Collider2D>();

            EventController.Instance.Subscribe("BattleGroupEvent", ToBattleState);
            EventController.Instance.Subscribe("NoticeGroupEvent", ToNoticeState);
        }

        private void DebugEvent(EventParameter param)
        {
            var spaceKey = param.Get<bool>("SpaceKey");
            if (spaceKey)
            {
                Interact(PlayerBehaviour.Instance);
            }
        }

        private void ToBattleState(EventParameter param)
        {
            var paramBattleGroup = param.Get<string>("BattleGroup");
            if (paramBattleGroup == null) return;
            if (paramBattleGroup == BattleGroup)
                OnStateTransition(AutoState.Wait);
        }

        private void ToNoticeState(EventParameter param)
        {
            var paramNoticeGroup = param.Get<string>("NoticeGroup");
            if (paramNoticeGroup == null) return;
            if (paramNoticeGroup == NoticeGroup)
                OnStateTransition(AutoState.Follow);
        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();
            AutoUpdate();
            if (InteractState != InteractState.OnAction)
                MoveTo();

            DamageTimer += Time.deltaTime;
        }

        private void FixedUpdate()
        {
            SetDirection();
        }

        #region IMovable

        public Vector3 Velocity { get; set; }

        public void MoveTo()
        {
            _anim.ResetTrigger("ActionStateOnChange");
            var prevAnim = AnimState;

            if (Velocity.magnitude < Mathf.Epsilon)
                AnimState = AnimState.Stand;
            else
                AnimState = AnimState.Move;

            _anim.SetInteger("ActionState", (int)AnimState);
            if (prevAnim != AnimState)
                _anim.SetTrigger("ActionStateOnChange");
            if (AnimState == AnimState.Stand |
                AnimState == AnimState.Hold) return;

            _col.enabled = false;
            var hit = Physics2D.Raycast(Position,
                Velocity.normalized,
                Mathf.Max(Velocity.magnitude * Time.deltaTime, 0.3f) * 2f, 1 << 8 | 1 << 9);
            if (hit)
            {
                _collisionPoint = hit.point;
                var normal = hit.normal;
                var x = normal.x > 0 ? 1 : -1;
                var y = normal.y > 0 ? 0.5f : -0.5f;
                var result = new Vector3(-x, y, 0);
                if (Velocity.x == 0 && Velocity.y != 0f) //수직 이동시
                    result *= -1;

                var prevVelocity = Velocity;
                Velocity = result;

                hit = Physics2D.Raycast(Position,
                    Velocity.normalized,
                    Mathf.Max(Velocity.magnitude * Time.deltaTime, 0.3f), 1 << 8);
                if (hit)
                {
                    Velocity = prevVelocity;
                    return;
                }
            }

            _col.enabled = true;
            Position += Velocity * Time.deltaTime;
        }

        private void SetDirection()
        {
            _anim.ResetTrigger("DirectionOnChange");

            var prevHDir = (Direction.Left & Direction) | (Direction.Right & Direction);
            var prevVDir = (Direction.Up & Direction) | (Direction.Down & Direction);

            Direction = Direction.None;

            if (Velocity.x < 0)
            {
                Direction = Direction.Left;
            }
            else if (Velocity.x > 0)
            {
                Direction = Direction.Right;
            }

            if (Velocity.y < 0)
            {
                Direction = Direction | Direction.Down;
            }
            else if (Velocity.y > 0)
            {
                Direction = Direction | Direction.Up;
            }

            if ((prevHDir | prevVDir) != Direction.None && Direction == Direction.None)
            {
                Direction = prevHDir | prevVDir;
                return;
            }

            if (prevHDir != ((Direction.Left & Direction) | (Direction.Right & Direction)) ||
                prevVDir != ((Direction.Up & Direction) | (Direction.Down & Direction)))
                _anim.SetTrigger("DirectionOnChange");


            if (Direction != Direction.None)
            {
                var up = Utils.DirectionContains(Direction, Direction.Up);
                var down = Utils.DirectionContains(Direction, Direction.Down);
                var left = Utils.DirectionContains(Direction, Direction.Left);
                var right = Utils.DirectionContains(Direction, Direction.Right);

                if (up || down)
                {
                    _anim.SetBool("IsUp", up);
                    _anim.SetBool("IsDown", down);
                }
                if (left || right)
                {
                    _anim.SetBool("IsLeft", left);
                    _anim.SetBool("IsRight", right);
                }

                spriteRenderer.flipX = right;
            }
        }

        #endregion

        #region IAutoBehaviour

        [SerializeField] private string battleGroup;
        public string BattleGroup { get => battleGroup; set => battleGroup = value; }
        [SerializeField] private string noticeGroup;
        public string NoticeGroup { get => noticeGroup; set => noticeGroup = value; }

        public AutoState AutoState { get; set; }

        public void OnStateTransition(AutoState to)
        {
            // Dispose Step for previous state
            switch (AutoState)
            {

            }

            AutoState = to;

            // Initiate Step for next state
            switch (to)
            {

            }

        }

        public virtual void AutoUpdate()
        {
            if (InteractState == InteractState.OnAction)
            {
                Velocity = Vector3.zero;
                return;
            }

            switch (AutoState)
            {
                case AutoState.None:
                    break;
                case AutoState.Wait:
                    if (this.TargetInRange(PlayerBehaviour.Instance, sightRange))
                    {
                        AutoState = AutoState.Follow;
                        var param = new EventParameter(
                            "BattleGroup".EventParameterPairing(BattleGroup),
                            "NoticeGroup".EventParameterPairing(NoticeGroup));
                        EventController.Instance.EventCall("BattleGroupEvent", param);
                        EventController.Instance.EventCall("NoticeGroupEvent", param);
                    }
                    break;
                case AutoState.Follow:
                    var v = PlayerBehaviour.Instance.Position - Position;
                    Direction = Utils.ClampVectorToDirection(v);
                    var singleDirectionMult = (Utils.IsHorizontal(Direction) || Utils.IsVertical(Direction))
                        ? Mathf.Sqrt(5) / 4
                        : 0.5f;
                    Velocity = Utils.DirectionToVector(Direction) * (singleDirectionMult * Stats.moveSpeed);
                    if (this.TargetInRange(PlayerBehaviour.Instance, attackRange))
                    {
                        AutoState = AutoState.Action;
                        Interact(PlayerBehaviour.Instance);
                    }
                    break;
                case AutoState.Action:
                    if (InteractState != InteractState.OnAction)
                    {
                        AutoState = AutoState.Follow;
                        if (_showRange != null)
                        {
                            _showRange.Dispose();
                            _showRange = null;
                        }
                    }
                    break;
            }
        }

        #endregion

        protected override void InteractStateUpdate(InteractState state)
        {
            switch (state)
            {
                case InteractState.OnAction:
                    OnActionStateUpdate();
                    break;
            }
        }

        protected virtual void OnActionStateUpdate()
        {
            if (innerTimer > 1f * attackTime)
                InteractState = InteractState.EndInteract;
            if (innerTimer > 0.5f * attackTime)
            {
                if (_showRange != null)
                {
                    _showRange.Dispose();
                    _showRange = null;
                    
                    AnimState = AnimState.Attack;
                    _anim.SetInteger("ActionState", (int)AnimState);
                    _anim.SetTrigger("ActionStateOnChange");

                    var state = new DamageState
                    {
                        Damage = Stats.atk,
                        DamageType = 0,
                        Getter = PlayerBehaviour.Instance,
                        Sender = this,
                        KnockBack = 1f,
                    };
                    if (this.TargetInRangeAngle(PlayerBehaviour.Instance, attackRange, Direction))
                        PlayerBehaviour.Instance.GetHit(state);
                }
            }
            else
            {
                var yVelocity = 1.5f - 1.5f * (innerTimer * 4f / attackTime);
                Transform.GetChild(0).localPosition += new Vector3(0f, 1f, 2f) * (yVelocity * Time.deltaTime);
            }
        }


        #region IInteractableObject

        public override void Interact(ICharacterObject target)
        {
            if (AutoState == AutoState.Action)
            {
                innerTimer = 0f;
                InteractState = InteractState.OnAction;
                if (_showRange != null)
                {
                    _showRange.Dispose();
                    _showRange = null;
                }
                _showRange = ObjectPoolController.Self.Instantiate("RadialFX",
                    new PoolParameters(transform.position));
                var (x, y) = Direction.DirectionToRange();
                if (Direction == Direction.Up)
                    _showRange.gameObject.GetComponent<RadialFX>()
                        .Initialize(x, y, Color.red, attackRange, y);
                else
                    _showRange.gameObject.GetComponent<RadialFX>().Initialize(
                        x, y, Color.red, attackRange);

                _anim.SetTrigger("DirectionOnChange");
                if (Direction != Direction.None)
                {
                    var up = Utils.DirectionContains(Direction, Direction.Up);
                    var down = Utils.DirectionContains(Direction, Direction.Down);
                    var left = Utils.DirectionContains(Direction, Direction.Left);
                    var right = Utils.DirectionContains(Direction, Direction.Right);

                    if (up || down)
                    {
                        _anim.SetBool("IsUp", up);
                        _anim.SetBool("IsDown", down);
                    }
                    if (left || right)
                    {
                        _anim.SetBool("IsLeft", left);
                        _anim.SetBool("IsRight", right);
                    }

                    spriteRenderer.flipX = right;
                }
                
                return;
            }
            if (InteractState == InteractState.Interactable)
            {
                if (_showRange != null)
                {
                    _showRange.Dispose();
                    _showRange = null;
                }
                innerTimer = 0f;
                InteractState = InteractState.OnAction;
                return;
            }

        }

        #endregion

        #region IDamaged

        public HitType HitType { get; set; }
        public virtual void GetHit(DamageState state)
        {
            if (DamageTimer < 0.5f) return;
            DamageTimer = 0f;
            if (InteractState == InteractState.OnAction)
            {
                CurrentHP -= state.Damage;
            
                var bar = ObjectPoolController.Self.Instantiate("UIBarFX", new PoolParameters(Position)) as UIBarFX;
                bar.Initialize(transform, 0, CurrentHP / Stats.hpMax, 0.5f);

                var tfx = ObjectPoolController.Self.Instantiate("DamageTextFX", new PoolParameters(Position)) as DamageTextFX;
                tfx.Initialize(string.Format($"{state.Damage}"), false, 0.5f);

                Utils.DamageRedPulse(spriteRenderer, 0.3f);
            }
            else
            {
                CurrentHP -= state.Damage;
            
                var bar = ObjectPoolController.Self.Instantiate("UIBarFX", new PoolParameters(Position)) as UIBarFX;
                bar.Initialize(transform, 0, CurrentHP / Stats.hpMax, 0.5f);

                var tfx = ObjectPoolController.Self.Instantiate("DamageTextFX", new PoolParameters(Position)) as DamageTextFX;
                tfx.Initialize(string.Format($"{state.Damage}"), false, 0.5f);

                Utils.DamageRedPulse(spriteRenderer, 0.3f);
                Interact(state.Sender);
            }
            
#if UNITY_EDITOR
            Debug.Log(string.Format($"{state.Damage}damage dealt from {state.Sender} to {state.Getter}"));
#endif

            var param = new EventParameter(
                "BattleGroup".EventParameterPairing(BattleGroup),
                "NoticeGroup".EventParameterPairing(NoticeGroup));
            EventController.Instance.EventCall("BattleGroupEvent", param);
            EventController.Instance.EventCall("NoticeGroupEvent", param);
        }

        #endregion

    }
}
