using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour, IFieldObject, ICharacterObject, IMovable, IDamaged
{
    private Vector3 _collisionPoint;
    private Animator _anim;
    private SpriteRenderer _sr;
    private IInteractableObject _currentItem;
    private IHoldable _currentHold;
    
    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponentInChildren<Animator>();
        _sr = GetComponentInChildren<SpriteRenderer>();
        Stats = new Stats()
        {
            Hp = 10f,
            HpGen = 0.1f,
            Atk = 3f,
            Fever = 1.3f,
            MoveSpeed = 2f,
        };
    }

    // Update is called once per frame
    void Update()
    {
        
        Velocity = new Vector3(GlobalInputController.Instance.hInput,
            GlobalInputController.Instance.vInput * 0.5f) * Stats.MoveSpeed;
        MoveTo();
        FaceDirection();

        InputCheck();
        
        if (Direction != Direction.None)
        {
            var up = (Direction.Up & Direction) == Direction.Up;
            var down = (Direction.Down & Direction) == Direction.Down;
            var left = (Direction.Left & Direction) == Direction.Left;
            var right = (Direction.Right & Direction) == Direction.Right;
                
            _anim.SetBool("IsUp", up);
            _anim.SetBool("IsDown", down);
            _anim.SetBool("IsLeft", left);
            _anim.SetBool("IsRight", right);

            _sr.flipX = right;
        }
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
            if (tiio != null)
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

    public Stats Stats { get; set; }

    #endregion

    #region IFieldObject
    
    public string Name { get; set; }
    
    public GameObject GameObject { get => gameObject; }
    public Transform Transform { get => transform;}
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
        if (Velocity.magnitude < Mathf.Epsilon) return;
        
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
    }

    #endregion
    
    #region IDamaged

    public void GetHit()
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
