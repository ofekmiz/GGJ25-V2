using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameModifier
{
    public ModifierType Type;
    public Sprite Icon;
}

public enum ModifierType
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