using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameModifier
{
    public GameModifierType Type;
    public Sprite Icon;
}

public enum GameModifierType
{
    FireExtinguisher,
    Goggles,
    Hat,
    Jump,
    Pistol,
    RubberFloat,
    Sword,
    Random
}