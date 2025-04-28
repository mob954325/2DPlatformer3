using UnityEngine;
using UnityEngine.InputSystem;

public class Test_00_PlayerState : TestBase
{
    public Player player;

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        player.TakeDamage(1);
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        player.Hp = 0;
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        player.Hp = player.MaxHp;
    }
}