using System.Collections;
using Proto.Behaviours;
using UnityEditor;
using UnityEngine;

namespace Proto.Behaviours.Impl.Tutorial
{
    public class Bed : AbstractInteractableObject
    {

        protected override void OnInteract(ICharacterObject interacted)
        {
            ExitGame();
            InteractState = InteractState.EndInteract;
        }

        private void ExitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
        }
    }

}
