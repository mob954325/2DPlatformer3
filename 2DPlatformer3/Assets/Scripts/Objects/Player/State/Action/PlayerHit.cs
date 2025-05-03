using UnityEngine;

public class PlayerHit : StateBase
{
    Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    public override void StateEnter()
    {
        player.PlayAnimation("Hit");
    }

    public override void StateExit()
    {
        player.MoveStop();
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