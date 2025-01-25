using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _skySprite;
    [SerializeField] private SpriteRenderer _groundSprite;

    private void Awake()
    {
        OnGameOver();
    }

    public void OnGameOver()
    {
        _skySprite.material.DOColor(Color.grey, 5f);
    }
}
