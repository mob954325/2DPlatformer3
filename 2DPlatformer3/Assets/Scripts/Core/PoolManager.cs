using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IPoolable
{
    public void OnSpawn();
    public void OnDespawn();

    /// <summary>
    /// PoolManager Queue�� ���ư��� ���� ��������Ʈ
    /// </summary>
    Action ReturnAction { get; set; }
}

// �ʱ�ȭ
// ������Ʈ ������
// �迭 Ȯ��
// ��� ������Ʈ ����

public class PoolManager : Singleton<PoolManager>
{
    private class PoolData
    {
        public GameObject prefab;
        public Queue<GameObject> readyQueue = new Queue<GameObject>();
        public List<GameObject> objectList = new List<GameObject>();
        public int capacity;
    }

    Dictionary<string, PoolData> poolDictionary = new();

    public void Register(string key, GameObject prefab, int capacity = 8)
    {
        if(poolDictionary.ContainsKey(key))
        {
            Debug.LogWarning($"Pool with key '{key}' already registered.");
            return;
        }

        PoolData data = new()
        {
            prefab = prefab,
            capacity = capacity
        };

        for (int i = 0; i < capacity; i++)
        {
            GameObject obj = CreateInstantiate(key, data);
            data.readyQueue.Enqueue(obj);
            data.objectList.Add(obj);
        }

        poolDictionary.Add(key, data);
    }

    public GameObject Pop(string key, Vector3? position = null, Quaternion? rotataion = null)
    {
        if (!poolDictionary.TryGetValue(key, out var data))
        {
            Debug.LogError($"Pool with key '{key}' not found.");
            return null;
        }

        if (data.readyQueue.Count == 0) // Ȯ��
        {
            ExpandPoolSize(key);
        }

        GameObject obj = data.readyQueue.Dequeue();

        obj.transform.position = position.GetValueOrDefault(Vector3.zero);
        obj.transform.rotation = rotataion.GetValueOrDefault(Quaternion.identity);

        obj.SetActive(true);

        if (obj.TryGetComponent<IPoolable>(out var poolable))
        {
            poolable.OnSpawn();
        }

        return obj;
    }

    public GameObject Pop(PoolType type, Vector3? position = null, Quaternion? rotataion = null)
    {
        return Pop(type.ToString(), position, rotataion);
    }

    private void ReturnToPool(string key, GameObject obj)
    {
        if (!poolDictionary.TryGetValue(key, out var data))
        {
            Debug.LogError($"Trying to return object to nonexistent pool '{key}'.");
            Destroy(obj);
            return;
        }

        if (obj.TryGetComponent(out IPoolable poolable))
        {
            poolable.OnDespawn();
        }

        obj.SetActive(false);
        data.readyQueue.Enqueue(obj);
    }

    private void ExpandPoolSize(string key)
    {
        if (!poolDictionary.TryGetValue(key, out var data))
        {
            Debug.LogError($"Trying to expand pool to nonexistent pool '{key}'.");
            return;
        }

        int prevCapacity = data.capacity;
        data.capacity *= 2;
        Debug.LogWarning($"{gameObject.name} Ǯ �Ŵ��� ũ�� Ȯ�� | {prevCapacity} -> {data.capacity}");

        // ���ο� Ǯ ���
        data.objectList = new List<GameObject>(data.capacity);
        for(int i = 0; i < prevCapacity; i++)
        {
            GameObject obj = CreateInstantiate(key, data);
            data.objectList.Add(obj);
            data.readyQueue.Enqueue(obj);
        }
    }

    private GameObject CreateInstantiate(string key, PoolData data)
    {
        GameObject obj = Instantiate(data.prefab);

        obj.SetActive(false);
        obj.transform.position = Vector3.zero; 

        if (obj.TryGetComponent<IPoolable>(out var poolable))
        {
            poolable.ReturnAction = () => { ReturnToPool(key, obj); };
        }

        return obj;
    }

    public void ClearPool(string key)
    {
        if (!poolDictionary.TryGetValue(key, out var data)) return;

        foreach (var obj in data.objectList)
        {
            Destroy(obj);
        }

        poolDictionary.Remove(key);
    }
}