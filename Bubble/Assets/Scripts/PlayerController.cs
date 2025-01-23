using Domains.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    public event Action PlayerDead;

    public event Action<Effect> PlayerEffect;

    [SerializeField]
    private GameManager _gameOverManager;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hazard"))
        {
            PlayerDead?.Invoke();
        }
        else if (collision.gameObject.CompareTag("Effect"))
        {
            Effect effectToTake;
            PlayerEffect.Invoke(effectToTake);
        }
        else if (collision.gameObject.CompareTag("Blocker"))
        {
            // TODO
        }
    }
}
