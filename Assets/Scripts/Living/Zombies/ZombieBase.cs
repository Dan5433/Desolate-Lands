using CustomExtensions;
using System.Collections.Generic;
using UnityEngine;

public class ZombieBase : LivingBase
{
    [Header("Base Properties")]
    [SerializeField] State state;
    [SerializeField] protected float baseDamage = 3f;
    [SerializeField] protected float speed = 5f;
    [SerializeField] protected float sightRange = 12f;
    [Header("Property Modifiers")]
    [SerializeField] protected float searchDuration = 10f;
    [SerializeField] protected float searchSpeedMultiplier = 0.85f;
    [SerializeField] protected float attackSightMultiplier = 1.5f;
    [SerializeField] protected float attackSpeedMultiplier = 1.25f;
    [Header("Pathfinding")]
    [SerializeField] protected int crumbLimit = 15;
    [SerializeField] protected float crumbInterval = 10f;
    [SerializeField] protected float crumbRemovalDistance = 0.05f;
    [SerializeField] protected Transform raycastOrigin;
    [Header("Idle Movement")]
    [SerializeField] protected Vector2 idleMoveRadius = new(0.25f,5f);
    [SerializeField] protected Vector2 idleMoveIntervalTime = new(0.5f,4f);

    Rigidbody2D rb;

    Queue<Vector3> targetBreadcrumbs = new();

    Transform target;
    float searchTimer;
    float idleMoveTimer;

    protected override void Start()
    {
        base.Start();
        target = GameManager.Instance.Player.transform;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {

#if UNITY_EDITOR
        if(targetBreadcrumbs.Count > 0)
        {
            foreach (var crumb in targetBreadcrumbs)
                crumb.DrawDiamond(0.25f, Color.cyan, 1f);
        }
#endif
        switch (state)
        {
            case State.Idle:
                Idle(); break;
            case State.Searching:
                CrumbUpdate();
                Search();
                break;
            case State.Attacking:
                CrumbUpdate();
                Attack(); 
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Idle:
                MoveRandomly(); break;
            case State.Searching:
                FollowBreadcrumbs(speed * searchSpeedMultiplier); break;
            case State.Attacking:
                MoveTowardsTarget(target.position, speed * attackSpeedMultiplier); break;
        }
    }

    void MoveRandomly()
    {
        FollowBreadcrumbs(speed);

        if (targetBreadcrumbs.Count > 0)
            return;

        if (idleMoveTimer <= 0)
        {
            Vector3 randomPosition;
            do
            {
                float xPos = Random.Range(0, 2) == 1 ?
                    Random.Range(rb.position.x + idleMoveRadius.x, rb.position.x + idleMoveRadius.y):
                    Random.Range(rb.position.x - idleMoveRadius.x, rb.position.x - idleMoveRadius.y);

                float yPos = Random.Range(0, 2) == 1 ?
                    Random.Range(rb.position.y + idleMoveRadius.x, rb.position.y + idleMoveRadius.y):
                    Random.Range(rb.position.y - idleMoveRadius.x, rb.position.y - idleMoveRadius.y);

                randomPosition = new(xPos,yPos);
            }
            while (!CanReachPoint(randomPosition));

            AddBreadcrumb(randomPosition);
            idleMoveTimer = Random.Range(idleMoveIntervalTime.x, idleMoveIntervalTime.y);
        }
        else
        {
            idleMoveTimer -= Time.deltaTime;
        }
    }

    void CrumbUpdate()
    {
        //add crumb if no existing or player is far enough from previous
        //TODO: figure out why crumbs are sometimes placed in bursts

        if (targetBreadcrumbs.Count == 0 ||
            Vector2.Distance(targetBreadcrumbs.Peek(), target.position) > crumbInterval)
        {
            AddBreadcrumb(target.position);
        }

        Debug.DrawLine(target.position, targetBreadcrumbs.Peek(), Color.magenta);
        Debug.Log(Vector2.Distance(targetBreadcrumbs.Peek(), target.position));
    }


    protected void Idle()
    {
        var hit = raycastOrigin.position.RaycastTo(target.position, sightRange, LayerMask.GetMask("Player", "Solid"), Color.white);

        //transition to attack if player sighted
        if (hit && hit.transform.CompareTag("Player"))
        {
            targetBreadcrumbs.Clear();
            state = State.Attacking;
        }
    }
    protected void Search()
    {
        searchTimer += Time.deltaTime;

        //reset to idle state
        if (searchTimer >= searchDuration)
        {
            targetBreadcrumbs.Clear();
            searchTimer = 0;
            state = State.Idle;
            return;
        }

        var hit = raycastOrigin.position.RaycastTo(target.position, sightRange, LayerMask.GetMask("Player", "Solid"), Color.yellow);

        if (hit && hit.transform.CompareTag("Player"))
        {
            searchTimer = 0;
            state = State.Attacking;
        }
    }
    protected void Attack()
    {
        var hit = raycastOrigin.position.RaycastTo(target.position,sightRange*attackSightMultiplier, LayerMask.GetMask("Player", "Solid"),Color.red);

        if (!hit || !hit.transform.CompareTag("Player"))
            state = State.Searching;

    }
    void FollowBreadcrumbs(float speed)
    {
        if (targetBreadcrumbs.Count == 0)
            return;

        Vector3 nextBreadcrumb = targetBreadcrumbs.Peek();
        MoveTowardsTarget(nextBreadcrumb, speed);

        //remove the breadcrumb if reached
        if (Vector2.Distance(rb.position, nextBreadcrumb) <= crumbRemovalDistance)
            targetBreadcrumbs.Dequeue();
    }

    void AddBreadcrumb(Vector3 position)
    {
        targetBreadcrumbs.Enqueue(position);

        //limit the number of breadcrumbs stored
        if (targetBreadcrumbs.Count > crumbLimit)
            targetBreadcrumbs.Dequeue();
    }

    void MoveTowardsTarget(Vector2 target, float speed)
    {
        var newPosition = Vector3.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);
    }

    bool CanReachPoint(Vector3 point)
    {
        int mask = LayerMask.GetMask("Solid");
        var distanceToPoint = Vector2.Distance(rb.position, point);
        var hit = Physics2D.Raycast(raycastOrigin.position, 
            (point - raycastOrigin.position).normalized, distanceToPoint, mask);

        if (!hit)
            return true;

        if(distanceToPoint < hit.distance) 
            return true;

        return false;
    }

    protected enum State
    {
        Idle = 0,
        Searching = 1,
        Attacking = 2,
    }
}
