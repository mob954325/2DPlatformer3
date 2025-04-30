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
        player.PlayAnimation("Idle");
    }

    public override void StateExit()
    {
    }

    public override void StateUpdate()
    {
    }

    public override void StateFixedUpdate()
    {
        // 사용 안함
    }
}
