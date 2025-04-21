﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_01_PlayerAttack : TestBase
{
#if UNITY_EDITOR
    public GameObject enemyObject;
    public Transform spawnTransform;

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        EnemyBase enemy = Instantiate(enemyObject).GetComponent<EnemyBase>();

        enemy.gameObject.transform.position = spawnTransform.position;

        enemy.MaxHp = 10;
    }
#endif
}
