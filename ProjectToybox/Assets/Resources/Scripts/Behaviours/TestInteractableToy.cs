using System.Collections;
using System.Collections.Generic;
using Proto;
using UnityEngine;

public class TestInteractableToy : MonoBehaviour, IInteractableObject, IFieldObject, ICharacterObject
{
    private IPooledObject _interactableFX;
    private float _innerTimer;
    
    // Start is called before the first frame update
    void Start()
    {
        InteractState = InteractState.Interactable;
        Direction = Direction.Down | Direction.Right;
        Stats = new Stats()
        {
            Hp = 10f,
            HpGen = 0.1f,
            Atk = 3f,
            Fever = 1.3f,
            MoveSpeed = 2f,
        };
        _innerTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        InteractableUpdate();
        
        if (InteractState == InteractState.EndInteract)
            InteractState = InteractState.Interactable;

        _innerTimer += Time.deltaTime;
    }

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
        transform.localPosition += new Vector3(0f, 1f, 2f) * (yVelocity * Time.deltaTime);
    }
    

    #region IInteractableObject
    
    public InteractState InteractState { get; set; }

    public void Interact(ICharacterObject target)
    {
        if (InteractState == InteractState.Interactable)
        {
            _innerTimer = 0f;
            InteractState = InteractState.OnAction;
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

    public Stats Stats { get; set; }

    #endregion
}
