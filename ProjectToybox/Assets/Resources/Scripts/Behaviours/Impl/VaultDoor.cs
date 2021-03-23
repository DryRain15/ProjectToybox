using System.Collections;
using Proto.Behaviours;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Proto.Behaviours.Impl
{
    public class VaultDoor : AbstractInteractableObject, IDynamicSprite
    {
        [SerializeField] private Sprite[] sprites;

        public Vector2 offset;
        public string targetPhase;

        public void SetSprite(int index = 0)
        {
            if (index < 0 || index >= sprites.Length)
                index = 0;

            spriteRenderer.sprite = sprites[index];
        }
        protected override void OnInteract(ICharacterObject interacted)
        {
            CoroutineManager.Instance.StartCoroutineCall(DoorTransition(interacted));
        }

        private IEnumerator DoorTransition(ICharacterObject target)
        {
            GlobalInputController.Instance.RemoveControl();
            bool toDoor;
            if (FieldObjectController.FOs.ContainsKey(targetPhase))
                toDoor = true;
            else
                toDoor = false;

            if (!toDoor) yield break;
            
            SetSprite(1);
            yield return ScreenUIController.Instance.ScreenFadeCall(Color.black, 1);
            
            GetToDoor(target);
            yield return ScreenUIController.Instance.ScreenFadeCall(new Color(0, 0, 0, 0), 1);
            GlobalInputController.Instance.RestoreControl();

            InteractState = InteractState.EndInteract;
        }

        private void GetToDoor(ICharacterObject target)
        {
            target.SetPosition(FieldObjectController.FOs[targetPhase].Position + (Vector3)offset);
        }
    }
}
