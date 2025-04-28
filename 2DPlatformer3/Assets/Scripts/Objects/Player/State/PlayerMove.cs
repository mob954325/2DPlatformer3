using UnityEngine;

public class PlayerMove : StateBase
{
    Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    public override void StateEnter()
    {
        Debug.Log("Player Move Enter");
        player.PlayAnimation("Run");
    }

    public override void StateExit()
    {
        Debug.Log("Player Move Exit");
        player.MoveStop();
    }

    public override void StateUpdate()
    {
        Debug.Log("Player Move Update");

        player.SpriteFlip(player.Input.InputVec.x < 0);
    }

    public override void StateFixedUpdate()
    {
        if(player.Input.InputVec.x != 0)
        {
            player.OnMove();
        }
    }
}
