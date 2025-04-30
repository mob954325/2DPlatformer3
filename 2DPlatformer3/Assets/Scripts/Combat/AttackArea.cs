using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// IDamageable을 가진 오브젝트 정보
/// </summary>
public struct DamageableInfo
{
    public GameObject targetObj;
    public IDamageable target;
}

[RequireComponent(typeof(Collider2D))]
public class AttackArea : MonoBehaviour
{
    private Collider2D attackCollider;

    DamageableInfo info;
    public DamageableInfo Info { get => info; }

    public void Initialize()
    {
        attackCollider = GetComponent<Collider2D>();        
        attackCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // target 추가
        collision.gameObject.TryGetComponent(out IDamageable damageable);
        if (damageable != null && damageable != GetComponentInParent<IDamageable>())
        {
            info.target = damageable;
            info.targetObj = collision.gameObject;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // target 추가
        collision.gameObject.TryGetComponent(out IDamageable damageable);
        if (damageable != null && damageable != GetComponentInParent<IDamageable>())
        {
            info.target = damageable;
            info.targetObj = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // target 제거
        collision.gameObject.TryGetComponent(out IDamageable damageable);
        if (damageable != null && damageable != GetComponentInParent<IDamageable>())
        {
            info.target = null;
            info.targetObj = null;
        }
    }

    public void SetEnableCollider(bool value)
    {
        attackCollider.enabled = value;
        Debug.Log($"{gameObject.name} : {value}");
    }
}