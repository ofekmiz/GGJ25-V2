using Domains.Core;
using UnityEngine;

namespace Domains.Bubbles.BubbleEntity
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Bubble : MonoBehaviour, IConstructed
    {
        public void Construct(in Dependencies deps) { }

        public float Radius => Collider.radius;
        public Rigidbody2D Rigidbody { get; private set; }
        public CircleCollider2D Collider { get; private set; }
        public Animator Animator { get; private set; }
        public float SecondsAlive { get; set; }


        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            Collider = GetComponent<CircleCollider2D>();
            Animator = GetComponent<Animator>();
        }

        private void Start()
        {
            Rigidbody.gravityScale = 0;
        }
    }
}
