using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_02_PlayerHitAndDead : TestBase
{
#if UNITY_EDITOR
    public Player player;
    public float damage = 2f;

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        player.Hp = 10;
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        player.TakeDamage(damage);
    }
#endif
}
