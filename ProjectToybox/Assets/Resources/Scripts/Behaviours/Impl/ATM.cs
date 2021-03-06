using Proto.Behaviours;
using UnityEngine;

namespace Proto.Behaviours.Impl
{
    public class ATM : AbstractInteractableObject
    {
        protected override void OnInteract(ICharacterObject interacted)
        {
            InteractState = InteractState.EndInteract;
        }
    }
}
