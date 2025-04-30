using UnityEngine;

public class PlayerJump : PlayerMove
{
    public override void StateEnter()
    {
        player.PlayAnimation("Jump");
        player.OnJump();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if(player.CheckAnimationEnd() && player.IsFalling())
        {
            player.SetMovementState(PlayerMovementState.Fall);
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
