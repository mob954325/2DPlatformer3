using UnityEngine;

public class PlayerDead : StateBase
{
    Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    public override void StateEnter()
    {
        //Debug.Log("Player Dead Enter");
        player.PlayAnimation("Dead");
    }

    public override void StateExit()
    {
        //Debug.Log("Player Dead Exit");
    }

    public override void StateUpdate()
    {
    }

    public override void StateFixedUpdate()
    {
        // 사용 안함
    }
}
