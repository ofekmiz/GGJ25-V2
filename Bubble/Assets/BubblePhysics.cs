using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Jobs;

[RequireComponent(typeof(Collider2D))]
public class BubblePhysics : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;

    [SerializeField] private Color _startColor;
    [SerializeField] private Color _endColor;

    private readonly List<BubblePhysics> _connected = new();

    private readonly List<SpringJoint2D> _springs = new();

    private Rigidbody2D Rb { get; set; }

    private Transform _center;

    private float _radius;

    private float _noiseSpeed = 0.1f;
    private float _noiseFactor = 20f;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Rb.velocity = new(Random.Range(-2, 2), Random.Range(-2, 2));
        transform.position += new Vector3(0.1f, 0.1f, 0.1f);
        setRandomColor();
    }

    public void SetCenter(Transform center, float radius)
    {
        _center = center;
        _radius = radius;
    }

    private void setRandomColor()
    {
        var main = _particleSystem.main;
        main.startColor = Color.Lerp(_startColor, _endColor, Random.Range(0f,1f));
    }

    private void FixedUpdate()
    {
        AccelerateBubble(Time.fixedDeltaTime);
        ApplyCenterForce();
        ApplyNoiseForce();

        if (Vector3.Distance(_center.position, transform.position) > _radius)
        {
            //Destroy(gameObject);
        }
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

        if (!IsConnectedTo(otherBubble) && _connected.Count <= 0)
        {
            var thisSpring = gameObject.AddComponent<SpringJoint2D>();
            thisSpring.connectedBody = other.gameObject.GetComponent<Rigidbody2D>();
            thisSpring.autoConfigureDistance = false;
            thisSpring.distance = 1f;
            thisSpring.frequency = 0.5f;
            _springs.Add(thisSpring);
            _connected.Add(otherBubble);
        }
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

    private void ApplyNoiseForce()
    {
        var noiseX = Mathf.PerlinNoise(transform.position.x + Time.time * _noiseSpeed, transform.position.y + Time.time * _noiseSpeed);
        var noiseY = Mathf.PerlinNoise(transform.position.x + Time.time * 2 * _noiseSpeed, transform.position.y + Time.time * 2 * _noiseSpeed);
        Rb.AddForce(new Vector2(noiseX * _noiseFactor, noiseY * _noiseFactor));
    }

    public List<BubblePhysics> Connected => _connected;
}
