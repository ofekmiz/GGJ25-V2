using UnityEngine;

public abstract class EffectItem : MonoBehaviour
{
    public abstract EffectArgs EffectArgs { get; protected set; }
}
