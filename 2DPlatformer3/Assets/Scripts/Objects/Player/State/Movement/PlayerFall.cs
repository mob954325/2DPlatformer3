using UnityEngine;

public class PlayerFall : PlayerMove
{
    public override void StateEnter()
    {
        player.PlayAnimation("Fall");
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if(player.CheckIsGround())
        {
            player.SetMovementState(PlayerMovementState.Idle);
        }
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
    }

    public override void StateExit()
    {
        base.StateExit();
    }
}
