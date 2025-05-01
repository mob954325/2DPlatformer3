using UnityEngine;

// NOTE: 기본 스프라이트 왼쪽바라봄

public class EnemyLight : EnemyBase
{
    private bool isPlayerDetected = false;
    private float distanceToTarget = 0f;

    protected override void Update()
    {
        isPlayerDetected = CheckDetectPlayer(out distanceToTarget);

        base.Update();
    }

    protected override void CheckActionState()
    {
        if (IsDead) return;

        if (ActionState == EnemyActionState.Hit || ActionState == EnemyActionState.Dead)
            return;

        // 공격
        if (distanceToTarget <= attackRange && ActionState != EnemyActionState.Attack)
        {
            ActionState = EnemyActionState.Attack;
        }
    }

    protected override void CheckMovementState()
    {
        if (IsDead) return;

        if (ActionState == EnemyActionState.Hit || ActionState == EnemyActionState.Dead)
            return;

        CheckMovementTransitionBlock();

        if (!isPlayerDetected)
        {
            // 대기
            MoveState = EnemyMovementState.Idle;
        }
        else
        {
            // 이동
            MoveState = EnemyMovementState.Move;
        }
    }

    public Vector2 GetTargetDiection()
    {
        return (detectPlayer.transform.position - transform.position).normalized;
    }

    public bool IsTargetInAttackRange()
    {
        return distanceToTarget <= attackRange;
    }

    public void KnockBack()
    {
        float knockBackPower = 1.2f;
        if (GetTargetDiection().x < 0) // 왼쪽
        {
            rigid2d.AddForce(Vector2.one * knockBackPower, ForceMode2D.Impulse);
        }
        else // 오른쪽
        {
            rigid2d.AddForce(-Vector2.one * knockBackPower, ForceMode2D.Impulse);
        }
    }
}