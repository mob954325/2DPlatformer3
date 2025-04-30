using NUnit.Framework;
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
    private float groundCheckRadius = 0.12f;


    [SerializeField] PlayerMovementState moveState;
    private PlayerMovementState Movestate
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
            actionStateMachine.StateChange((int)(actionState)); // None 제외
        }
    }

    private float rollPower = 7f;
    private float jumpPower = 7f;
    private float speed = 5f;

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
            float prevHp = currentHp;
            currentHp = Mathf.Clamp(value, 0.0f, MaxHp);
            OnHpChange?.Invoke();

            if(IsDead)
            {
                OnDead();
            }

            Debug.Log($"Player {Hp}");
        }
    }

    public bool IsDead => Hp <= 0f;
    public Action OnHpChange { get; set; }
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

    private void OnEnable()
    {
        Initialize();
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
        Movestate = PlayerMovementState.Idle;
        ActionState = PlayerActionState.None;
    }

    private void UpdatePlayerState()
    {
        CheckActionState();
        CheckMovementState();
    }

    private void CheckActionState()
    {
        // 공격
        if (input.IsAttack && ActionState != PlayerActionState.Attack)
        {
            ActionState = PlayerActionState.Attack;
        }

        // 구르기
        if (input.InputVec.x != 0 && input.IsRoll && ActionState != PlayerActionState.Rolling)
        {
            ActionState = PlayerActionState.Rolling;
        }
    }

    private void CheckMovementState()
    {
        CheckMovementTransitionBlock();

        bool isGround = CheckIsGround();
        // 점프
        if (isGround 
            && input.IsJump && Movestate != PlayerMovementState.Jump 
            && ActionState == PlayerActionState.None)
        {
            Movestate = PlayerMovementState.Jump;
        }

        if (Movestate != PlayerMovementState.Jump && Movestate != PlayerMovementState.Fall)
        {
            if (input.InputVec.x == 0)
            {
                // 대기
                Movestate = PlayerMovementState.Idle;
            }
            else
            {
                // 이동
                Movestate = PlayerMovementState.Move;
            }
        }
    }
    private void CheckMovementTransitionBlock()
    {
        if (ActionState == PlayerActionState.Hit || ActionState == PlayerActionState.Dead)
            return; // 입력 무시

        if (ActionState != PlayerActionState.None)
        {
            movementStateMachine.SetTransitionBlocked(true);
        }
        else
        {
            movementStateMachine.SetTransitionBlocked(false);

            // NOTE : 반드시 StateNode 이름과 Enum 타입의 이름이 동일할 것
            PlayAnimation(Movestate.ToString());
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
        Movestate = state;
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
        return rigid2d.linearVelocity.y < 0;
    }

    public void PlayAnimation(string name)
    {
        animator.Play(name);
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
    public IDamageable ActiveAttackArea()
    {
        IDamageable target;
        if (spriteRenderer.flipX)
        {
            attackAreas[1].SetEnableCollider(true);

            target = attackAreas[1].Info.target;
        }
        else
        {
            attackAreas[0].SetEnableCollider(true);      
            target = attackAreas[0].Info.target;
        }

        return target;
    }

    public void DeactiveAttackArea()
    {
        attackAreas[1].SetEnableCollider(false);
        attackAreas[0].SetEnableCollider(false);
    }
    #endregion

    #region IAttacker
    public void OnAttack(IDamageable target)
    {
        if (target == null) return;

        target.TakeDamage(AttackDamage);
    }
    #endregion

    #region IDamageable
    public void TakeDamage(float damageValue)
    {
        if (IsDead) return;

        Hp -= damageValue;
        ActionState = PlayerActionState.Hit;
    }

    public void OnDead()
    {
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