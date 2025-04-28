using UnityEngine;

public class PlayerRoll : StateBase
{
    Player player;

    private bool isRolling = false; 

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    public override void StateEnter()
    {
        Debug.Log("Player Roll Enter");
        player.PlayAnimation("Roll");
    }

    public override void StateExit()
    {
        Debug.Log("Player Roll Exit");
        player.MoveStop();
    }

    public override void StateUpdate()
    {
        Debug.Log("Player Roll Update");

        if (player.CheckAnimationEnd())
        {
            Debug.Log("Player Roll End");
            isRolling = false;
            player.State = PlayerState.Idle;
        }
    }

    public override void StateFixedUpdate()
    {
        if (!isRolling)
        {
            isRolling = true;
            player.OnRoll();
        }
    }
}
