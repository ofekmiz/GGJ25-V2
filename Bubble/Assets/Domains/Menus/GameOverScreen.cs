using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;

    public void SetScore(int score)
    {
        _scoreText.text = score.ToString();
    }
}
