using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BubblePhysics : MonoBehaviour
{
    private readonly List<BubblePhysics> _connected = new();

    private readonly List<SpringJoint2D> _springs = new();

    private Rigidbody2D Rb { get; set; }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Rb.velocity = new(Random.Range(-2, 2), Random.Range(-2, 2));
        transform.position += new Vector3(0.1f, 0.1f, 0.1f);
    }

    private void FixedUpdate()
    {
        AccelerateBubble(Time.fixedDeltaTime);
        ApplyCenterForce();
    }

    private bool IsConnectedTo(BubblePhysics bubble) => _connected.Contains(bubble) || bubble.Connected.Contains(this);

    private SpringJoint2D GetSpringConnectedTo(BubblePhysics bubble)
    {
        foreach (var spring in _springs)
        {
            if (spring.connectedBody == bubble.Rb)
                return spring;
        }
        return null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var otherBubble = other.gameObject.GetComponent<BubblePhysics>();

        if (otherBubble == null) return;

        if (IsConnectedTo(otherBubble) || _connected.Count > 0)
            return;

        var thisSpring = gameObject.AddComponent<SpringJoint2D>();
        thisSpring.connectedBody = other.gameObject.GetComponent<Rigidbody2D>();
        thisSpring.autoConfigureDistance = false;
        thisSpring.distance = 3f;
        thisSpring.frequency = 0.5f;
        _springs.Add(thisSpring);
        _connected.Add(otherBubble);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var otherBubble = other.gameObject.GetComponent<BubblePhysics>();
        
        if (otherBubble == null) return;

        if (!IsConnectedTo(otherBubble))
            return;

        var springConnected = GetSpringConnectedTo(otherBubble);
        if (springConnected == null)
            return;

        _springs.Remove(springConnected);
        Destroy(springConnected);
        _connected.Remove(otherBubble);
    }

    private void AccelerateBubble(float dt)
    {
        const float targetSpeed = 4f;
        const float acc = 0.5f;

        var vel = Rb.velocity.magnitude;

        if (vel < targetSpeed)
            Rb.velocity += Rb.velocity.normalized * (dt * acc);
    }

    private void ApplyCenterForce()
    {
        const float acc = 2f;

        var center = BubbleSpawner.Instance.SpawningArea.bounds.center;
        var distToCenter = (center - transform.position);
        Rb.AddForce(distToCenter * acc, ForceMode2D.Force);

    }

    private List<BubblePhysics> Connected => _connected;
}
