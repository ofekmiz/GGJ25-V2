using Domains.Core;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [SerializeField]
    private Button _startBtn;

    [SerializeField]
    private GameManager _gm;

    [SerializeField] private CanvasGroup _canvasGroup;
    
    [SerializeField] private float _fadeDuration = 1;
    
    [SerializeField] private Ease _fadeEase = Ease.InOutQuad;
    
    [SerializeField] FTUEScreen _ftueScreen;

    private void Awake()
    {
        _startBtn.onClick.AddListener(OnStartClicked);
    }

    private void OnStartClicked()
    {
        _ftueScreen.Show(transitionToGameScene);
    }

    private void transitionToGameScene()
    {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Additive);
        _canvasGroup.DOFade(0, _fadeDuration).SetEase(_fadeEase).OnComplete(() =>
        {
            SceneManager.UnloadSceneAsync("StartMenu");
        });
    }

    private void OnDestroy()
    {
        _startBtn.onClick.RemoveAllListeners();
    }
}
