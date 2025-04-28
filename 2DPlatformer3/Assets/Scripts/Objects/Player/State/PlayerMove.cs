using UnityEngine;

public class PlayerMove : StateBase
{
    public override void StateEnter()
    {
        Debug.Log("Player Move Enter");
    }

    public override void StateExit()
    {
        Debug.Log("Player Move Exit");
    }

    public override void StateUpdate()
    {
        Debug.Log("Player Move Update");
    }
}
