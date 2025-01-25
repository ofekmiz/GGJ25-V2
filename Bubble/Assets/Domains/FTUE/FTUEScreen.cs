using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FTUEScreen : MonoBehaviour
{
    [SerializeField] private List<RectTransform> _ftuePages;
    [SerializeField] private float _transitionDuration = 0.5f;
    [SerializeField] private Ease _easeType = Ease.InOutCubic;
    
    [SerializeField] private CanvasScaler _canvasScaler;
    private int _currentPageIndex = 0;

    private Action _closeCb;

    public void Show(Action closeCb)
    {
        gameObject.SetActive(true);
        moveToFirstPageImmediate();
        Time.timeScale = 0;
        _closeCb = closeCb;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
        _closeCb?.Invoke();
    }

    private void Update()
    {
        if (Input.anyKeyDown)
            slideToNextPage();
    }

    private void moveToFirstPageImmediate()
    {
        _currentPageIndex = 0;

        for (int i = 0; i < _ftuePages.Count; i++)
        {
            _ftuePages[i].anchoredPosition = i == 0 ? Vector2.zero : new Vector2(_canvasScaler.referenceResolution.x, 0);
        }
    }

    private void slideToNextPage()
    {
        if (_currentPageIndex >= _ftuePages.Count - 1)
        {
            Hide();
            return;
        } 
        
        var currentPage = _ftuePages[_currentPageIndex];
        var nextPage = _ftuePages[_currentPageIndex + 1];

        currentPage.DOAnchorPos(new Vector2(-_canvasScaler.referenceResolution.x, 0), _transitionDuration).SetEase(_easeType).SetUpdate(true);

        nextPage.DOAnchorPos(Vector2.zero, _transitionDuration).SetEase(_easeType).SetUpdate(true);

        _currentPageIndex++;
    }
}
