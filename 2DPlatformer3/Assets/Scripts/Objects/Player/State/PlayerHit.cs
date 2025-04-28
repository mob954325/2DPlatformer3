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
        Debug.Log("Player Hit Enter");
        player.PlayAnimation("Hit");
    }

    public override void StateExit()
    {
        Debug.Log("Player Hit Exit");        
    }

    public override void StateUpdate()
    {
        Debug.Log("Player Hit Update");
        if (player.CheckAnimationEnd())
        {
            Debug.Log("Player Hit End");
            player.State = PlayerState.Idle;
        }
    }

    public override void StateFixedUpdate()
    {
        // 사용 안함
    }
}