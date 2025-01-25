using Domains.Core;

public class GoodBullet : Bullet
{
    protected override void OnHitModifier(ModifierBubble modifierBubble)
    {
        AudioManager.Instance.PlayGameAudio("CollectBubblePop");
        GameManager.Instance.GameModifierCollected(modifierBubble.Modifier);
        Destroy(modifierBubble.gameObject);
    }
}
