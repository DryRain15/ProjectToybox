using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour, IFieldObject, ICharacterObject, IMovable, IDamaged
{
    public static PlayerBehaviour Instance;
    
    private Vector3 _collisionPoint;
    private Animator _anim;
    private SpriteRenderer _sr;
    private IInteractableObject _currentItem;
    private IHoldable _currentHold;
    private float _velocityMultiplier = 1f;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponentInChildren<Animator>();
        _sr = GetComponentInChildren<SpriteRenderer>();
        AnimState = AnimState.Stand;
        HitType = HitType.Player;
    }

    // Update is called once per frame
    void Update()
    {
        MoveTo();
        FaceDirection();

        InputCheck();
    }

    private void InputCheck()
    {
        if (GlobalInputController.Instance.useKeyDown)
        {
            if (_currentHold != null)
            {
                if (_currentHold.HoldState == HoldState.Holding)
                {
                    _currentHold.Use();
                }
            }
            else if (_currentItem != null)
            {
                if (_currentItem.InteractState == InteractState.Interactable)
                {
                    _currentItem.Interact(this);
                    var holdable = _currentItem.GameObject.GetComponent<IHoldable>();
                    if (holdable != null)
                        _currentHold = holdable;
                }
            }
        }

        if (GlobalInputController.Instance.cancelKeyDown)
        {
            if (_currentHold != null)
            {
                if (_currentHold.HoldState == HoldState.Holding)
                {
                    _currentHold.Release();
                    _currentHold = null;
                }
            }
        }

    }

    private void FixedUpdate()
    {
        SetDirection();
    }

    void FaceDirection()
    {
        var face_x = ((Direction.Left & Direction) == Direction.Left) ? -0.25f : 
            ((Direction.Right & Direction) == Direction.Right) ? 0.25f : 0f;
        var face_y = ((Direction.Up & Direction) == Direction.Up) ? 0.125f : 
            ((Direction.Down & Direction) == Direction.Down) ? -0.125f : 0f;
        var face_pos = transform.position + new Vector3(face_x, face_y);
        Collider2D[] hits = Physics2D.OverlapBoxAll(face_pos, 
            new Vector2(0.5f, 0.25f), 0, 1 << 9);

        var dist = 10f;
        IInteractableObject iio = null;

        for (int i = 0; i < hits.Length; i++)
        {
            var tiio = hits[i].GetComponent<IInteractableObject>();
            if (tiio != null && 
                tiio.InteractState == InteractState.Interactable &&
                _currentHold == null)
            {
                var t_dist = Vector3.Distance(hits[i].transform.position, transform.position);
                if (t_dist < dist && 
                    (_currentHold == null || (_currentHold != null && tiio.GameObject != _currentHold.GameObject)))
                {
                    dist = t_dist;
                    iio = tiio;
                }
            }
        }

        if (iio != null)
        {
            if (_currentItem == null)
            {
                _currentItem = iio;
                _currentItem.ShowInteractable();
            }
            else if (_currentItem != iio)
            {
                _currentItem.HideInteractable();
                _currentItem = iio;
                iio.ShowInteractable();
            }
        }
        else
        {
            if (_currentItem != null)
            {
                _currentItem.HideInteractable();
                _currentItem = null;
            }
        }
            
    }

    #region ICharacterObject

    public Direction Direction { get; set; }

    public AnimState AnimState { get; set; }

    [SerializeField] private Stats stats;
    public Stats Stats { get => stats; set => stats = value; }

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

    #region IMovable

    public Vector3 Velocity { get; set; }
    
    public void MoveTo()
    {
        var singleDirectionMult = (GlobalInputController.Instance.hInput * GlobalInputController.Instance.vInput == 0)
            ? Mathf.Sqrt(5) / 2
            : 1; 
        Velocity = new Vector3(GlobalInputController.Instance.hInput,
            GlobalInputController.Instance.vInput * 0.5f) * (singleDirectionMult * Stats.moveSpeed);

        Velocity *= _velocityMultiplier;

        if (GlobalInputController.Instance.dashKeyDown)
        {
            var isDashing = _velocityMultiplier > 1f;
            if (!isDashing) {
                var core = DOTween.To(() => _velocityMultiplier, x => _velocityMultiplier = x, 3f, .15f);
                core.SetTarget(_velocityMultiplier);
                core.SetEase(Ease.OutExpo);
                core.OnComplete(() =>
                {
                    _velocityMultiplier = 1f;
                });
            }
        }

        _anim.ResetTrigger("ActionStateOnChange");
        var prevAnim = AnimState;
        if (_currentHold != null)
        {
            if (Velocity.magnitude < Mathf.Epsilon)
                AnimState = AnimState.Hold;
            else
                AnimState = AnimState.HoldMove;
        }
        else
        {
            if (Velocity.magnitude < Mathf.Epsilon)
                AnimState = AnimState.Stand;
            else
                AnimState = AnimState.Move;
        }
        
        _anim.SetInteger("ActionState", (int)AnimState);
        if (prevAnim != AnimState)
            _anim.SetTrigger("ActionStateOnChange");
        if (AnimState == AnimState.Stand | 
            AnimState == AnimState.Hold) return;
        
        var hit = Physics2D.Raycast(Position, 
            Velocity.normalized,
            Mathf.Max(Velocity.magnitude * Time.deltaTime, 0.3f), 1 << 8);
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

    #region IDamaged

    public HitType HitType { get; set; }
    public void GetHit(DamageState state)
    {
        
    }

    #endregion

    private void OnDrawGizmos()
    {
        var pos = transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pos, pos + Velocity);
        Gizmos.DrawSphere(_collisionPoint, 0.1f);
        
        var face_x = ((Direction.Left & Direction) == Direction.Left) ? -0.25f : 
            ((Direction.Right & Direction) == Direction.Right) ? 0.25f : 0f;
        var face_y = ((Direction.Up & Direction) == Direction.Up) ? 0.125f : 
            ((Direction.Down & Direction) == Direction.Down) ? -0.125f : 0f;
        var face_pos = pos + new Vector3(face_x, face_y);
        Gizmos.DrawWireCube(face_pos, new Vector2(0.5f, 0.25f));
    }
}
