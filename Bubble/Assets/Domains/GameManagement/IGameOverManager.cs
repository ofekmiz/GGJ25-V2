using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameOverManager
{
    event Action<bool> GameOver; // true for win, false for lose
}
