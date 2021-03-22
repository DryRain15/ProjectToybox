using System;
using System.Collections;
using System.Collections.Generic;
using Proto;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "defaultStats", menuName = "DataSet/CharacterStats", order = 1)]
public class Stats : ScriptableObject
{
    [SerializeField]
    public float hp = 10f;
    [SerializeField]
    public float hpGen = 0.5f;
    [SerializeField]
    public float atk = 3f;
    [SerializeField]
    public float fever = 1.3f;
    [SerializeField]
    public float moveSpeed = 2f;
}