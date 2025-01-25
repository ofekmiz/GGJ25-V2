using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DestroyingBullet : Bullet
{
    protected override void OnHitModifier(ModifierBubble modifierBubble)
    {
        Destroy(modifierBubble.gameObject);
    }
}
