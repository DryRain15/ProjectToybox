using System;
using System.Collections;
using System.Collections.Generic;
using Proto;
using UnityEngine;

public class TestInteractableEnemy : MonoBehaviour, IInteractableObject, IFieldObject, ICharacterObject, IDamaged, IAutoBehaviour, IMovable
{
    private Collider2D _col;
    private Vector3 _collisionPoint;
    private Animator _anim;
    private IPooledObject _interactableFX;
    private SpriteRenderer _sr;
    private float _innerTimer;
    private IPooledObject _showRange;
    
    // Start is called before the first frame update
    void Start()
    {
        AutoState = AutoState.None;
        InteractState = InteractState.Interactable;
        AnimState = AnimState.Stand;
        Direction = Direction.Down | Direction.Right;
        HitType = HitType.Enemy;
        _innerTimer = 0f;
        _sr = GetComponentInChildren<SpriteRenderer>();
        _anim = GetComponentInChildren<Animator>();
        _col = GetComponent<Collider2D>();
        
        EventController.Instance.Subscribe("DebugEvent", DebugEvent);
        EventController.Instance.Subscribe("BattleGroupEvent", ToBattleState);
        EventController.Instance.Subscribe("NoticeGroupEvent", ToNoticeState);
    }

    void DebugEvent(EventParameter param)
    {
        var spaceKey = param.Get<bool>("SpaceKey");
        if (spaceKey)
        {
            Interact(FindObjectOfType<PlayerBehaviour>());
        }
    }

    void ToBattleState(EventParameter param)
    {
        var paramBattleGroup = param.Get<string>("BattleGroup");
        if (paramBattleGroup == null) return;
        if(paramBattleGroup == BattleGroup)
            OnStateTransition(AutoState.Wait);
    }

    void ToNoticeState(EventParameter param)
    {
        var paramNoticeGroup = param.Get<string>("NoticeGroup");
        if (paramNoticeGroup == null) return;
        if(paramNoticeGroup == NoticeGroup)
            OnStateTransition(AutoState.Follow);
    }

    // Update is called once per frame
    void Update()
    {
        InteractableUpdate();
        AutoUpdate();
        MoveTo();
        
        if (InteractState == InteractState.EndInteract)
            InteractState = InteractState.Interactable;

        _innerTimer += Time.deltaTime;
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

            _sr.flipX = right;
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

    #region IFieldObject

    public string Name { get; set; }
    public GameObject GameObject { get => gameObject; }
    public Transform Transform { get => transform;}
    public Transform GFXTransform { get => transform.GetChild(0); }
    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }

    #endregion

    public void InteractableUpdate()
    {
        switch (InteractState)
        {
            case InteractState.Interactable:
                InteractableStateUpdate();
                break;
            case InteractState.OnAction:
                OnActionStateUpdate();
                break;
        }
    }

    private void InteractableStateUpdate()
    {
    }

    private void OnActionStateUpdate()
    {
        if (_innerTimer > 1f)
            InteractState = InteractState.EndInteract;
        var yVelocity = 2.5f - 2.5f * (_innerTimer / 0.5f);
        Transform.GetChild(0).localPosition += new Vector3(0f, 1f, 2f) * (yVelocity * Time.deltaTime);
    }


    #region IInteractableObject

    public InteractState InteractState { get; set; }

    public void Interact(ICharacterObject target)
    {
        if (AutoState == AutoState.Action)
        {
            _innerTimer = 0f;
            InteractState = InteractState.OnAction;
            if (_showRange == null)
            {
                _showRange = ObjectPoolController.Self.Instantiate("RadialFX",
                    new PoolParameters(transform.position));
                var range = Direction.DirectionToRange();
                if (Direction == Direction.Up)
                    _showRange.gameObject.GetComponent<RadialFX>()
                        .Initialize(range.Item1, range.Item2, Color.red, 2f, range.Item2);
                else
                    _showRange.gameObject.GetComponent<RadialFX>().Initialize(
                        range.Item1, range.Item2, Color.red, 2f);
            }
            return;
        }
        if (InteractState == InteractState.Interactable)
        {
            _innerTimer = 0f;
            InteractState = InteractState.OnAction;
            return;
        }

    }

    public void ShowInteractable()
    {
        if (InteractState != InteractState.Interactable) return;

        if (_interactableFX == null)
            _interactableFX = ObjectPoolController.Self.Instantiate("InteractableFX",
            new PoolParameters(transform.position + Vector3.up * 0.5f));
    }

    public void HideInteractable()
    {
        if (_interactableFX == null) return;

        _interactableFX.Dispose();
        _interactableFX = null;
    }

    #endregion

    #region ICharacterObject

    public Direction Direction { get; set; }

    public AnimState AnimState { get; set; }

    [SerializeField] private Stats stats;
    public Stats Stats { get => stats; set => stats = value; }

    #endregion

    #region IDamaged

    public HitType HitType { get; set; }
    public void GetHit(DamageState state)
    {
        if (InteractState == InteractState.OnAction) return;
        Utils.DamageRedPulse(_sr);
        Interact(state.Sender);
        Debug.Log(string.Format($"{state.Damage}damage dealt from {state.Sender} to {state.Getter}"));
        EventParameter param = new EventParameter(
            "BattleGroup".EventParameterPairing(BattleGroup), 
            "NoticeGroup".EventParameterPairing(NoticeGroup)); 
        EventController.Instance.EventCall("BattleGroupEvent", param);
        EventController.Instance.EventCall("NoticeGroupEvent", param);
    }

    #endregion

}
