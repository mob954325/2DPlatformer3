using System.Collections;
using UnityEngine;

public class EnemyLightAttack : StateBase
{
    EnemyLight enemyLight;
    WaitForFixedUpdate waitFixedUpdate;

    private void Awake()
    {
        enemyLight = GetComponentInParent<EnemyLight>();
    }

    public override void StateEnter()
    {
        enemyLight.MoveStop();
        enemyLight.PlayAnimation("Attack");
        enemyLight.ActiveAttackArea(enemyLight.GetTargetDiection().x < 0);
        StartCoroutine(AttackProcess());
    }

    public override void StateExit()
    {
        enemyLight.DeactiveAttackArea();
    }

    public override void StateFixedUpdate()
    {
        if (enemyLight.CheckAnimationEnd())
        {
            if (!enemyLight.IsTargetInAttackRange()) // Å½Áö
            {
                enemyLight.SetActionState(EnemyActionState.None);
            }
            else
            {
                enemyLight.PlayAnimation("Attack");
                StartCoroutine(AttackProcess());
                enemyLight.SpriteFlip(enemyLight.GetTargetDiection().x > 0);

            }
        }
    }

    public override void StateUpdate()
    {

    }

    private IEnumerator AttackProcess()
    {
        yield return waitFixedUpdate;
        enemyLight.TryAttack(enemyLight.GetTargetDiection().x < 0);
    }
}
