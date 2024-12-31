using UnityEngine;

public class ZombieBase : LivingBase
{
    [SerializeField] State state;
    [SerializeField] protected float speed = 0.15f;
    [SerializeField] protected float searchDuration = 6f;

    private void Update()
    {
        switch (state)
        {
            case State.Idle:
                Idle(); break;
            case State.Searching:
                Search(); break;
            case State.Attacking:
                Attack(); break;
        }
    }

    protected void Idle()
    {

    }
    protected void Search()
    {

    }
    protected void Attack()
    {

    }


    protected enum State
    {
        Idle = 0,
        Searching = 1,
        Attacking = 2,
    }
}
