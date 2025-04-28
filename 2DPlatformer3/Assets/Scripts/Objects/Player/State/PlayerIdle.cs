using UnityEngine;

public class PlayerIdle : StateBase
{
    public override void StateEnter()
    {
        Debug.Log("Player Idle Eneter");
    }

    public override void StateExit()
    {
        Debug.Log("Player Idle Exit");
    }

    public override void StateUpdate()
    {
        Debug.Log("Player Idle Update");
    }
}
