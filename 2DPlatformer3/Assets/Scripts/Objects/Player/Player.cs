using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어 상태 ( Idle, Move, Attack, Dead )
/// </summary>
public enum PlayerState
{
    Idle = 0,
    Move,
    Attack,
    Dead,

    Rolling,
    Hit,
}

// state에서 Player가 사용하는 변수 빼기
// 1. [0428] - 각 행동별 bool값 만들고 해당 스크립트에서 애니메이션 끝나면 idle상태로 돌아가게 만들기

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour, IAttacker, IDamageable
{
    private PlayerInput input;
    public PlayerInput Input { get => input; }
    private StateMachine stateMachine;

    //AttackArea attackArea;
    //Vector2 attackLocalPosition;

    private Rigidbody2D rigid2d;
    private SpriteRenderer spriteRenderer;
    private Animator animator;


    [SerializeField] PlayerState state;
    private PlayerState State
    {
        get => state;
        set
        {
            if (state == value) return; // 중복 상태 변환 무시

            state = value;
            stateMachine.StateChange((int)state);
        }
    }

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

    private float rollPower = 7f;
    private float speed = 5f;

    // flags
    private bool isAttacking = false;
    private bool isRolling = false;


    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        rigid2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        stateMachine = GetComponentInChildren<StateMachine>();
        //attackArea = GetComponentInChildren<AttackArea>();
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
    private void UpdatePlayerState()
    {
        // 공격
        if (input.IsAttack)
        {
            State = PlayerState.Attack;
        }

        // 구르기
        if (input.IsRoll)
        {
            State = PlayerState.Rolling;
        }

        if(input.InputVec.x == 0)
        {
        // 대기
            State = PlayerState.Idle;
        }
        else
        {
        // 이동
            State = PlayerState.Move;
        }
    }

    public void Initialize()
    {
        Hp = maxHp;
        State = PlayerState.Idle;
    }

    public void PlayAnimation(string name)
    {
        animator.Play(name, 0);
    }

    public bool CheckAnimationEnd()
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
    }

    public void SpriteFlip(bool isLeft)
    {
        spriteRenderer.flipX = isLeft;
    }

    public void SetStateIdle()
    {
        State = PlayerState.Idle;
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
    #endregion

    #region Attack

    #endregion

    #region Interface
    public void OnAttack(IDamageable target)
    {
        target.TakeDamage(AttackDamage);
    }

    public void TakeDamage(float damageValue)
    {
        if (IsDead) return;

        Hp -= damageValue;
        State = PlayerState.Hit;
    }

    public void OnDead()
    {
        State = PlayerState.Dead;
    }
    #endregion
}