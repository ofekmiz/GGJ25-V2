using Domains.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [SerializeField]
    private Button _startBtn;

    [SerializeField]
    private GameManager _gm;

    private void Awake()
    {
        _startBtn.onClick.AddListener(OnStartClicked);
    }

    private void OnStartClicked()
    {
        gameObject.SetActive(false);
        _gm.Init();
    }

    private void OnDestroy()
    {
        _startBtn.onClick.RemoveAllListeners();
    }
}
