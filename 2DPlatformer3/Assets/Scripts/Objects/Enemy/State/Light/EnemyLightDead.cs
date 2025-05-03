using UnityEngine;

public class EnemyLightDead : StateBase
{
    EnemyLight enemyLight;
    private void Awake()
    {
        enemyLight = GetComponentInParent<EnemyLight>();
    }

    public override void StateEnter()
    {
        enemyLight.MoveStop();
        enemyLight.PlayAnimation("Dead");
    }

    public override void StateExit()
    {
        
    }

    public override void StateFixedUpdate()
    {
        
    }

    public override void StateUpdate()
    {
        
    }
}
