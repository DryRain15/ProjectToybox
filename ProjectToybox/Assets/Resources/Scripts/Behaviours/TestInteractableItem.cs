using System.Collections;
using System.Collections.Generic;
using Proto;
using UnityEngine;

public class TestInteractableItem : MonoBehaviour, IInteractableObject, IHoldable, IFieldObject
{
    private IPooledObject _interactableFX;
    private float _innerTimer;
    
    // Start is called before the first frame update
    void Start()
    {
        InteractState = InteractState.Interactable;
        HoldState = HoldState.None;
        _innerTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        HoldableUpdate();
        
        if (InteractState == InteractState.EndInteract)
            InteractState = InteractState.Interactable;

        if (HoldState == HoldState.EndHold)
            HoldState = HoldState.None;
        if (HoldState == HoldState.StartHold)
            HoldState = HoldState.Holding;

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

    #region IHoldable

    public HoldState HoldState { get; set; }
    
    public ICharacterObject Holder { get; set; }
    
    public void Hold(ICharacterObject target)
    {
        HoldState = HoldState.StartHold;
        Holder = target;
        transform.SetParent(target.Transform);
        transform.localPosition = new Vector3(0.5f, 0.75f);
    }
    
    public void Use()
    {
        _innerTimer = 0f;
        HoldState = HoldState.OnAction;
        InteractState = InteractState.OnAction;
    }
    
    public void Release()
    {
        HoldState = HoldState.EndHold;
        InteractState = InteractState.EndInteract;
        Holder = null;
        transform.localPosition = Vector3.zero;
        transform.SetParent(null);
    }

    public void HoldableUpdate()
    {
        switch (HoldState)
        {
            case HoldState.Holding:
                HoldingStateUpdate();
                break;
            case HoldState.OnAction:
                OnActionStateUpdate();
                break;
        }
    }

    private void HoldingStateUpdate()
    {
        transform.localPosition = Utils.GetAngularOffset(
            Utils.RotateDirectionCW(Holder.Direction, 2), 0.5f) + Vector3.up * 0.75f;
    }

    private void OnActionStateUpdate()
    {
        if (_innerTimer < 0.1f)
        {
            var initPos = Utils.GetAngularOffset(
                Utils.RotateDirectionCW(Holder.Direction, 2), 0.5f) + Vector3.up * 0.75f;
            var endPos = Utils.GetAngularOffset(
                Utils.RotateDirectionCW(Holder.Direction, 1), 0.5f) + Vector3.up * 0.6f;

            transform.localPosition = Vector3.Lerp(initPos, endPos, _innerTimer / 0.1f);
        }
        else if (_innerTimer < 0.2f)
        {
            var initPos = Utils.GetAngularOffset(
                Utils.RotateDirectionCW(Holder.Direction, 1), 0.5f) + Vector3.up * 0.6f;
            var endPos = Utils.GetAngularOffset(Holder.Direction, 0.5f) + Vector3.up * 0.45f;

            transform.localPosition = Vector3.Lerp(initPos, endPos, (_innerTimer - 0.1f) / 0.1f);
        }
        else if (_innerTimer < 0.3f)
        {
            var initPos = Utils.GetAngularOffset(Holder.Direction, 0.5f) + Vector3.up * 0.45f;
            var endPos = Utils.GetAngularOffset(
                Utils.RotateDirectionCCW(Holder.Direction, 1), 0.5f) + Vector3.up * 0.3f;

            transform.localPosition = Vector3.Lerp(initPos, endPos, (_innerTimer - 0.2f) / 0.1f);
        }
        else
        {
            HoldState = HoldState.Holding;
        }
    }
    
    #endregion

    #region IInteractableObject
    
    public InteractState InteractState { get; set; }

    public void Interact(ICharacterObject target)
    {
        if (InteractState == InteractState.Interactable)
        {
            InteractState = InteractState.Interacting;
            Hold(target);
        }
        else if (InteractState == InteractState.Interacting) 
        {
            Use();
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

}
