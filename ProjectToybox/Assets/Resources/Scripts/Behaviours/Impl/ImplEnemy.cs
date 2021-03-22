using Proto;
using Proto.Behaviours;
using UnityEngine;

namespace Proto.Behaviours.Impl
{
    public class ImplEnemy : AbstractInteractableObject, IDamaged, IAutoBehaviour, IMovable
    {
        private Collider2D _col;
        private Vector3 _collisionPoint;
        private Animator _anim;
        private IPooledObject _showRange;

        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
            HitType = HitType.Enemy;
            _anim = GetComponentInChildren<Animator>();
            _col = GetComponent<Collider2D>();

            EventController.Instance.Subscribe("DebugEvent", DebugEvent);
            EventController.Instance.Subscribe("BattleGroupEvent", ToBattleState);
            EventController.Instance.Subscribe("NoticeGroupEvent", ToNoticeState);
        }

        private void DebugEvent(EventParameter param)
        {
            var spaceKey = param.Get<bool>("SpaceKey");
            if (spaceKey)
            {
                Interact(FindObjectOfType<PlayerBehaviour>());
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
            MoveTo();
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

                _anim.SetBool("IsUp", up);
                _anim.SetBool("IsDown", down);
                _anim.SetBool("IsLeft", left);
                _anim.SetBool("IsRight", right);

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

        public void AutoUpdate()
        {
            if (InteractState == InteractState.OnAction)
            {
                Velocity = Vector3.zero;
                return;
            }

            switch (AutoState)
            {
                case AutoState.None:
                    return;
                case AutoState.Wait:
                    break;
                case AutoState.Follow:
                    var v = PlayerBehaviour.Instance.Position - Position;
                    Direction = Utils.ClampVectorToDirection(v);
                    var singleDirectionMult = (Utils.IsHorizontal(Direction) || Utils.IsVertical(Direction))
                        ? Mathf.Sqrt(5) / 4
                        : 0.5f;
                    Velocity = Utils.DirectionToVector(Direction) * (singleDirectionMult * Stats.moveSpeed);
                    if (this.TargetInRange(PlayerBehaviour.Instance, 1f))
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

        private void OnActionStateUpdate()
        {
            if (innerTimer > 1f)
                InteractState = InteractState.EndInteract;
            var yVelocity = 2.5f - 2.5f * (innerTimer / 0.5f);
            Transform.GetChild(0).localPosition += new Vector3(0f, 1f, 2f) * (yVelocity * Time.deltaTime);
        }


        #region IInteractableObject

        public override void Interact(ICharacterObject target)
        {
            if (AutoState == AutoState.Action)
            {
                innerTimer = 0f;
                InteractState = InteractState.OnAction;
                if (_showRange == null)
                {
                    _showRange = ObjectPoolController.Self.Instantiate("RadialFX",
                        new PoolParameters(transform.position));
                    var (x, y) = Direction.DirectionToRange();
                    if (Direction == Direction.Up)
                        _showRange.gameObject.GetComponent<RadialFX>()
                            .Initialize(x, y, Color.red, 2f, y);
                    else
                        _showRange.gameObject.GetComponent<RadialFX>().Initialize(
                            x, y, Color.red, 2f);
                }
                return;
            }
            if (InteractState == InteractState.Interactable)
            {
                innerTimer = 0f;
                InteractState = InteractState.OnAction;
                return;
            }

        }

        #endregion

        #region IDamaged

        public HitType HitType { get; set; }
        public void GetHit(DamageState state)
        {
            if (InteractState == InteractState.OnAction) return;
            Utils.DamageRedPulse(spriteRenderer);
            Interact(state.Sender);
            Debug.Log(string.Format($"{state.Damage}damage dealt from {state.Sender} to {state.Getter}"));

            var param = new EventParameter(
                "BattleGroup".EventParameterPairing(BattleGroup),
                "NoticeGroup".EventParameterPairing(NoticeGroup));
            EventController.Instance.EventCall("BattleGroupEvent", param);
            EventController.Instance.EventCall("NoticeGroupEvent", param);
        }

        #endregion

    }
}
