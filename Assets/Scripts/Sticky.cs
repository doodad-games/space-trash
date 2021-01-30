using System;
using System.Collections.Generic;
using UnityEngine;

public class Sticky : MonoBehaviour
{
    const float POST_DISCONNECTION_X_VELOCITY_STEP = 0.01f;

    public event Action onDestroyed;

    [HideInInspector] public float torque;

#pragma warning disable CS0649
    [SerializeField] AudioSource _stickSound;
#pragma warning restore CS0649

    int _id;
    Player _player;
    Rigidbody2D _rb;

    Sticky _root;
    Sticky _parent;
    List<Sticky> _directChildren;
    bool _destroyed;

    void Awake()
    {
        _player = GetComponent<Player>();
        _rb = GetComponent<Rigidbody2D>();
    }
    
    void OnEnable()
    {
        _root = this;
        _directChildren = new List<Sticky>();
        _id = GameConfig.NewID;
    }
    
    void FixedUpdate()
    {
        UpdatePostDisconnectionVelocity();
        _rb.rotation += torque;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        var otherSticky = collision.collider.GetComponent<Sticky>();
        if (otherSticky != null)
        {
            HandleStickyCollision(otherSticky);
            return;
        }

        var deathTrash = collision.collider.GetComponent<DeathTrash>();
        if (deathTrash != null)
        {
            DestroyFromCollision(collision);
            return;
        }

        var bullet = collision.collider.GetComponent<TurretBullet>();
        if (
            bullet != null &&
            bullet.Owner != gameObject
        )
        {
            DestroyFromCollision(collision);
            return;
        }
    }

    void UpdatePostDisconnectionVelocity()
    {
        if (
            _player != null ||
            _parent != null ||
            _rb.velocity.x <= 0
        ) return;

        _rb.velocity = new Vector2(
            _rb.velocity.x - POST_DISCONNECTION_X_VELOCITY_STEP,
            _rb.velocity.y
        );
    }

    void HandleStickyCollision(Sticky otherSticky)
    {
        if (otherSticky._id < _id)
            return;

        _directChildren.Add(otherSticky);
        otherSticky._root = _root;
        otherSticky._parent = this;
        otherSticky.transform.parent = transform;
        otherSticky._stickSound.Play();
        
        var otherRB = otherSticky.GetComponent<Rigidbody2D>();
        otherRB.velocity = Vector2.zero;
        otherSticky.torque = 0;
    }

    void DestroyFromCollision(Collision2D collision)
    {
        if (_destroyed)
            return;
        _destroyed = true;

        var boomPrefab = Resources.Load<GameObject>("Effects/Boom1");
        var effectPoint = collision.contacts[0].point;
        Instantiate(boomPrefab, effectPoint, Quaternion.identity);
        
        onDestroyed?.Invoke();

        if (_player != null)
            return;
        
        if (_parent != null)
            _parent._directChildren.Remove(this);

        foreach (var child in _directChildren)
        {
            child._root = child;
            child.transform.parent = null;
            child.torque = _root.torque;

            var otherRB = child.GetComponent<Rigidbody2D>();
            otherRB.velocity = _root._rb.velocity;
        }

        Destroy(gameObject);
    }
}
