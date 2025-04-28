using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어 상태 ( Idle, Move, Attack, Dead )
/// </summary>
enum PlayerState
{
    Idle = 0,
    Move,
    Attack,
    Dead,
}

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour, IAttacker, IDamageable
{
    PlayerInput input;
    StateMachine stateMachine;

    //AttackArea attackArea;
    //Vector2 attackLocalPosition;

    Rigidbody2D rigid2d;
    SpriteRenderer spriteRenderer;
    Animator anim;

    [SerializeField] PlayerState state;
    PlayerState State
    {
        get => state;
        set
        {
            if (state == value) return; // 중복 상태 변환 무시

            state = value;
        }
    }

    private float stateTimer = 0f;
    private int attackCount = 1;

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
            else if(prevHp - currentHp > 0)
            {
                //State = PlayerState.Hit;
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

    bool isRolling = false;
    bool isAttacking = false;
    bool isHit = false;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();
        rigid2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        //attackArea = GetComponentInChildren<AttackArea>();
    }

    private void OnEnable()
    {
        Initialize();
    }

    private void FixedUpdate()
    {   
    }

    private void Update()
    {    
    }

    private void Initialize()
    {
        Hp = maxHp;
    }

    #region Functions
    private bool CheckAnimationEnd()
    {
        return anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
    }

    public void OnAttack(IDamageable target)
    {
        target.TakeDamage(AttackDamage);
    }

    public void TakeDamage(float damageValue)
    {
        if (IsDead) return;

        Hp -= damageValue;
    }

    public void OnDead()
    {
        State = PlayerState.Dead;
    }
    #endregion
}