using Proto.Behaviours;
using UnityEngine;

namespace Proto.Behaviours
{
    public abstract class AbstractHoldableObject : AbstractInteractableObject, IHoldable
    {
        public override void Update()
        {
            base.Update();
            HoldableUpdate();

            if (HoldState == HoldState.EndHold)
                HoldState = HoldState.None;
            if (HoldState == HoldState.StartHold)
                HoldState = HoldState.Holding;
        }

        public override void Interact(ICharacterObject target)
        {
            switch (InteractState)
            {
                case InteractState.Interactable:
                    InteractState = InteractState.Interacting;
                    Hold(target);
                    break;
                case InteractState.Interacting:
                    Use();
                    break;
            }
        }

        #region IHoldable

        public HoldState HoldState { get; set; }
        public ICharacterObject Holder { get; set; }

        protected virtual void HoldStateUpdate(HoldState state) {}
        protected virtual void OnHold() {}
        protected virtual void OnUse() {}
        protected virtual void OnRelease() {}

        public void Hold(ICharacterObject target)
        {
            HoldState = HoldState.StartHold;
            Holder = target;
            transform.SetParent(target.Transform);
            transform.localPosition = new Vector3(0.5f, 0.75f);
            OnHold();
        }

        public void Use()
        {
            innerTimer = 0f;
            HoldState = HoldState.OnAction;
            InteractState = InteractState.OnAction;
            OnUse();
        }

        public void Release()
        {
            HoldState = HoldState.EndHold;
            InteractState = InteractState.EndInteract;
            Holder = null;
            transform.localPosition = Vector3.zero;
            transform.SetParent(null);
            OnRelease();
        }

        public void HoldableUpdate()
        {
            HoldStateUpdate(HoldState);
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
            if (innerTimer < 0.05f)
            {
                var initPos = Utils.GetAngularOffset(
                    Utils.RotateDirectionCW(Holder.Direction, 2), 0.5f) + Vector3.up * 0.75f;
                var endPos = Utils.GetAngularOffset(
                    Utils.RotateDirectionCW(Holder.Direction), 0.5f) + Vector3.up * 0.6f;

                transform.localPosition = Vector3.Lerp(initPos, endPos, innerTimer / 0.1f);
            }
            else if (innerTimer < 0.08f)
            {
                var initPos = Utils.GetAngularOffset(
                    Utils.RotateDirectionCW(Holder.Direction, 1), 0.5f) + Vector3.up * 0.6f;
                var endPos = Utils.GetAngularOffset(Holder.Direction, 0.5f) + Vector3.up * 0.35f;

                transform.localPosition = Vector3.Lerp(initPos, endPos, (innerTimer - 0.1f) / 0.1f);
            }
            else if (innerTimer < 0.13f)
            {
                var initPos = Utils.GetAngularOffset(Holder.Direction, 0.5f) + Vector3.up * 0.35f;
                var endPos = Utils.GetAngularOffset(
                    Utils.RotateDirectionCCW(Holder.Direction, 1), 0.5f) + Vector3.up * 0.15f;

                transform.localPosition = Vector3.Lerp(initPos, endPos, (innerTimer - 0.2f) / 0.1f);
            }
            else if (innerTimer > 0.3f)
            {
                HoldState = HoldState.Holding;
            }

            OnCollisionCheck();
        }

        private void OnCollisionCheck()
        {
            var pos = Holder.Transform.position + Utils.GetAngularOffset(Holder.Direction, 0.5f);
            Collider2D[] hits = Physics2D.OverlapBoxAll(pos,
                new Vector2(1f, 0.5f), 0, 1 << 9);


            foreach (var col in hits)
            {
                var tidmg = col.GetComponent<IDamaged>();
                if (tidmg != null && tidmg.HitType == HitType.Enemy)
                {
                    var state = new DamageState
                    {
                        Damage = Holder.Stats.atk,
                        DamageType = 0,
                        Getter = tidmg.GameObject.GetComponent<ICharacterObject>(),
                        Sender = Holder,
                        KnockBack = 1f,
                    };
                    if(OnHit(state))
                        tidmg.GetHit(state);
                }
            }

        }

        #endregion

    }
}
