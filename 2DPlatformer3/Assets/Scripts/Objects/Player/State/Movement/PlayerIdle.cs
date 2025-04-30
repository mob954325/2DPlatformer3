using UnityEngine;

public class PlayerIdle : StateBase
{
    Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    public override void StateEnter()
    {
        //Debug.Log("Player Idle Enter");
        player.PlayAnimation("Idle");
    }

    public override void StateExit()
    {
        //Debug.Log("Player Idle Exit");
    }

    public override void StateUpdate()
    {
        //Debug.Log("Player Idle Update");
    }

    public override void StateFixedUpdate()
    {
        // 사용 안함
    }
}
