using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    public static CoroutineManager Instance;

    public Dictionary<IEnumerator, Coroutine> Coroutines;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            Coroutines = new Dictionary<IEnumerator, Coroutine>();
        }
    }

    private void FixedUpdate()
    {
        Debug.Log(string.Format($"{Coroutines.Count} running coroutine"));
    }

    public Coroutine StartCoroutineCall(IEnumerator coroutine)
    {
        return StartCoroutine(StartCoroutineCallRoutine(coroutine));
    }

    private IEnumerator StartCoroutineCallRoutine(IEnumerator coroutine)
    {
        var routine = StartCoroutine(coroutine);
        Coroutines.Add(coroutine, routine);
        yield return routine;
        Coroutines.Remove(coroutine);
    }

    public void StopCoroutineCall(IEnumerator coroutine)
    {
        if (Coroutines.Remove(coroutine))
            StopCoroutine(coroutine);
    }
}
