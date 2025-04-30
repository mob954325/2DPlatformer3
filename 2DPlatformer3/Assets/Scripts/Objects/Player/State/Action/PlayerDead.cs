using UnityEngine;

public class PlayerDead : StateBase
{
    Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    public override void StateEnter()
    {
        player.PlayAnimation("Dead");
    }

    public override void StateExit()
    {
    }

    public override void StateUpdate()
    {
    }

    public override void StateFixedUpdate()
    {
    }
}
