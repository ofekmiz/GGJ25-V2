using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierBubble : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _icon;
    
    private GameModifier _modifier;
    
    public void Set(GameModifier gameModifier)
    {
        _modifier = gameModifier;
        _icon.sprite = gameModifier.Icon;
    }
    
    public GameModifierType ModifierType => _modifier.Type;
}
