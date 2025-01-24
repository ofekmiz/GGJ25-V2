using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BubblePhysics : MonoBehaviour
{
    private List<BubblePhysics> _connected = new();

    private List<SpringJoint2D> _springs = new();

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.velocity = new Vector2(Random.Range(-2, 2), Random.Range(-2, 2));
        transform.position += new Vector3(0.1f, 0.1f, 0.1f);
    }

    private void FixedUpdate()
    {
        accelerateBubble(Time.fixedDeltaTime);
        applyCenterForce();
    }

    public bool IsConnectedTo(BubblePhysics bubble)
    {
        if (_connected.Contains(bubble) || bubble.Connected.Contains(this))
        {
            return true;
        }
        return false;
    }

    private SpringJoint2D GetSpringConnectedTo(BubblePhysics bubble)
    {
        foreach (var spring in _springs)
        {
            if (spring.connectedBody == bubble.Rb)
            {
                return spring;
            }
        }
        return null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var otherBubble = other.gameObject.GetComponent<BubblePhysics>();

        if (otherBubble == null) return;

        if (!IsConnectedTo(otherBubble) && _connected.Count <= 0)
        {
            var thisSpring = gameObject.AddComponent<SpringJoint2D>();
            thisSpring.connectedBody = other.gameObject.GetComponent<Rigidbody2D>();
            thisSpring.autoConfigureDistance = false;
            thisSpring.distance = 3f;
            thisSpring.frequency = 0.5f;
            _springs.Add(thisSpring);
            _connected.Add(otherBubble);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var otherBubble = other.gameObject.GetComponent<BubblePhysics>();
        
        if (otherBubble == null) return;

        if (IsConnectedTo(otherBubble))
        {
            var springConnected = GetSpringConnectedTo(otherBubble);
            if (springConnected != null)
            {
                _springs.Remove(springConnected);
                Destroy(springConnected);
                _connected.Remove(otherBubble);
            }
        }    
    }

    private void accelerateBubble(float dt)
    {
        float targetSpeed = 4f;
        float acc = 0.5f;

        float vel = _rb.velocity.magnitude;
        if (vel < targetSpeed)
        {
            _rb.velocity += _rb.velocity.normalized * dt * acc;
        }
    }

    private void applyCenterForce()
    {
        float acc = 2f;

        var center = BubbleSpawner.Instance.SpawningArea.bounds.center;
        var distToCenter = (center - transform.position);
        _rb.AddForce(distToCenter * acc, ForceMode2D.Force);

    }

    public List<BubblePhysics> Connected => _connected;

    public Rigidbody2D Rb => _rb;
}
