using Proto.Behaviours.Impl;
using UnityEngine;

namespace Proto.Behaviours.Impl
{
    public class PatrolBot : ImplEnemy
    {
        public override void Start()
        {
            base.Start();
        }

        public override void GetHit(DamageState state)
        {
            base.GetHit(state);
            if (CurrentHP <= 0f)
                gameObject.SetActive(false);
        }
    }
}