using Proto.Behaviours;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Proto.Behaviours.Impl.Tutorial
{
    public class Door : AbstractInteractableObject
    {

        protected override void OnInteract(ICharacterObject interacted)
        {
            NextPhase();
            InteractState = InteractState.EndInteract;
        }

        private void NextPhase()
        {
            //TODO: next to phase 3
            // SceneManager.LoadSceneAsync("Phase3");
        }
    }

}
