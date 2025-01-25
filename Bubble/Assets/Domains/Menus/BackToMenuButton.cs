using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackToMenuButton : MonoBehaviour
{
    private Button _btn;

    private void Awake()
    {
        _btn = GetComponent<Button>();
        _btn.onClick.AddListener(BackToMenu);
    }

    private void BackToMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }


    private void OnDisable()
    {
        _btn.onClick.RemoveAllListeners();
    }
}
