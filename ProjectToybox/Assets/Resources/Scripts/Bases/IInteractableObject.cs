using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractState
{
    Free,
    Interactable,
    Interacting,
    OnAction,
    Fixed,
    EndInteract,
    NonInteractable
}
public interface IInteractableObject
{
    InteractState InteractState { get; set; }
    void Interact(IFieldObject target);
    void ShowInteractable();
    void HideInteractable();
}
