using System.Collections;
using System.Collections.Generic;
using Proto;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class RadialFX : MonoBehaviour, IPooledObject 
{
    protected string _name;
    [SerializeField] private Image startAngle;
    [SerializeField] private Image endAngle;
    [SerializeField] private Image subEndAngle;

    public string Name
    {
        get => _name; 
        set => _name = value;
    }

    public void Initialize(float from, float to, Color color, float scale = 1f, float subto = 0f)
    {
        transform.parent = FieldUIController.Instance.transform;
        transform.localScale = Vector3.one * (scale * 2);
        startAngle.fillAmount = from;
        endAngle.fillAmount = to;
        subEndAngle.fillAmount = subto;
        startAngle.color = color;
    }
    
    public void Dispose()
    {
        ObjectPoolController.Self.Dispose(this);
    }
}
