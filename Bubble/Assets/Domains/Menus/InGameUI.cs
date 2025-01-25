using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private Button _ftueButton;

    [SerializeField] private FTUEScreen _ftueScreen;

    private void Awake()
    {
        _ftueButton.onClick.AddListener(()=>_ftueScreen.Show(null));
    }
}
