using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierBubble : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _icon;
    
    [SerializeField] private List<Sprite> _sprites;
    [SerializeField] private List<Sprite> _badSprites;
    [SerializeField] private SpriteRenderer _bubbleSprite;
    public GameModifier Modifier {get; private set;}
    
    public void Set(GameModifier gameModifier)
    {
        Modifier = gameModifier;
        _icon.sprite = gameModifier.Icon;
        
        var randSprite = gameModifier.Good ? _sprites[Random.Range(0, _sprites.Count)] : _badSprites[Random.Range(0, _badSprites.Count)];
        _bubbleSprite.sprite = randSprite;
    }
}
