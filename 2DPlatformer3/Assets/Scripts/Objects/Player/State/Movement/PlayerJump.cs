using UnityEngine;

public class PlayerJump : PlayerMove
{
    public override void StateEnter()
    {
        //Debug.Log("Player Jump Enter");
        player.PlayAnimation("Jump");
        player.OnJump();
    }

    public override void StateUpdate()
    {
        //Debug.Log("Player Jump Update");
        base.StateUpdate();

        if(player.CheckAnimationEnd() && player.isFalling())
        {
            player.SetMovementState(PlayerMovementState.Fall);
        }
    }

    public override void StateFixedUpdate()
    {
        //Debug.Log("Player Jump FixedUpdate");
        base.StateFixedUpdate();
    }

    public override void StateExit()
    {
        //Debug.Log("Player Jump Exit");
        base.StateExit();
    }
}
