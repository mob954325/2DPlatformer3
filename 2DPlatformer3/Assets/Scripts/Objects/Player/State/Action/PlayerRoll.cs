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
        //Debug.Log("Player Roll Enter");
        player.PlayAnimation("Roll");
        player.OnRoll();
    }

    public override void StateExit()
    {
        //Debug.Log("Player Roll Exit");
        player.MoveStop();
    }

    public override void StateUpdate()
    {
        //Debug.Log("Player Roll Update");

        if (player.CheckAnimationEnd())
        {
            //Debug.Log("Player Roll End");
            player.SetActionState(PlayerActionState.None);
        }
    }

    public override void StateFixedUpdate()
    {
        //Debug.Log("Player Roll FixedUpdate");
    }
}
