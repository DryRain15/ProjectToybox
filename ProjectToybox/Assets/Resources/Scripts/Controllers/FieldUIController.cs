using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldUIController : MonoBehaviour
{
    public static FieldUIController Instance;

    private void Awake()
    {
        Instance = this;
    }
}
