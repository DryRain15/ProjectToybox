using System.Collections;
using DG.Tweening;
using DG.Tweening.Plugins.Options;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Phase1 : MonoBehaviour
{

    private const float PHASE_TIME = 10f;

    public AudioClip alarmClip;
    private AudioSource alarmSource;

    private void Start()
    {
        alarmSource = this.CreateSource(alarmClip);
        alarmSource.Play();
        alarmSource.volume = 0f;
        alarmSource.DOFade(1f, PHASE_TIME);
        CoroutineManager.Instance.StartCoroutineCall(PhaseOver());
    }

    private IEnumerator PhaseOver()
    {
        yield return new WaitForSeconds(PHASE_TIME);
        alarmSource.Stop();
        ExitGame();
    }

    private void Update()
    {
        if (!Input.anyKeyDown)
        {
            return;
        }

        Debug.Log("any keydcow");

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitGame();
        }
        NextPhase();
    }

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
        SceneManager.LoadScene("Scenes/Phase2");
        CoroutineManager.Instance.StopCoroutineCall(PhaseOver());
    }

}
