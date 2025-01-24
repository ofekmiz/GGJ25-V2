using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [SerializeField]
    private Button _startBtn;

    private void Awake()
    {
        _startBtn.onClick.AddListener(OnStartClicked);
    }

    private void OnStartClicked()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void OnDestroy()
    {
        _startBtn.onClick.RemoveAllListeners();
    }
}
