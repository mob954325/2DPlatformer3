using UnityEngine;

public class PlayerAttack : StateBase
{
    public override void StateEnter()
    {
        Debug.Log("Player Attack Enter");

    }

    public override void StateExit()
    {
        Debug.Log("Player Attack Exit");
    }

    public override void StateUpdate()
    {
        Debug.Log("Player Attack Update");
    }
}
