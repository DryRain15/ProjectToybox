using System.Collections;
using DG.Tweening;
using DG.Tweening.Plugins.Options;
using UnityEditor;
using UnityEngine;

public class Phase2 : MonoBehaviour
{

    private void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    private void NextPhase()
    {
        Debug.Log("next phase");
    }

}
