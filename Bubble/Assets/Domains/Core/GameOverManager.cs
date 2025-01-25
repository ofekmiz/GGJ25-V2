using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _skySprite;
    [SerializeField] private SpriteRenderer _groundSprite;

    [SerializeField] private GameOverScreen _gameOverScreen;

    private void Awake()
    {
        //OnGameOver();
    }

    public void OnGameOver(int score)
    {
        //_skySprite.material.DOColor(Color.grey, 5f);
        _gameOverScreen.SetScore(score);
    }
}
