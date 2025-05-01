using UnityEngine;

public class EnemyLightHit : StateBase
{
    EnemyLight enemyLight;
    private void Awake()
    {
        enemyLight = GetComponentInParent<EnemyLight>();
    }

    public override void StateEnter()
    {
        enemyLight.PlayAnimation("Hit");
        enemyLight.KnockBack();
    }

    public override void StateExit()
    {
        
    }

    public override void StateFixedUpdate()
    {
        
    }

    public override void StateUpdate()
    {
        if(enemyLight.CheckAnimationEnd())
        {
            enemyLight.SetActionState(EnemyActionState.None);
        }
    }
}
