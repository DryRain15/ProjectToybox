using System.Collections;
using Proto.Behaviours;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Proto.Behaviours.Impl.Tutorial
{
    public class Door : AbstractInteractableObject
    {

        public string targetPhase;

        protected override void OnInteract(ICharacterObject interacted)
        {
            CoroutineManager.Instance.StartCoroutineCall(DoorTransition(interacted));
        }

        private IEnumerator DoorTransition(ICharacterObject target)
        {
            bool toDoor;
            if (FieldObjectController.FOs.ContainsKey(targetPhase))
                toDoor = true;
            else
                toDoor = false;
            
            yield return ScreenUIController.Instance.ScreenFadeCall(Color.black, toDoor ? 0.5f : 1f);
            
            if (toDoor)
            {
                GetToDoor(target);
                yield return ScreenUIController.Instance.ScreenFadeCall(new Color(0, 0, 0, 0), toDoor ? 0.5f : 1f);
            }
            else NextPhase();
            
            
            InteractState = InteractState.EndInteract;
        }

        private void NextPhase()
        {
            SceneManager.LoadSceneAsync(targetPhase);
        }
        
        private void GetToDoor(ICharacterObject target)
        {
            target.SetPosition(FieldObjectController.FOs[targetPhase].Position);
        }
    }

}
