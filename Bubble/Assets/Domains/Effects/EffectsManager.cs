using Domains.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager
{
    private static Dictionary<string,IEffectable> _effectables = new();

    public static void Subscribe(string key, IEffectable effectable)
    {
        _effectables.Add(key, effectable);
    }
    
    public static void Unsubscribe(string key)
    {
        _effectables.Remove(key);
    }

    public void PlayEffect(EffectArgs args)
    {
        if (!_effectables.TryGetValue(args.Type, out var effectable))
        {
            Debug.LogError($"Tried to play effect {args.Type} but it doesn't exist");
            return;
        }
        effectable.ApplyEffect(args);
    }
    
    public void DisableEffect(string effectType)
    {
        if (!_effectables.TryGetValue(effectType, out var effectable))
        {
            Debug.LogError($"Tried to disable effect {effectType} but it doesn't exist");
            return;
        }
        
        effectable.DisableEffect();
    }
}
