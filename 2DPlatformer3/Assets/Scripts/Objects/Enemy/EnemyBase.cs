using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public enum EnemyMovementState
{
    Idle = 0,
    Move,
}

public enum EnemyActionState
{
    None = 0,
    Attack,
    Hit,
    Dead
}

[RequireComponent(typeof(Rigidbody2D),typeof(Collider2D))]
public abstract class EnemyBase : MonoBehaviour, IAttacker, IDamageable, IPoolable
{
    [SerializeField] protected EnemyDataSO data;
    private StateMachine movementStateMachine;
    private StateMachine actionStateMachine;
    private AttackArea[] attackAreas = new AttackArea[2];

    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D rigid2d;

    private EnemyMovementState moveState;
    protected EnemyMovementState MoveState
    {
        get => moveState;
        set
        {
            if (moveState == value) return;

            moveState = value;
            movementStateMachine.StateChange((int)moveState);
        }
    }

    private EnemyActionState actionState;
    private EnemyActionState ActionState
    {
        get => actionState;
        set
        {
            if (actionState == value) return;

            actionState = value;
            actionStateMachine.StateChange((int)(actionState)); // None 제외
        }
    }

    #region IDamageable
    public Action ReturnAction { get; set; }

    private float maxHp = 0f;
    public float MaxHp { get => maxHp; set => maxHp = value; }

    private float currentHp = 0;
    public float Hp
    {
        get => currentHp;
        set
        {
            currentHp = Mathf.Clamp(value, 0.0f, MaxHp);
            OnHpChange?.Invoke();

            Debug.Log($"{gameObject.name} {Hp}");

            if (IsDead)
            {
                OnDead();
            }
        }
    }

    public bool IsDead => Hp <= 0;

    public Action OnHpChange { get; set; }
    public Action OnHitPerformed { get; set; }
    public Action OnDeadPerformed { get; set; }
    #endregion

    #region IAttacker
    private float attackDamage = 0;
    public float AttackDamage => attackDamage;

    private float attackCooldown = 0f;
    public float AttackCooldown 
    {
        get => attackCooldown;
        set => attackCooldown = value; 
    }

    private float maxAttackCooldown = 0f;
    public float MaxAttackCooldown => maxAttackCooldown;

    private bool canAttack = false;
    public bool CanAttack { get => canAttack; set => canAttack = value; }
    public Action<IDamageable> OnAttackPerformed { get; set; }
    #endregion

    private Vector2 moveVec = Vector2.zero;
    private float sightAngle = 0f;
    private float sightRange = 0f;
    private float attackRange = 0f;
    private float moveSpeed = 0f;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        rigid2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        movementStateMachine = transform.GetChild(0).GetComponent<StateMachine>();
        actionStateMachine = transform.GetChild(1).GetComponent<StateMachine>();

        attackAreas = transform.GetChild(2).GetComponentsInChildren<AttackArea>();
        foreach (AttackArea area in attackAreas)
        {
            area.Initialize();
        }
    }

    private void Update()
    {
        UpdatePlayerState();
    }

    #region Functions
    public void Initialize()
    {
        SetData();
        SetAdditionalData();

        // 공격 콜라이더 초기화
        foreach (AttackArea area in attackAreas)
        {
            area.SetEnableCollider(false);
        }

        // 상태 초기화
        MoveState = EnemyMovementState.Idle;
        ActionState = EnemyActionState.None;
    }

    private void SetData()
    {
        MaxHp = data.maxHp;
        Hp = MaxHp;
        sightAngle = data.sightAngle;
        sightRange = data.sightRange;
        attackRange = data.attackRange;
        moveSpeed = data.moveSpeed;
        attackDamage = data.damage;
        maxAttackCooldown = data.attackCooldown;
    }

    protected virtual void SetAdditionalData()
    {
        // 추가 데이터 설정하는 곳
    }

    private void UpdatePlayerState()
    {
        CheckActionState();
        CheckMovementState();
    }

    protected virtual void CheckActionState()
    {
        // 공격
        if (ActionState != EnemyActionState.Attack)
        {
            ActionState = EnemyActionState.Attack;
        }
    }

    protected virtual void CheckMovementState()
    {
        CheckMovementTransitionBlock();

        // 점프
        if (moveVec.x == 0)
        {
            // 대기
            MoveState = EnemyMovementState.Idle;
        }
        else
        {
            // 이동
            MoveState = EnemyMovementState.Move;
        }
    }
    protected virtual void CheckMovementTransitionBlock()
    {
        if (ActionState == EnemyActionState.Hit || ActionState == EnemyActionState.Dead)
            return; // 입력 무시

        if (ActionState != EnemyActionState.None)
        {
            movementStateMachine.SetTransitionBlocked(true);
        }
        else
        {
            movementStateMachine.SetTransitionBlocked(false);

            // NOTE : 반드시 StateNode 이름과 Enum 타입의 이름이 동일할 것
            PlayAnimation(MoveState.ToString());
        }
    }

    public bool CheckAnimationEnd()
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
    }

    public void SpriteFlip(bool isLeft)
    {
        spriteRenderer.flipX = isLeft;
    }

    public void SetMovementState(EnemyMovementState state)
    {
        MoveState = state;
    }

    public void SetActionState(EnemyActionState state)
    {
        ActionState = state;
    }

    public void PlayAnimation(string name)
    {
        animator.Play(name);
    }
    #endregion

    #region IDamageable
    public void OnDead()
    {
        OnDeadPerformed?.Invoke();
        ActionState = EnemyActionState.Dead;
    }

    public void TakeDamage(float damageValue)
    {
        if (IsDead) return;

        Hp -= damageValue;
        OnHitPerformed?.Invoke();
        ActionState = EnemyActionState.Hit;
    }
    #endregion

    #region IPoolable
    public void OnDespawn()
    {
        // 풀에서 디스폰 시 실행
        BeforeDespawn();
    }

    protected virtual void BeforeDespawn()
    {
        // 상속 받은 enemy가 풀에서 디스폰 시 실행될 내용
    }

    public void OnSpawn()
    {
        // 풀에서 스폰 시 실행
        Initialize();
        BeforeSpawn();
    }

    protected virtual void BeforeSpawn()
    {
        // 상속 받은 enemy가 풀에서 스폰 시 실행될 내용
    }
    #endregion

    #region IAttacker
    public void OnAttack(IDamageable target)
    {
        target.TakeDamage(AttackDamage);
    }
    #endregion
}