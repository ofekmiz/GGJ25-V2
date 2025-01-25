using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSheeter : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite[] _sprites;
    [SerializeField] private float _changeTime = 0.5f;

    private float _timeToNextChange;
    private int _currSpriteIdx = 0;
    
    private void Awake()
    {
        _spriteRenderer.sprite = _sprites[0];
        _timeToNextChange = _changeTime;
    }

    private void Update()
    {
        _timeToNextChange -= Time.deltaTime;
        if (_timeToNextChange < 0)
        {
            _timeToNextChange = _changeTime;
            _currSpriteIdx = (_currSpriteIdx + 1) % _sprites.Length;
            _spriteRenderer.sprite = _sprites[_currSpriteIdx];
        }
    }
}
