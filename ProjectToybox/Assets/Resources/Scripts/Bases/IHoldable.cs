using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHoldable
{
    void Hold();
    void Use();
    void Release();

    void HoldableUpdate();
}
