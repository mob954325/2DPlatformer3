using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 상태 ( Idle, Move, Attack, Dead )
/// </summary>
public enum PlayerMovementState
{
    Idle = 0,
    Move,
    Jump,
    Fall,
}

public enum PlayerActionState
{
    None = 0,
    Attack,
    Rolling,
    Hit,
    Dead,
}

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour, IAttacker, IDamageable
{
    private PlayerInput input;
    public PlayerInput Input { get => input; }
    private StateMachine movementStateMachine;
    private StateMachine actionStateMachine;

    public AttackArea[] attackAreas = new AttackArea[2]; // right, left

    private Rigidbody2D rigid2d;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private Transform groundCheckTransform;
    private LayerMask groundLayer;
    private float groundCheckRadius = 0.2f;


    [SerializeField] PlayerMovementState moveState;
    private PlayerMovementState MoveState
    {
        get => moveState;
        set
        {
            if (moveState == value) return; // 중복 상태 변환 무시

            moveState = value;
            movementStateMachine.StateChange((int)moveState);
        }
    }

    [SerializeField] PlayerActionState actionState;
    private PlayerActionState ActionState
    {
        get => actionState;
        set
        {
            if (actionState == value) return;

            actionState = value;
            actionStateMachine.StateChange((int)(actionState));
        }
    }

    private float rollPower = 4f;
    private float jumpPower = 7f;
    private float speed = 3f;
    private bool isImmnue = false; // 피격 가능 여부 ( true : 피격 가능, false : 피격 불가 )
    public bool IsImmnue { get => isImmnue; set => isImmnue = value; }

    #region IAttackable
    private float attackDamage = 1f;
    public float AttackDamage => attackDamage;

    private float attackCooldown = 0.1f;
    public float AttackCooldown 
    {
        get => attackCooldown; 
        set => attackCooldown = Mathf.Clamp(value, 0, MaxAttackCooldown);             

    }

    private float maxAttackCooldown = 0.1f;
    public float MaxAttackCooldown => maxAttackCooldown;

    private bool canAttack = true;
    public bool CanAttack { get => canAttack; set => canAttack = value; }

    public Action<IDamageable> OnAttackPerformed { get; set; }
    #endregion

    #region IDamageable
    private float maxHp = 10;
    public float MaxHp { get => maxHp; set => maxHp = value; }

    private float currentHp;
    public float Hp 
    { 
        get => currentHp;
        set
        {
            currentHp = Mathf.Clamp(value, 0.0f, MaxHp);
            OnHpChange?.Invoke(currentHp / maxHp);

            if(IsDead)
            {
                OnDead();
            }
        }
    }

    public bool IsDead => Hp <= 0f;
    public Action<float> OnHpChange { get; set; }
    public Action OnHitPerformed { get; set; }
    public Action OnDeadPerformed { get; set; }
    #endregion

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        rigid2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        movementStateMachine = transform.GetChild(0).GetComponent<StateMachine>();
        actionStateMachine = transform.GetChild(1).GetComponent<StateMachine>();

        groundCheckTransform = transform.GetChild(2).transform;
        groundLayer = LayerMask.GetMask("Ground");

        attackAreas = transform.GetChild(3).GetComponentsInChildren<AttackArea>();
        foreach(AttackArea area in attackAreas)
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
        // 데이터 초기화
        Hp = maxHp;

        // 공격 콜라이더 초기화
        foreach (AttackArea area in attackAreas)
        {
            area.SetEnableCollider(false);
        }

        // 상태 초기화
        MoveState = PlayerMovementState.Idle;
        ActionState = PlayerActionState.None;
    }

    private void UpdatePlayerState()
    {
        if (GameManager.Instacne.State != GameState.Play) return;

        CheckActionState();
        CheckMovementState();
    }

    private void CheckActionState()
    {
        if (IsDead || ActionState == PlayerActionState.Hit) return;

        // 공격
        if (input.IsAttack && ActionState != PlayerActionState.Attack)
        {
            ActionState = PlayerActionState.Attack;
        }

        // 구르기
        if (input.InputVec.x != 0 && input.IsRoll 
            && MoveState == PlayerMovementState.Move 
            && ActionState != PlayerActionState.Rolling)
        {
            Input.IsRoll = false;
            ActionState = PlayerActionState.Rolling;
        }
    }

    private void CheckMovementState()
    {
        if (IsDead || ActionState == PlayerActionState.Hit) return;

        CheckMovementTransitionBlock();

        bool isGround = CheckIsGround();
        // 점프
        if (isGround 
            && input.IsJump && MoveState != PlayerMovementState.Jump 
            && ActionState == PlayerActionState.None)
        {
            input.IsJump = false;
            MoveState = PlayerMovementState.Jump;
            SoundManager.Instacne.PlaySound(SFXType.Jump);
        }

        if (MoveState != PlayerMovementState.Jump && MoveState != PlayerMovementState.Fall)
        {
            if (input.InputVec.x == 0)
            {
                // 대기
                MoveState = PlayerMovementState.Idle;
            }
            else
            {
                // 이동
                MoveState = PlayerMovementState.Move;
            }
        }
    }
    private void CheckMovementTransitionBlock()
    {
        if (ActionState == PlayerActionState.Dead || ActionState == PlayerActionState.Hit)
            return; // 입력 무시

        if (ActionState != PlayerActionState.None)
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

    public void SetMovementState(PlayerMovementState state)
    {
        MoveState = state;
    }

    public void SetActionState(PlayerActionState state)
    {
        ActionState = state;
    }

    public bool CheckIsGround()
    {
        return Physics2D.OverlapCircle(groundCheckTransform.position, groundCheckRadius, groundLayer);
    }

    public bool IsFalling()
    {
        return rigid2d.linearVelocity.y <= 0;
    }

    public void PlayAnimation(string name)
    {
        animator.Play(name);
    }

    public void PlaySound(SFXType type)
    {
        SoundManager.Instacne.PlaySound(type);
    }

    #endregion

    #region Move
    public void OnMove()
    {
        if(input.InputVec.x != 0)
        {
            rigid2d.linearVelocity = new Vector2(input.InputVec.x * speed, rigid2d.linearVelocity.y);
        }
    }

    public void OnRoll()
    {
        if(input.InputVec.x != 0)
        {
            rigid2d.linearVelocity = new Vector2(input.InputVec.x * rollPower, rigid2d.linearVelocity.y);
        }
    }

    public void MoveStop()
    {
        rigid2d.linearVelocity = new Vector2(0f, rigid2d.linearVelocity.y);
    }

    public void OnJump()
    {
        rigid2d.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
    }
    #endregion

    #region Attack
    public void ActiveAttackArea()
    {
        if (spriteRenderer.flipX)
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

    public void TryAttack()
    {
        if (spriteRenderer.flipX)
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

        SoundManager.Instacne.PlaySound(SFXType.Attack);
    }
    #endregion

    #region IAttacker
    public void OnAttack(IDamageable target)
    {
        if (target == null) return;

        MonoBehaviour mono = target as MonoBehaviour;
        if(!target.IsDead)
        {
            PoolManager.Instacne.Pop(PoolType.HitFx, mono.transform.position + Vector3.up);
            SoundManager.Instacne.PlaySound(SFXType.Hit);
        }
        target.TakeDamage(AttackDamage);
    }
    #endregion

    #region IDamageable
    public void TakeDamage(float damageValue)
    {
        if (IsDead || IsImmnue) return;

        ActionState = PlayerActionState.Hit;
        Hp -= damageValue;
    }

    public void OnDead()
    {
        OnDeadPerformed?.Invoke();

        MoveState = PlayerMovementState.Idle;
        ActionState = PlayerActionState.Dead;
    }
    #endregion

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (groundCheckTransform == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheckTransform.position, groundCheckRadius);
#endif
    }
}