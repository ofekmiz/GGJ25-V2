using System;
using System.Collections;
using System.Collections.Generic;
using Domains.Core;
using Unity.VisualScripting;
using UnityEngine;

public class GoodBullet : Bullet
{
    protected override void OnHitModifier(ModifierBubble modifierBubble)
    {
        GameManager.Instance.GameModifierCollected(modifierBubble.ModifierType);
        Destroy(modifierBubble.gameObject);
    }
}
