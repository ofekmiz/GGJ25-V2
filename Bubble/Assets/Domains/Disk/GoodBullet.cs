using Domains.Core;

public class GoodBullet : Bullet
{
    protected override void OnHitModifier(ModifierBubble modifierBubble)
    {
        GameManager.Instance.GameModifierCollected(modifierBubble.ModifierType);
        Destroy(modifierBubble.gameObject);
    }
}
