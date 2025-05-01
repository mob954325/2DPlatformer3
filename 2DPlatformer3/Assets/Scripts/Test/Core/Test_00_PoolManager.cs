﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_00_PoolManager : TestBase
{
#if UNITY_EDITOR
    public Transform spawnPosition;
    public GameObject[] enemyPrefabs;

    public PoolType type;

    private void Start()
    {
        PoolManager.Instacne.Register(PoolType.EnemyMelee.ToString(), enemyPrefabs[0]);
    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        PoolManager.Instacne.Pop(type.ToString(), spawnPosition.position);
    }
#endif
}