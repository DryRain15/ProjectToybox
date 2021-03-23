using System.Collections;
using System.Collections.Generic;
using Proto;
using UnityEngine;
using UnityEngine.UI;

public class DamageTextFX : PooledFX
{
    [SerializeField] private Text txt;
    [SerializeField] private Image mark;

    private float _innerTimer;
    private bool _guard;

    private Vector2 _velocity;
    private const float Gravity = 8f;

    public override void Update()
    {
        if (_innerTimer < _duration)
        {
            transform.position += (Vector3)_velocity * Time.deltaTime;
            _innerTimer += Time.deltaTime;
            txt.color = new Color(_guard ? 0f : 1f, 0f, 0f, 1 - (_innerTimer / _duration));
            mark.color = new Color(1f, 1f, 1f, 1 - (_innerTimer / _duration));
            _velocity.y -= Gravity * Time.deltaTime;
        }
        else
        {
            _innerTimer = 0f;
            _duration = 9999f;
            Dispose();
        }
    }

    public void Initialize(string text, bool isMark, float duration)
    {
        transform.parent = ScreenUIController.Instance.transform;
        txt.text = text;
        mark.fillAmount = isMark ? 1f : 0f;
        _guard = isMark;
        _duration = duration;
        _innerTimer = -_duration;
        _velocity = new Vector2((Random.value - 0.5f) * 3f, 4f);
    }
}
