using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldUIController : MonoBehaviour
{
    public static FieldUIController Instance;
    private Canvas _canvas;

    private void Awake()
    {
        if(Instance != null) Destroy(gameObject);
        else Instance = this;

        _canvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        _canvas.worldCamera = Camera.main;
    }
}
