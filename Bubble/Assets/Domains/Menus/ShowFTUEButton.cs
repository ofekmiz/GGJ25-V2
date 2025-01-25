using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShowFTUEButton : MonoBehaviour
{
    private Button _btn;

    private void Awake()
    {
        _btn = GetComponent<Button>();
        _btn.onClick.AddListener(BackToMenu);
    }

    private void BackToMenu()
    {
        
    }


    private void OnDisable()
    {
        _btn.onClick.RemoveAllListeners();
    }
}
