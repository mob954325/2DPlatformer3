using UnityEngine;

// NOTE: �⺻ ��������Ʈ ���ʹٶ�

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

        // ����
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
            // ���
            MoveState = EnemyMovementState.Idle;
        }
        else
        {
            // �̵�
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
        if (GetTargetDiection().x < 0) // ����
        {
            rigid2d.AddForce(Vector2.one * knockBackPower, ForceMode2D.Impulse);
        }
        else // ������
        {
            rigid2d.AddForce(-Vector2.one * knockBackPower, ForceMode2D.Impulse);
        }
    }
}