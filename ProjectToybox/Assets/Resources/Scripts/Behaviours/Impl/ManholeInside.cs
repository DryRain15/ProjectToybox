using System.Collections;
using Proto.Behaviours;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Proto.Behaviours.Impl
{
    public class ManholeInside : AbstractInteractableObject
    {
        protected override void OnInteract(ICharacterObject interacted)
        {
            StartCoroutine(GoCredit());
            InteractState = InteractState.EndInteract;
        }

        private IEnumerator GoCredit()
        {
            yield return ScreenUIController.Instance.ScreenFadeCall(new Color(0, 0, 0, 0), 1f);
            SceneManager.LoadScene("Credit");
        }
    }
}
