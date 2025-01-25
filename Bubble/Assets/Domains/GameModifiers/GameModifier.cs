using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameModifier
{
    public GameModifierType Type;
    public Sprite Icon;
    public bool Good = true;
    public GameObject Prefab;
}

public enum GameModifierType
{
    Goggles,
    Hat,
    Jump,
    JetPack,
    BreakablePlatforms,
    ShortPlatforms,
    LongPlatforms,
    Enemy,
    Shield,
    Slow,
    Background,
    Random
}
