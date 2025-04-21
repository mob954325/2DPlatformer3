using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;


/// <summary>
/// ��� �����ϴ� ���� �������� �޴� Ŭ����
/// </summary>
public class EnemyCombat : EnemyBase, IAttacker
{

    protected Transform targetTransform;
    protected IDamageable target;

    protected AttackArea attackArea;
    protected CircleCollider2D attackAreaCollider;

    protected float sightAngle = 20.0f;
    protected float sightRadius = 5.0f;

    protected Vector2 moveDirection = Vector2.zero;
    protected float attackRange = 2.0f;
    protected float speed = 3;
    protected bool isFacingLeft = true;
    protected float distanceToTarget = 0;

    private float attackDamage = 1f;
    public float AttackDamage => attackDamage;

    private float currentattackCooldown = 0f;
    public float AttackCooldown
    {
        get => currentattackCooldown;
        set
        {
            currentattackCooldown = Mathf.Clamp(value, 0.0f, MaxAttackCooldown);
        }
    }

    private float maxAttackCooldown = 2f;
    public float MaxAttackCooldown => maxAttackCooldown;

    private bool canAttack = true;
    public bool CanAttack
    {
        get => canAttack;
        set => canAttack = value;

    }

    /// <summary>
    /// ���� ������ Ȯ���ϴ� ����
    /// </summary>
    protected bool IsAttack = false;

    protected float attackDelayTimer = 0.0f;
    protected float maxAttackDelayTime = 1.0f;

    public Action<IDamageable> OnAttackPerformed { get; set; }

    protected override void Start()
    {
        base.Start();

        attackArea = GetComponentInChildren<AttackArea>();
        attackAreaCollider = attackArea.gameObject.GetComponent<CircleCollider2D>();

        attackArea.OnActiveAttackArea += HandleTargetDetected;
        attackAreaCollider.radius = sightRadius;
        CanAttack = true;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        // attackArea.OnActiveAttackArea = null; //remove listener ���� ����ϱ� AttackArea����
    }

    protected override void Update()
    {
        HandleCooldown();
        base.Update();
    }

    protected override void SetData(EnemyDataSO data)
    {
        if(data.isCombat)
        {
            sightAngle = data.sightAngle;
            sightRadius = data.sightRange;
            attackRange = data.attackRange;
            speed = data.moveSpeed;
            attackRange = data.damage;
            maxAttackCooldown = data.attackCooldown;
        }
        base.SetData(data);
    }

    // Functions ---------------------------------------------------------------------------------------

    /// <summary>
    /// Chasing Update �� | base ������ �� �������� �����ϱ�
    /// </summary>
    protected override void OnChasingState()
    {
        UpdateChasingTarget();

        if(targetTransform != null)
        {
            distanceToTarget = Vector2.Distance(targetTransform.position, (transform.position));
        }
        base.OnChasingState();
    }

    /// <summary>
    /// Attack Update �� | base ������ �� ������ �����ϱ�
    /// </summary>
    protected override void OnAttackState()
    {
        UpdateAttackTarget();

        if(targetTransform != null)
        {
            distanceToTarget = Vector2.Distance(targetTransform.position, (transform.position));
        }
        base.OnAttackState();
    }

    // Functions ---------------------------------------------------------------------------------------

    private void HandleTargetDetected(IDamageable target, Transform targetTransform)
    {
        if (gameObject.layer == targetTransform.gameObject.layer) return;

        isFacingLeft = targetTransform.position.x - transform.position.x < 0 ? true : false; // �÷��̾ ���� �ȿ� ���� ���� �ٶ󺸴� ��ġ ����
        moveDirection = isFacingLeft ? Vector2.left : Vector2.right;

        this.targetTransform = targetTransform;
        this.target = target;

        // �þ߰��� �ִ��� Ȯ��
        if (IsInsight(targetTransform))
        {
            CurrentState = EnemyState.Chasing;
        }
        else
        {
            this.targetTransform = null;
            this.target = null;
        }
    }

    /// <summary>
    /// target�� �þ߿� ���Դ��� Ȯ��
    /// </summary>
    bool IsInsight(Transform target)
    {
        if (targetTransform == null) return false ;

        Vector2 dir = targetTransform.position - (transform.position);
        float dot = Vector2.Dot(dir.normalized, GetFactingDirection());

        return dot > Mathf.Cos(sightAngle * 0.5f * Mathf.Deg2Rad);
    }

    Vector2 GetFactingDirection()
    {
        return isFacingLeft ? Vector2.left : Vector2.right;
    }

    void UpdateChasingTarget()
    {
        if (targetTransform == null || ShouldStopChase())
        {
            // Ÿ���� ������ ���
            targetTransform = null;
            target = null;
            CurrentState = EnemyState.Idle;
        }
        else if(distanceToTarget < attackRange)
        {
            CurrentState = EnemyState.Attack;
        }
    }

    bool ShouldStopChase()
    {
        if (targetTransform == null) return false;

        Vector2 dir = targetTransform.position - transform.position;

        return dir.sqrMagnitude > sightRadius * sightRadius;
    }

    void UpdateAttackTarget()
    {
        if (targetTransform != null)
        {
            // ��Ÿ��ȿ� �÷��̾ ����
            if(distanceToTarget < attackRange)
            {
                OnAttack(target);
            }
            else
            {
                CurrentState = EnemyState.Chasing;
            }
        }
        else // �þ� ������ ���
        {
            CurrentState = EnemyState.Idle;
        }
    }

    /// <summary>
    /// CanAttack ��Ȱ��ȭ, attack cooldown �ʱ�ȭ �ϴ� �Լ�
    /// </summary>
    protected void StartAttackCoolDown()
    {
        CanAttack = false;
        AttackCooldown = MaxAttackCooldown;
    }

    // IAttackable -------------------------------------------------------------------------------------------

    public void OnAttack(IDamageable target)
    {
        if (CanAttack && target != null)
        {
            PerformAttack(target);
            StartAttackCoolDown();
        }
    }

    /// <summary>
    /// ���� �� �����ϴ� �߰��� ����Ǵ� ����
    /// </summary>
    protected virtual void PerformAttack(IDamageable target)
    {
        OnAttackPerformed?.Invoke(target);
    }

    void HandleCooldown()
    {
        if (!CanAttack)
        {
            AttackCooldown -= Time.deltaTime;

            if (AttackCooldown <= 0.0f)
            {
                CanAttack = true;
            }
        }
    }

    // Debug -------------------------------------------------------------------------------------------
    private void OnDrawGizmos()
    {
        if (attackArea != null)
        {
            // �þ߰� 
            Handles.color = CurrentState == EnemyState.Attack ? Color.red : Color.green;

            Vector3 origin = transform.position;
            Vector3 rightDir = Quaternion.Euler(0, 0, sightAngle * 0.5f) * GetFactingDirection();
            Vector3 leftDir = Quaternion.Euler(0, 0, -sightAngle * 0.5f) * GetFactingDirection();

            Handles.DrawLine(origin, origin + rightDir * sightRadius);
            Handles.DrawLine(origin, origin + leftDir * sightRadius);
            Vector3 fromDir = Quaternion.Euler(0, 0, -sightAngle * 0.5f) * GetFactingDirection();
            Handles.DrawWireArc(origin, Vector3.forward, fromDir, sightAngle, sightRadius);

            Handles.color = Color.yellow;
            Handles.DrawWireArc(origin, Vector3.forward, fromDir, sightAngle, attackRange);
        }
    }
}