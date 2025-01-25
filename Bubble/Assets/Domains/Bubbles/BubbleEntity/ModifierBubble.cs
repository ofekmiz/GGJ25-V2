using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierBubble : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _icon;
    public void Set(GameModifier gameModifier)
    {
        _icon.sprite = gameModifier.Icon;
    }
}
