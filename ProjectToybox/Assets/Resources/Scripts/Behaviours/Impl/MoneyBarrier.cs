using System;
using Proto.Behaviours;
using UnityEngine;

namespace Proto.Behaviours.Impl
{
    public class MoneyBarrier : AbstractHoldableObject
    {
        public PolygonCollider2D col;

        protected override void OnHold()
        {
            col.enabled = false;
        }

        protected override void OnRelease()
        {
            col.enabled = true;
        }
    }
}
