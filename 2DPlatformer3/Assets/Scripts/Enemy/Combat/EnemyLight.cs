using UnityEngine;

public class EnemyLight : EnemyCombat
{
    private Animator anim;


    protected override void Start()
    {
        anim = GetComponent<Animator>();

        base.Start();
    }

    #region StateStart
    protected override void OnIdleStateStart()
    {
        base.OnIdleStateStart();

        anim.Play("Idle", 0);
    }

    protected override void OnChaseStateStart()
    {
        base.OnChaseStateStart();

        //anim.Play("Run", 0);
    }

    protected override void OnAttackStateStart()
    {
        base.OnAttackStateStart();

        //anim.Play("Attack", 0);
    }

    protected override void OnHitStateStart()
    {
        base.OnHitStateStart();

        //anim.Play("Hit", 0);
    }

    protected override void OnDeadStateStart()
    {
        base.OnDeadStateStart();

        anim.Play("Dead");
    }

    #endregion

    #region StateUpdate

    protected override void OnIdleState()
    {
        base.OnIdleState();

        if (targetTransform != null && distanceToTarget < sightRadius && IsInsight(targetTransform))
        {
            CurrentState = EnemyState.Chasing;
        }
    }

    protected override void OnChasingState()
    {
        base.OnChasingState();

        rigid2d.linearVelocity = new Vector2(moveDirection.x * speed, rigid2d.linearVelocity.y);

        if (distanceToTarget < attackRange) CurrentState = EnemyState.Attack;
        else if (distanceToTarget > sightRadius)
        {
            targetTransform = null;
            CurrentState = EnemyState.Idle;
        }
    }

    protected override void OnAttackState()
    {
        base.OnAttackState();

        if (CheckAnimationEnd())
        {
            CurrentState = EnemyState.Idle;
        }
    }

    protected override void OnHitState()
    {
        base.OnHitState();

/*        if(CheckAnimationEnd())
        {
            CurrentState = EnemyState.Idle;
        }*/
    }

    #endregion

    #region StateEnd
    #endregion

    #region Functions
    private bool CheckAnimationEnd()
    {
        return anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
    }
    #endregion
}