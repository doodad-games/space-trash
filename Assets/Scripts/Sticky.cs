using System;
using System.Collections;
using System.Collections.Generic;
using MyLibrary;
using UnityEngine;

public class Sticky : MonoBehaviour
{
    const float POST_DISCONNECTION_X_VELOCITY_STEP = 0.01f;
    const float CHAIN_REACTION_STEP_DELAY = 0.25f;

    public event Action onDestroyed;
    public event Action onChildrenChanged;

    public bool HasChildren => _directChildren.Count != 0;
    public int NumDescendents => _numDescendents;
    public int NumTNTs => _numTNTs;

    [HideInInspector] public float torque;

#pragma warning disable CS0649
    [SerializeField] AudioSource _stickSound;
#pragma warning restore CS0649

    int _id;
    bool _isPlayer;
    bool _isTNT;
    Rigidbody2D _rb;

    int _numDescendents;
    int _numTNTs;
    bool _isInChainReaction;
    Sticky _root;
    Sticky _parent;
    List<Sticky> _directChildren;
    bool _destroyed;

    void Awake()
    {
        _isPlayer = GetComponent<Player>() != null;
        _rb = GetComponent<Rigidbody2D>();

        var tnt = GetComponent<TNT>();
        _isTNT = tnt != null;
    }
    
    void OnEnable()
    {
        _root = this;
        _parent = null;
        _isInChainReaction = _isPlayer || _isTNT;
        _directChildren = new List<Sticky>();
        _id = GameConfig.NewID;
        _numDescendents = _numTNTs = 0;
        _destroyed = false;
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
            HandleStickyCollision(otherSticky, collision.contacts[0].point);
            return;
        }

        if (
            _isPlayer ||
            _root.gameObject == Player.I.gameObject
        )
        {
            var deathTrash = collision.collider.GetComponent<DeathTrash>();
            if (deathTrash != null)
            {
                DestroyFromCollision(collision, false);
                return;
            }
        }

