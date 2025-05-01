using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_00_EnemyLight : TestBase
{
#if UNITY_EDITOR

    public Transform spawnTransform;

    public float damage = 1;

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        PoolManager.Instacne.Pop(PoolType.EnemyMelee, spawnTransform.position, Quaternion.identity);
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        EnemyBase[] enemies = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);

        foreach(EnemyBase enemy in enemies)
        {
            enemy.Hp -= damage;
        }
    }
#endif
}
