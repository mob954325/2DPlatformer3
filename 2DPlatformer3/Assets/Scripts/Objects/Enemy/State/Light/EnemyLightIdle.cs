using UnityEngine;

public class EnemyLightIdle : StateBase
{
    EnemyLight enemyLight;

    private void Awake()
    {
        enemyLight = GetComponentInParent<EnemyLight>();
    }

    public override void StateEnter()
    {
        enemyLight.PlayAnimation("Idle");
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
