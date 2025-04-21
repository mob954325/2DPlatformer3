using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData_xxx", menuName = "ScriptableObject/EnemyData", order = 0)]
public class EnemyDataSO : ScriptableObject
{
    [Header("���� �Ӽ�")]
    public float maxHp;

    [Header("���� �Ӽ�")]
    public bool isCombat;   // true�� �� �Ʒ� �� ��� 
    public float sightAngle;
    public float sightRange;
    public float attackRange;
    public float moveSpeed;
    public int damage;
    public float attackCooldown;

    [Header("���Ÿ� ����")]
    public bool isRanged; // true�� �� �Ʒ� �� ���
    public float minAttackDistance;
    public float backstepSpeed;
    public GameObject bulletPrefab;
}