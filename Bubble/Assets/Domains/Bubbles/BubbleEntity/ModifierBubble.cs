using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierBubble : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _icon;
    
    public GameModifier Modifier {get; private set;}
    
    public void Set(GameModifier gameModifier)
    {
        Modifier = gameModifier;
        _icon.sprite = gameModifier.Icon;
    }
}
