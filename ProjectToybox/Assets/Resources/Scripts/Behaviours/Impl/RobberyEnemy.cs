using Proto.Behaviours.Impl;
using UnityEngine;

namespace Proto.Behaviours.Impl
{
    public class RobberyEnemy : ImplEnemy
    {
        public override void Start()
        {
            base.Start();
            AutoState = AutoState.Wait;
        }

        public override void GetHit(DamageState state)
        {
            base.GetHit(state);
            if (CurrentHP <= 0f)
            {
                if (_showRange != null)
                {
                    _showRange.Dispose();
                    _showRange = null;
                }

                gameObject.SetActive(false);
            }
        }
        
        public override void AutoUpdate()
        {
            if (InteractState == InteractState.OnAction)
            {
                Velocity = Vector3.zero;
                return;
            }

            switch (AutoState)
            {
                case AutoState.None:
                    break;
                case AutoState.Wait:
                    if (this.TargetInRange(PlayerBehaviour.Instance, sightRange))
                    {
                        AutoState = AutoState.Follow;
                        var param = new EventParameter(
                            "BattleGroup".EventParameterPairing(BattleGroup),
                            "NoticeGroup".EventParameterPairing(NoticeGroup));
                        EventController.Instance.EventCall("BattleGroupEvent", param);
                        EventController.Instance.EventCall("NoticeGroupEvent", param);
                    }
                    break;
                case AutoState.Follow:
                    var v = PlayerBehaviour.Instance.Position - Position;
                    Direction = Utils.ClampVectorToDirection(v);
                    var singleDirectionMult = (Utils.IsHorizontal(Direction) || Utils.IsVertical(Direction))
                        ? Mathf.Sqrt(5) / 4
                        : 0.5f;
                    Velocity = Utils.DirectionToVector(Direction) * (singleDirectionMult * Stats.moveSpeed);
                    if (this.TargetInRange(PlayerBehaviour.Instance, attackRange))
                    {
                        AutoState = AutoState.Action;
                    
                        AnimState = AnimState.Attack;
                        _anim.SetInteger("ActionState", (int)AnimState);
                        _anim.SetTrigger("ActionStateOnChange");

                        Interact(PlayerBehaviour.Instance);
                    }
                    break;
                case AutoState.Action:
                    if (InteractState != InteractState.OnAction)
                    {
                        AutoState = AutoState.Follow;
                        if (_showRange != null)
                        {
                            _showRange.Dispose();
                            _showRange = null;
                        }
                    }
                    break;
            }
        }
        protected override void OnActionStateUpdate()
        {
            if (innerTimer > 1f * attackTime)
                InteractState = InteractState.EndInteract;
            if (innerTimer > 0.5f * attackTime)
            {
                if (_showRange != null)
                {
                    _showRange.Dispose();
                    _showRange = null;

                    var state = new DamageState
                    {
                        Damage = Stats.atk,
                        DamageType = 0,
                        Getter = PlayerBehaviour.Instance,
                        Sender = this,
                        KnockBack = 1f,
                    };
                    if (this.TargetInRangeAngle(PlayerBehaviour.Instance, attackRange, Direction))
                        PlayerBehaviour.Instance.GetHit(state);
                }
            }
            else
            {
                var yVelocity = 1.5f - 1.5f * (innerTimer * 4f / attackTime);
                Transform.GetChild(0).localPosition += new Vector3(0f, 1f, 2f) * (yVelocity * Time.deltaTime);
            }
        }

    }

}
