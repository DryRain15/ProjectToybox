using Proto.Behaviours;
using UnityEngine;

namespace Proto.Behaviours.Impl
{
    public class ImplInteractableToy : AbstractInteractableObject, IDamaged
    {
        public override void Start()
        {
            base.Start();
            HitType = HitType.Enemy;
        }

        protected override void InteractStateUpdate(InteractState state)
        {
            if (state == InteractState.OnAction)
            {
                if (innerTimer > 1f)
                    InteractState = InteractState.EndInteract;
                var yVelocity = 2.5f - 2.5f * (innerTimer / 0.5f);
                transform.localPosition += new Vector3(0f, 1f, 2f) * (yVelocity * Time.deltaTime);
            }
        }

        protected override void OnInteract(ICharacterObject interacted)
        {
            InteractState = InteractState.OnAction;
        }

        #region IDamaged

        public HitType HitType { get; set; }
        public void GetHit(DamageState state)
        {
            if (InteractState != InteractState.Interactable)
                return;

            Utils.DamageRedPulse(spriteRenderer);
            Interact(state.Sender);
            Debug.Log(string.Format($"{state.Damage}damage dealt from {state.Sender} to {state.Getter}"));
        }

        #endregion

    }
}
