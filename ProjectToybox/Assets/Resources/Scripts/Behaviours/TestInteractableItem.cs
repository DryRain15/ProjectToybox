using System.Collections;
using System.Collections.Generic;
using Proto;
using UnityEngine;

public class TestInteractableItem : MonoBehaviour, IInteractableObject, IHoldable, IFieldObject
{
    private IPooledObject _interactableFX;
    
    // Start is called before the first frame update
    void Start()
    {
        InteractState = InteractState.Interactable;
    }

    // Update is called once per frame
    void Update()
    {
        HoldableUpdate();
        
        if (InteractState == InteractState.EndInteract)
            InteractState = InteractState.Interactable;
    }

    #region IFieldObject

    public string Name { get; set; }
    public Transform Transform { get => transform;}
    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }

    #endregion

    #region IHoldable

    public HoldState HoldState { get; set; }
    
    public ICharacterObject Holder { get; set; }
    
    public void Hold(IFieldObject target)
    {
        transform.SetParent(target.Transform);
        transform.localPosition = new Vector3(0.5f, 0.75f);
    }
    
    public void Use()
    { 
        
    }
    
    public void Release()
    {
        transform.localPosition = new Vector3(0f, 0f);
        transform.SetParent(null);
    }

    public void HoldableUpdate()
    {
        
    }
    
    #endregion

    #region IInteractableObject
    
    public InteractState InteractState { get; set; }

    public void Interact(IFieldObject target)
    {
        if (InteractState == InteractState.Interactable)
        {
            InteractState = InteractState.Interacting;
            Hold(target);
        }
        else if (InteractState == InteractState.Interacting) 
        {
            InteractState = InteractState.EndInteract;
            Release();
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
        _interactableFX.Dispose();
        _interactableFX = null;
    }
    
    #endregion

}
