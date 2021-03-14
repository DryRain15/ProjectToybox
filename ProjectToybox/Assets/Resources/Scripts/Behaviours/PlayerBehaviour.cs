using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour, IFieldObject, ICharacterObject, IMovable, IDamaged
{
    private Vector3 _collisionPoint;
    private Animator _anim;
    private IInteractableObject _currentItem;
    
    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponentInChildren<Animator>();
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
        if (Direction != Direction.None)
        {
            if ((Direction.Up & Direction) == Direction.Up)
                _anim.SetBool("IsUp", true);
            else if ((Direction.Down & Direction) == Direction.Down)
                _anim.SetBool("IsUp", false);
            if ((Direction.Left & Direction) == Direction.Left)
                _anim.SetBool("IsLeft", true);
            else if ((Direction.Right & Direction) == Direction.Right)
                _anim.SetBool("IsLeft", false);
        }
    }

    void FaceDirection()
    {
        var face_x = ((Direction.Left & Direction) == Direction.Left) ? -0.25f : 0.25f;
        var face_y = ((Direction.Up & Direction) == Direction.Up) ? 0.125f : -0.125f;
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
                if (t_dist < dist)
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
        var hit = Physics2D.Raycast(Position, 
            Velocity.normalized,
            Mathf.Max(Velocity.magnitude * Time.deltaTime, 0.3f), 1 << 8);
        if (hit)
        {
            _collisionPoint = hit.point;
            return;
        }

        Position += Velocity * Time.deltaTime;

        if (Velocity.x < 0)
        {
            Direction = Direction.Left;
        }
        else if (Velocity.x > 0)
        {
            Direction = Direction.Right;
        }
        else
        {
            Direction = (Direction.Left & Direction) | (Direction.Right & Direction);
        }
        if (Velocity.y < 0)
        {
            Direction = Direction | Direction.Down;
        }
        else if (Velocity.y > 0)
        {
            Direction = Direction | Direction.Up;
        }
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
        
        var face_x = ((Direction.Left & Direction) == Direction.Left) ? -0.25f : 0.25f;
        var face_y = ((Direction.Up & Direction) == Direction.Up) ? 0.125f : -0.125f;
        var face_pos = pos + new Vector3(face_x, face_y);
        Gizmos.DrawWireCube(face_pos, new Vector2(0.5f, 0.25f));
    }
}
