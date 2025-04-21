using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Tooltip("PoolType ������� ������Ʈ�� ��ġ �� ��")]
    public GameObject[] poolPrefab = new GameObject[(int)PoolType.PoolTypeCount];

    protected override void Awake()
    {
        base.Awake();
        SetPoolManager();
    }

    private void SetPoolManager()
    {
        for(int i = 0; i < (int)PoolType.PoolTypeCount; i++)
        {
            PoolManager.Instacne.Register(((PoolType)i).ToString(), poolPrefab[i]);
        }
    }
}
