using Domains.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager
{
    private GameManager _gm;

    private List<PlatformerItem> _platformerItems;

    public void ApplyEffect(Effect e, EffectTarget target)
    {
        
    }

    public EffectsManager(in Dependencies d)
    {
        _platformerItems = d.PlatformerItems;
        Debug.Log($"Effects Manager created with pItems = {_platformerItems}");
    }
}
