using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenUIController : MonoBehaviour
{
    public static ScreenUIController Instance;
    private Canvas _canvas;
    private RawImage _fade;

    private void Awake()
    {
        if(Instance != null) Destroy(gameObject);
        else Instance = this;

        _canvas = GetComponent<Canvas>();

        _fade = transform.Find("Fade")?.GetComponent<RawImage>();
        if (_fade == null)
        {
            _fade = new GameObject("Fade").AddComponent<RawImage>();
            _fade.transform.SetParent(transform);
        }
    }

    private void Start()
    {
        _canvas.worldCamera = Camera.main;
        _fade.color = Color.black;
        ScreenFadeCall(new Color(0, 0, 0, 0), 1f);
    }

    public void ScreenFadeCall(Color color, float duration)
    {
        CoroutineManager.Instance.StartCoroutineCall(ScreenFadeRoutine(_fade.color, color, duration));
    }

    private IEnumerator ScreenFadeRoutine(Color from, Color to, float duration)
    {
        float innerTimer = 0f;
        while (innerTimer < duration)
        {
            _fade.color = Color.Lerp(from, to, innerTimer / duration);
            innerTimer += Time.deltaTime;
            yield return null;
        }
    }
}