        var bullet = collision.collider.GetComponent<TurretBullet>();
        if (
            bullet != null &&
            bullet.Owner != gameObject
        )
        {
            DestroyFromCollision(collision, true);
            return;
        }
    }

    void UpdatePostDisconnectionVelocity()
    {
        if (
            _isPlayer ||
            _parent != null ||
            _rb.velocity.x <= 0
        ) return;

        _rb.velocity = new Vector2(
            _rb.velocity.x - POST_DISCONNECTION_X_VELOCITY_STEP,
            _rb.velocity.y
        );
    }

    void HandleStickyCollision(Sticky otherSticky, Vector3 pos)
    {
        var otherRoot = otherSticky._root;

        // Abort for same cluster
        if (otherRoot == _root)
            return;
        
        // Abort if lower priority
        {
            // Player is always priority
            if (otherRoot._isPlayer)
                return;
            if (!_root._isPlayer)
            {
                // Faster or older takes is priority
                var myVel = _root._rb.velocity.sqrMagnitude;
                var otherVel = otherRoot._rb.velocity.sqrMagnitude;

                if (
                    myVel < otherVel ||
                    (
                        myVel == otherVel &&
                        _id > otherRoot._id
                    )
                ) return;
            }
        }

        SpawnStickVFX(pos);

        if (otherSticky._stickSound.isActiveAndEnabled)
            otherSticky._stickSound.Play();

        // Remove other cluster's velocity
        var otherRB = otherRoot.GetComponent<Rigidbody2D>();
        otherRB.velocity = Vector2.zero;
        otherRoot.torque = 0;

        var visitedSet = new HashSet<Sticky>();
        visitedSet.Add(this);
        ReTopo(visitedSet, otherSticky);
    }

    void DestroyFromCollision(Collision2D collision, bool wasShot) =>
        DoTheBoom(collision.contacts[0].point, wasShot);
    
    void DoTheBoom(Vector3 boomPoint, bool wasShot)
    {
        if (_destroyed)
            return;
        _destroyed = true;

        SpawnBoomFX(boomPoint, wasShot);
        
        onDestroyed?.Invoke();

        if (_isPlayer)
            return;

        var numDescendents = 0;
        var numTNTs = 0;

        foreach (var child in _directChildren)
        {
            if (child == null)
                continue;

            var (childrenUpdated, tntsUpdated) = child.RecursivelyUpdateRoot(child);
            numDescendents += childrenUpdated;
            numTNTs += tntsUpdated;

            child.transform.parent = null;
            child.torque = _root.torque;

            var otherRB = child.GetComponent<Rigidbody2D>();
            otherRB.velocity = _root._rb.velocity;

            if (_isInChainReaction)
                child.MarkForChainReaction();
        }

        _root._numDescendents -= numDescendents;
        _root._numTNTs -= numTNTs;
        
        if (_parent != null)
        {
            _parent.RemoveChild(this);

            if (_isInChainReaction)
                _parent.MarkForChainReaction();
        }

        Destroy(gameObject);
    }

    void MarkForChainReaction() =>
        StartCoroutine(MarkForChainReactionCo());
    IEnumerator MarkForChainReactionCo()
    {
        _isInChainReaction = true;
        yield return new WaitForSeconds(CHAIN_REACTION_STEP_DELAY);
        DoTheBoom(transform.position, false);
    }

    void ReTopo(HashSet<Sticky> visitedSet, Sticky newChild)
    {
        if (
            newChild == null ||
            visitedSet.Contains(newChild)
        ) return;
        visitedSet.Add(newChild);

        var toVisit = new HashSet<Sticky>(newChild._directChildren);
        if (newChild._parent != null)
            toVisit.Add(newChild._parent);

        newChild.ClearChildren();
        newChild._root = _root;

        newChild._parent = this;
        AddChild(newChild);

        newChild.transform.parent = transform;

        foreach (var child in toVisit)
            newChild.ReTopo(visitedSet, child);
    }

    (int, int) RecursivelyUpdateRoot(Sticky newRoot)
    {
        var numDescendents = 1;
        var numTNTs = 0;
        if (_isTNT)
            numTNTs += 1;

        _root = newRoot;
        foreach (var child in _directChildren)
        {
            var (childDescendents, childTNTs) = child.RecursivelyUpdateRoot(newRoot);

            numDescendents += childDescendents;
            numTNTs += childTNTs;
        }

        return (numDescendents, numTNTs);
    }

    void SpawnStickVFX(Vector3 stickPoint) =>
        Visuals.Spawn(stickPoint, "EffectVisualsThud");

    void SpawnBoomFX(Vector3 boomPoint, bool wasShot)
    {
        var prependExtraShotVisual = false;
        int numVisuals;
        string vfxBucket, soundResource;
        if (_isPlayer)
        {
            vfxBucket = soundResource = "Player";
            numVisuals = UnityEngine.Random.Range(2, 5);
        }
        else if (_isTNT)
        {
            vfxBucket = soundResource = "TNT";

            if (
                _root._isPlayer ||
                _parent != null ||
                _directChildren.Count != 0
            ) numVisuals = 1;
            else
                numVisuals = UnityEngine.Random.value > 0.4f ? 1 : 0;

            if (wasShot)
                prependExtraShotVisual = true;
        }
        else
        {
            vfxBucket = wasShot ? "Shot" : "Sticky";
            soundResource = "Sticky";
            numVisuals = 1;
        }

        soundResource = $"Effects/BoomSound{soundResource}";
        var soundPrefab = Resources.Load<GameObject>(soundResource);
        Instantiate(soundPrefab, boomPoint, Quaternion.identity);

        if (numVisuals == 0)
            return;
        
        vfxBucket = $"EffectVisuals{vfxBucket}";
        Visuals.Spawn(boomPoint, vfxBucket, numVisuals, prependExtraShotVisual);
    }

    void AddChild(Sticky child)
    {
        ++_root._numDescendents;
        if (child._isTNT)
            ++_root._numTNTs;
        
        _directChildren.Add(child);
        _root.onChildrenChanged?.Invoke();
    }

    void RemoveChild(Sticky child)
    {
        --_root._numDescendents;
        if (child._isTNT)
            --_root._numTNTs;

        _directChildren.Remove(child);
        _root.onChildrenChanged?.Invoke();
    }

    void ClearChildren()
    {
        foreach (var child in _directChildren)
        {
            --_root._numDescendents;
            if (child._isTNT)
                --_root._numTNTs;
        }

        _directChildren.Clear();
        _root.onChildrenChanged?.Invoke();
    }
}
