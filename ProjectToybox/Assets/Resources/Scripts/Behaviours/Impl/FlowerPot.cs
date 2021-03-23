using Proto.Behaviours;
using UnityEngine;

namespace Proto.Behaviours.Impl
{
    public class FlowerPot : AbstractHoldableObject
    {
        protected override void OnInteract(ICharacterObject interacted)
        {
            InteractState = InteractState.EndInteract;
        }
    }
}
