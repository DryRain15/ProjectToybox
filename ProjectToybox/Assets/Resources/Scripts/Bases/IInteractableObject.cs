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
    GameObject GameObject { get; }
    InteractState InteractState { get; set; }
    void Interact(ICharacterObject target);
    void ShowInteractable();
    void HideInteractable();
}
