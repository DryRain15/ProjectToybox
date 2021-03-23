using System;
using UnityEngine;

namespace Proto.Behaviours
{
    public class AbstractInteractableObject : MonoBehaviour, IInteractableObject, IFieldObject, ICharacterObject
    {
        private IPooledObject _interactableFX;
        protected SpriteRenderer spriteRenderer;
        protected float innerTimer;
        public float CurrentHP { get; set; }

        #region IFieldObject

        public string Name { get => gameObject.name; set => gameObject.name = value; }
        public GameObject GameObject => gameObject;
        public Transform Transform => transform;
        public Transform GFXTransform => transform.GetChild(0);
        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        #endregion

        #region ICharacterObject

        public Direction Direction { get; set; }
        public AnimState AnimState { get; set; }
        [SerializeField] private Stats stats;
        public Stats Stats { get => stats; set => stats = value; }

        #endregion

        public virtual void Start()
        {
            InteractState = InteractState.Interactable;
            AnimState = AnimState.Stand;
            Direction = Direction.Down | Direction.Right;
            innerTimer = 0f;
            CurrentHP = stats.hpMax;
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            FieldObjectController.FOs.Add(Name, this);
        }

        public virtual void Update()
        {
            InteractStateUpdate(InteractState);

            if (InteractState == InteractState.EndInteract)
                InteractState = InteractState.Interactable;

            innerTimer += Time.deltaTime;
        }

        protected virtual void InteractStateUpdate(InteractState state) {}
        protected virtual void OnInteract(ICharacterObject interacted) {}

        #region IInteractableObject

        public InteractState InteractState { get; set; }

        public virtual void Interact(ICharacterObject target)
        {
            if (InteractState == InteractState.Interactable)
            {
                innerTimer = 0f;
                InteractState = InteractState.OnAction;
                OnInteract(target);
            }
        }
        public void ShowInteractable()
        {
            if (InteractState != InteractState.Interactable) return;

            _interactableFX ??= ObjectPoolController.Self.Instantiate("InteractableFX",
                new PoolParameters(transform.position + Vector3.up * 0.5f));
        }

        public void HideInteractable()
        {
            if (_interactableFX == null) return;

            _interactableFX.Dispose();
            _interactableFX = null;
        }

        #endregion

        //리턴 값이 true면 데미지 적용, false면 데미지 적용 안하고 캔슬
        protected virtual bool OnHit(DamageState state)
        {
            return true;
        }

    }
}
