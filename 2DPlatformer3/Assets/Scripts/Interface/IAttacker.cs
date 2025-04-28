using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttacker
{
    /// <summary>
    /// 공격 데미지
    /// </summary>
    public float AttackDamage { get; }

    /// <summary>
    /// 다음 공격까지 남은 시간
    /// </summary>
    public float AttackCooldown { get; set; }

    /// <summary>
    /// 다음 공격 대기 시간
    /// </summary>
    public float MaxAttackCooldown { get; }

    /// <summary>
    /// AttackCooldown의 시간이 남아있는지 확인하는 변수 ( AttackCooldown이 0보다 작으면 true 아니면 false)
    /// </summary>
    public bool CanAttack { get; set; }

    /// <summary>
    /// 공격 시 호출되는 함수
    /// </summary>
    /// <param name="target">공격 대상</param>
    public void OnAttack(IDamageable target);

    /// <summary>
    /// 공격 시 호출되는 델리게이트
    /// </summary>
    public Action<IDamageable> OnAttackPerformed { get; set; }
}
