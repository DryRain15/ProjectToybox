using System.Collections;
using System.Collections.Generic;
using System.Text;
using Proto;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubbleFX : PooledFX
{
    [SerializeField] private string originalText;
    [SerializeField] private string currentText;
    [SerializeField] private Text TextBox;
    [SerializeField] private Image Bubble;

    private StringBuilder _sb = new StringBuilder(); 
    
    private float _innerTimer;

    private int _progress;
    private float _speed;
    private float _lifeTime;
    private bool _skip;
    
    private const float TypeSpeed = 0.05f;
    
    public override void Update()
    {
        if (_progress < originalText.Length)
        {
            if (_innerTimer * _speed > TypeSpeed)
            {
                _innerTimer = 0f;
                _progress++;
            }

            if (_skip && GlobalInputController.Instance.useKeyDown)
            {
                _progress = originalText.Length - 1;
            }
                
        }

        currentText = _sb.ToString(0, _progress);
        TextBox.text = currentText;
        
        if (_innerTimer > _duration)
        {
            _innerTimer = 0f;
            _duration = 9999f;
            Dispose();
        }
        _innerTimer += Time.deltaTime;
    }

    public void Initialize(string text, bool skip, float speed, float lifetime)
    {
        transform.parent = ScreenUIController.Instance.transform;
        originalText = text;
        currentText = "";
        _sb.Append(originalText);
        _duration = lifetime;
        _skip = skip;
        _speed = speed;
        _progress = 0;
    }
}
