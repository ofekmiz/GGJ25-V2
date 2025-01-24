using Domains.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour, IConstructed
{
    private GameOverManager _gameOverManager;
    private EffectsManager _effects;    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hazard"))
        {
            _gameOverManager.OnGameOver();
        }
        else if (collision.gameObject.CompareTag("Effect"))
        {
            Effect effectToTake = new Effect();
        }
        else if (collision.gameObject.CompareTag("Blocker"))
        {
            // TODO
        }
    }

    public void Construct(in Dependencies d)
    {
        _gameOverManager = d.gameOverManager;
    }
}
