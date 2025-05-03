using UnityEngine;

public class PlayerRoll : StateBase
{
    Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    public override void StateEnter()
    {
        player.PlayAnimation("Roll");
        player.OnRoll();
        player.IsImmnue = true;
    }

    public override void StateExit()
    {
        player.MoveStop();
        player.IsImmnue = false;
    }

    public override void StateUpdate()
    {
        if (player.CheckAnimationEnd())
        {
            player.SetActionState(PlayerActionState.None);
        }
    }

    public override void StateFixedUpdate()
    {
    }
}
