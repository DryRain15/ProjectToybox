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
        
    }

    // Update is called once per frame
    void Update()
    {
        HoldableUpdate();
    }

    #region IFieldObject

    public string Name { get; set; }
    public Vector3 Position { get; set; }
    
    #endregion

    #region IHoldable

    public HoldState HoldState { get; set; }
    
    public ICharacterObject Holder { get; set; }
    
    public void Hold()
    {
        
    }
    
    public void Use()
    { 
        
    }
    
    public void Release()
    {
        
    }

    public void HoldableUpdate()
    {
        
    }
    
    #endregion

    #region IInteractableObject
    
    public InteractState InteractState { get; set; }

    public void Interact(ICharacterObject target)
    {
        
    }

    public void ShowInteractable()
    {
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
