using UnityEngine;

public class EnemyLightMove : StateBase
{
    EnemyLight enemyLight;

    private void Awake()
    {
        enemyLight = GetComponentInParent<EnemyLight>();
    }

    public override void StateEnter()
    {
        enemyLight.PlayAnimation("Move");
    }

    public override void StateExit()
    {
        
    }

    public override void StateFixedUpdate()
    {
        if (enemyLight.GetTargetDiection().x < 0)
        {
            enemyLight.OnMove(Vector2.left);
            enemyLight.SpriteFlip(false);
        }
        else if (enemyLight.GetTargetDiection().x > 0)
        {
            enemyLight.OnMove(Vector2.right);
            enemyLight.SpriteFlip(true);
        }
    }

    public override void StateUpdate()
    {
        
    }
}
