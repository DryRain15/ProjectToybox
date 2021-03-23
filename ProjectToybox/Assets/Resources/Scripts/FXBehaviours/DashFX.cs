using System.Collections;
using System.Collections.Generic;
using Proto;
using UnityEngine;

public class DashFX : PooledFX
{
    private float _innerTimer;
    private SpriteRenderer _sr;

    public override void Update()
    {
        if (_innerTimer < _duration)
        {
            _innerTimer += Time.deltaTime;
            _sr.color = new Color(0f, 0.9f, 1f, 1 - (_innerTimer / _duration));
        }
        else
        {
            _sr.color = new Color(1f, 1f, 1f, 0f);
            _sr.flipX = false;
            _innerTimer = 0f;
            _duration = 9999f;
            Dispose();
        }
    }

    public override void Dispose()
    {
        _sr.sprite = null;
        ObjectPoolController.Self.Dispose(this);
    }
        
    public override void Initialize(float duration)
    {
        transform.position += Vector3.forward * 1.85f;
        _sr = GetComponent<SpriteRenderer>();
        _sr.sprite = PlayerBehaviour.Instance.GetCurrentSprite();
        _sr.flipX = PlayerBehaviour.Instance.IsFlipped();
        _sr.color = new Color(0f, 0.9f, 1f, 1f);
        _duration = duration;
        _innerTimer = 0f;
    }
}
