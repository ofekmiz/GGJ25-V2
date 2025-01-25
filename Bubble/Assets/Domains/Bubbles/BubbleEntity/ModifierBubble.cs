using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierBubble : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _icon;
    
    [SerializeField] private List<Sprite> _sprites;
    [SerializeField] private List<Sprite> _badSprites;
    [SerializeField] private SpriteRenderer _bubbleSprite;
    
    private GameModifier _modifier;
    
    public void Set(GameModifier gameModifier)
    {
        _modifier = gameModifier;
        _icon.sprite = gameModifier.Icon;
        
        var randSprite = gameModifier.Good ? _sprites[Random.Range(0, _sprites.Count)] : _badSprites[Random.Range(0, _badSprites.Count)];
        _bubbleSprite.sprite = randSprite;
    }
    
    public GameModifierType ModifierType => _modifier.Type;
}
