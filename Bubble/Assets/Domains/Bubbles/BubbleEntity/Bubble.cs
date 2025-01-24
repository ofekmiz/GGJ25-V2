using System;
using Domains.Core;
using UnityEngine;

namespace Domains.Bubbles.BubbleEntity
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bubble : MonoBehaviour, IConstructed
    {
        public void Construct(in Dependencies deps) { }

        public Rigidbody2D Rigidbody { get; private set; }
        public float SecondsAlive { get; set; }


        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            Rigidbody.gravityScale = 0;
        }
    }
}
