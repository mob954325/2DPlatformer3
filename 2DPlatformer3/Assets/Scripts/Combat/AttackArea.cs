using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AttackArea : MonoBehaviour
{
    private Collider2D attackCollider;
    private List<IDamageable> targetlist = new List<IDamageable>();
    public List<IDamageable> TargetList { get => targetlist; }

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
            targetlist.Add(damageable);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // target 제거
        collision.gameObject.TryGetComponent(out IDamageable damageable);
        if (damageable != null && damageable != GetComponentInParent<IDamageable>())
        {
            targetlist.Remove(damageable);
        }
    }

    public void SetEnableCollider(bool value)
    {
        attackCollider.enabled = value;
        Debug.Log($"{gameObject.name} : {value}");
    }
}