using Proto.Behaviours;
using UnityEngine;

namespace Proto.Behaviours.Impl
{
    public class ImplHoldable : AbstractHoldableObject
    {
        protected override void HoldStateUpdate(HoldState state)
        {
            Debug.Log("hold state = " + state);
        }
        protected override void OnHold()
        {
            Debug.Log("OnHold");
        }
        protected override void OnUse()
        {
            Debug.Log("OnUse");
        }
        protected override void OnRelease()
        {
            Debug.Log("OnRelease");
        }

        protected override bool OnHit(DamageState state)
        {
            state.Damage = 10f;
            return base.OnHit(state);
        }
    }
}
