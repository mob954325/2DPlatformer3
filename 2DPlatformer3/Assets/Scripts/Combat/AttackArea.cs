using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO
// 여기서 target 관리하고 제거하기
// 다른 곳에서 target을 관리 X

[RequireComponent(typeof(Collider2D))]
public class AttackArea : MonoBehaviour
{
    private Collider2D attackCollider;
    public Action<IDamageable, Transform> OnActiveAttackArea;

    private void Awake()
    {
        attackCollider = GetComponent<Collider2D>();        
    }

    private void Start()
    {
        attackCollider.isTrigger = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // target 추가
        collision.gameObject.TryGetComponent(out IDamageable damageable);
        if (damageable != null && damageable != GetComponentInParent<IDamageable>())
        {
            OnActiveAttackArea?.Invoke(damageable, collision.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // target 제거
    }

    public void SetEnableCollider(bool value)
    {
        attackCollider.enabled = value;
    }
}