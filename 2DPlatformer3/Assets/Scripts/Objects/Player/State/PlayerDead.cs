using UnityEngine;

public class PlayerDead : StateBase
{
    public override void StateEnter()
    {
        Debug.Log("Player Dead Enter");
    }

    public override void StateExit()
    {
        Debug.Log("Player Dead Exit");
    }

    public override void StateUpdate()
    {
        Debug.Log("Player Dead Update");
    }
}
