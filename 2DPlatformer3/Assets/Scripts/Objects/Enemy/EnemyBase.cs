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
    private AttackArea detectArea;

    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D rigid2d;

    protected Transform detectPlayer;

    [SerializeField] private EnemyMovementState moveState;
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

    [SerializeField] private EnemyActionState actionState;
    protected EnemyActionState ActionState
    {
        get => actionState;
        set
        {
            if (actionState == value) return;

            actionState = value;
            actionStateMachine.StateChange((int)(actionState));
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
            currentHp = Mathf.Clamp(value, 0f, MaxHp);
            OnHpChange?.Invoke(currentHp);

            if (IsDead)
            {
                OnDead();
            }
        }
    }

    public bool IsDead => Hp <= 0;

    public Action<float> OnHpChange { get; set; }
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

    protected Vector2 moveVec = Vector2.zero;
    protected float sightRange = 0f;
    protected float attackRange = 0f;
    protected float moveSpeed = 0f;

    private bool isMoveTransitionLock = false; 

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
        detectArea = transform.GetChild(3).GetComponent<AttackArea>();
    }

    protected virtual void Update()
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
        detectArea.Initialize();
        detectArea.GetComponent<CircleCollider2D>().radius = sightRange;

        // 상태 초기화
        MoveState = EnemyMovementState.Idle;
        ActionState = EnemyActionState.None;
    }

    private void SetData()
    {
        MaxHp = data.maxHp;
        Hp = MaxHp;
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
        // 액션 상태 내용
    }

    protected virtual void CheckMovementState()
    {
        // 움직임 상태 내용
    }
    protected virtual void CheckMovementTransitionBlock()
    {
        if (ActionState == EnemyActionState.Hit || ActionState == EnemyActionState.Dead)
            return;

        if (ActionState != EnemyActionState.None)
        {
            movementStateMachine.SetTransitionBlocked(true);
            isMoveTransitionLock = true;
        }
        else
        {
            movementStateMachine.SetTransitionBlocked(false);

            // NOTE : 반드시 StateNode 이름과 Enum 타입의 이름이 동일할 것
            if(isMoveTransitionLock) PlayAnimation(MoveState.ToString());
            isMoveTransitionLock = false;
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
        animator.Play(name, -1, 0f);
    }

    public bool CheckDetectPlayer(out float distance)
    {
        distance = sightRange + 0.1f;
        detectPlayer = null;

        foreach (IDamageable target in detectArea.TargetList)
        {
            MonoBehaviour mono = target as MonoBehaviour;

            if(mono != null && mono.CompareTag("Player"))
            {
                detectPlayer = mono.transform;
                distance = Vector2.Distance(mono.gameObject.transform.position, transform.position);
                return true;
            }
        }

        return false;        
    }
    #endregion

    #region Move
    public void OnMove(Vector2 vec)
    {
        moveVec = vec;

        if (moveVec.x != 0)
        {
            rigid2d.linearVelocity = new Vector2(moveVec.x * moveSpeed, rigid2d.linearVelocity.y);
        }
    }
    public void MoveStop()
    {
        rigid2d.linearVelocity = new Vector2(0f, rigid2d.linearVelocity.y);
    }
    #endregion

    #region Attack
    public void ActiveAttackArea(bool isLeft)
    {
        if (isLeft)
        {
            attackAreas[1].SetEnableCollider(true);
        }
        else
        {
            attackAreas[0].SetEnableCollider(true);
        }
    }

    public void DeactiveAttackArea()
    {
        attackAreas[1].SetEnableCollider(false);
        attackAreas[0].SetEnableCollider(false);
    }

    public void TryAttack(bool isLeft)
    {
        if (detectPlayer == null) return;

        if (isLeft)
        {
            foreach (IDamageable target in attackAreas[1].TargetList)
            {
                OnAttack(target);
            }
        }
        else
        {
            foreach (IDamageable target in attackAreas[0].TargetList)
            {
                OnAttack(target);
            }
        }
    }
    #endregion

    #region IDamageable
    public void OnDead()
    {
        OnDeadPerformed?.Invoke();

        MoveState = EnemyMovementState.Idle;
        ActionState = EnemyActionState.Dead;
    }

    public void TakeDamage(float damageValue)
    {
        if (IsDead) return;

        ActionState = EnemyActionState.Hit;
        OnHitPerformed?.Invoke();
        Hp -= damageValue;
    }
    #endregion

    #region IPoolable
    public void OnDespawn()
    {
        // 풀에서 디스폰 시 실행
        detectPlayer = null;
        OnHpChange = null;
        OnHitPerformed = null;
        OnDeadPerformed = null;

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