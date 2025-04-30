using UnityEngine;

public class PlayerMove : StateBase
{
    protected Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    public override void StateEnter()
    {
        player.PlayAnimation("Move");
    }

    public override void StateExit()
    {
        player.MoveStop();
    }

    public override void StateUpdate()
    {
        if(player.Input.InputVec.x != 0)
        {
            player.SpriteFlip(player.Input.InputVec.x < 0);
        }
    }

    public override void StateFixedUpdate()
    {
        if(player.Input.InputVec.x != 0)
        {
            player.OnMove();
        }
    }
}
