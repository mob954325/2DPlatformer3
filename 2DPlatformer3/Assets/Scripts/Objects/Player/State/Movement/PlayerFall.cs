using UnityEngine;

public class PlayerFall : PlayerMove
{
    public override void StateEnter()
    {
        //Debug.Log("Player Fall Enter");
        player.PlayAnimation("Fall");
    }

    public override void StateUpdate()
    {
        //Debug.Log("Player Fall Update");
        base.StateUpdate();

        if(player.CheckIsGround())
        {
            player.SetMovementState(PlayerMovementState.Idle);
        }
    }

    public override void StateFixedUpdate()
    {
        //Debug.Log("Player Fall FixedUpdate");
        base.StateFixedUpdate();
    }

    public override void StateExit()
    {
        //Debug.Log("Player Fall Exit");
        base.StateExit();
    }
}
