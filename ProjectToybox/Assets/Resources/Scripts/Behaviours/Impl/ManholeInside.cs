using Proto.Behaviours;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Proto.Behaviours.Impl
{
    public class ManholeInside : AbstractInteractableObject
    {
        protected override void OnInteract(ICharacterObject interacted)
        {
            SceneManager.LoadScene("Credit");
            InteractState = InteractState.EndInteract;
        }
    }
}
