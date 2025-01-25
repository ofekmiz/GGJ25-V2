using Domains.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestartButton : MonoBehaviour
{
    private Button _btn;

    private void Awake()
    {
        _btn = GetComponent<Button>();
        _btn.onClick.AddListener(onClick);
    }

    private void onClick()
    {
        GameManager.Instance.RestartGame();
    }

    private void OnDisable()
    {
        _btn.onClick?.RemoveListener(onClick);
    }
}
