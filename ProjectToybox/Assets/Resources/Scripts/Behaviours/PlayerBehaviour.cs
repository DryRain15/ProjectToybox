using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour, IFieldObject, ICharacterObject, IMovable, IDamaged
{
    // Start is called before the first frame update
    void Start()
    {
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
            Velocity.magnitude * Time.deltaTime, 8);
        if (hit) return;

        Position += Velocity * Time.deltaTime;

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
    }

    #endregion
    
    #region IDamaged

    public void GetHit()
    {
        
    }

    #endregion
}
