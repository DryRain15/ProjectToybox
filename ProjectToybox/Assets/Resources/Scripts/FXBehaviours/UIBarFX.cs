using System.Collections;
using System.Collections.Generic;
using Proto;
using UnityEngine;
using UnityEngine.UI;

public class UIBarFX : PooledFX
{
    private Transform follow;
    private float yOffset;
    [SerializeField] private Image bg;
    [SerializeField] private Image fever;
    [SerializeField] private Image hp;

    private float _innerTimer;

    public override void Update()
    {
        if (_innerTimer < _duration)
        {
            transform.position = follow.position + Vector3.up * yOffset;
            _innerTimer += Time.deltaTime;
            bg.color = new Color(1f, 1f, 1f, 1 - (_innerTimer / _duration));
            fever.color = new Color(1f, 1f, 1f, 1 - (_innerTimer / _duration));
            hp.color = new Color(1f, 1f, 1f, 1 - (_innerTimer / _duration));
        }
        else
        {
            _innerTimer = 0f;
            _duration = 9999f;
            Dispose();
        }
    }

    public void Initialize(Transform follower, float ifever, float ihp, float duration, float y_offset = 0.75f)
    {
        follow = follower;
        transform.parent = ScreenUIController.Instance.transform;
        bg.fillAmount = ifever;
        hp.fillAmount = ihp;
        _duration = duration;
        _innerTimer = -_duration;
        yOffset = y_offset;
    }
    
    public void Dispose()
    {
        follow = null;
        ObjectPoolController.Self.Dispose(this);
    }
}
