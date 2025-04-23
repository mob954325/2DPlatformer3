using System;
using System.Collections;
using UnityEngine;

enum PlayerState
{
    Idle = 0,
    Move,
    Roll,
    Hit,
    Attack,
    Dead,
}

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour, IAttacker, IDamageable
{
    PlayerInput input;
    AttackArea attackArea;
    Vector2 attackLocalPosition;

    Rigidbody2D rigid2d;
    SpriteRenderer spriteRenderer;
    Animator anim;

    [SerializeField] PlayerState state;
    PlayerState State
    {
        get => state;
        set
        {
            if (state == value) return;

            StateEnd(state);
            state = value;

            rigid2d.linearVelocity = new Vector2(0.0f, rigid2d.linearVelocity.y); // 이동 정지
            StateStart(state);
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
                State = PlayerState.Hit;
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

    private void Start()
    {
        input = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();
        rigid2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        attackArea = GetComponentInChildren<AttackArea>();

        attackArea.OnActiveAttackArea += (target, _) => { OnAttack(target); };

        attackArea.SetEnableCollider(false);
        attackLocalPosition = attackArea.transform.localPosition;

        Initialize(); // 임시
    }

    private void FixedUpdate()
    {
        StateUpdate(State);        
    }

    private void Update()
    {
        KeyUpdate();
        AnimationUpdate();        
    }

    private void Initialize()
    {
        Hp = maxHp;
        State = PlayerState.Idle;
    }

    private void KeyUpdate()
    {
        if (IsDead || isHit) return;

        if (input.IsAttack && !isAttacking && !isRolling)
        {
            State = PlayerState.Attack;
        }

        if (!isAttacking && !isRolling) //
        {
            if (input.InputVec.x != 0)
            {
                State = PlayerState.Move;
            }
            else
            {
                State = PlayerState.Idle;
            }
        }
    }

    private void AnimationUpdate()
    {
        if (IsDead) return;

        if(input.InputVec.x != 0.0f)
        {
            spriteRenderer.flipX = input.InputVec.x < 0f;
        }
    }

    private void StateStart(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Idle:
                IdleStateStart();
                break;
            case PlayerState.Move:
                MoveStateStart();
                break;
            case PlayerState.Roll:
                RollStateStart();
                break;
            case PlayerState.Hit:
                HitStateStart();
                break;
            case PlayerState.Attack:
                AttackStateStart();
                break;
            case PlayerState.Dead:
                DeadStateStart();
                break;
            default:
                break;
        }
    }

    private void StateUpdate(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Idle:
                IdleState();
                break;
            case PlayerState.Move:
                MoveState();
                break;
            case PlayerState.Roll:
                RollState();
                break;
            case PlayerState.Hit:
                HitState();
                break;
            case PlayerState.Attack:
                AttackState();
                break;
            case PlayerState.Dead:
                DeadState();
                break;
            default:
                break;
        }
    }

    private void StateEnd(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Idle:
                IdleStateEnd();
                break;
            case PlayerState.Move:
                MoveStateEnd();
                break;
            case PlayerState.Roll:
                RollStateEnd();
                break;
            case PlayerState.Hit:
                HitStateEnd();
                break;
            case PlayerState.Attack:
                AttackStateEnd();
                break;
            case PlayerState.Dead:
                DeadStateEnd();
                break;
            default:
                break;
        }
    }

    // State Update -------------------------------------------------------------------------------------------------------

    #region State Start
    private void IdleStateStart()
    {
        anim.Play("Idle", 0);
    }

    private void MoveStateStart()
    {
        anim.Play("Run", 0);
    }

    private void RollStateStart()
    {
        isRolling = true;
        anim.Play("Roll", 0);
        rigid2d.AddForce(input.InputVec * rollPower, ForceMode2D.Impulse);
    }

    private void HitStateStart()
    {
        isHit = true;
        OnHitPerformed?.Invoke();
        anim.Play("Hit", 0);
    }

    private void AttackStateStart()
    {
        isAttacking = true;
        attackArea.SetEnableCollider(true);
        AttackCooldown = MaxAttackCooldown;

        anim.Play("Attack" + attackCount, 0);
        attackCount++;
        if (attackCount > 3) attackCount = 1;

        UpdateAttackAreaPosition();
    }

    private void DeadStateStart()
    {
        anim.Play("Dead");
        OnDeadPerformed?.Invoke();
    }
    #endregion

    #region State Update
    private void IdleState()
    {
        rigid2d.linearVelocity = new Vector2(0f, rigid2d.linearVelocity.y);
    }

    private void MoveState()
    {
        rigid2d.linearVelocity = new Vector2(input.InputVec.x * speed, rigid2d.linearVelocity.y);

        if (input.IsRoll && !isRolling && !isAttacking) // 움직일 때 구르기
        {
            State = PlayerState.Roll;
        }
    }

    private void RollState()
    {
        if (CheckAnimationEnd())
        {
            State = PlayerState.Idle;
        }
    }

    private void HitState()
    {
        if (CheckAnimationEnd())
        {
            State = PlayerState.Idle;
        }

    }

    private void AttackState()
    {
        AttackCooldown -= Time.deltaTime;

        if (CheckAnimationEnd())
        {
            State = PlayerState.Idle;
        }
    }

    private void DeadState()
    {
        
    }
    #endregion

    #region State End

    private void IdleStateEnd()
    {
        
    }
    private void MoveStateEnd()
    {
        
    }
    private void RollStateEnd()
    {
        isRolling = false;
    }
    private void HitStateEnd()
    {
        isHit = false;
    }

    private void AttackStateEnd()
    {
        isAttacking = false;
        attackArea.SetEnableCollider(false);
    }

    private void DeadStateEnd()
    {
        // 사용 안함
    }
    #endregion

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

    private void UpdateAttackAreaPosition()
    {
        if (spriteRenderer.flipX) attackArea.transform.localPosition = new Vector3(-attackLocalPosition.x, attackLocalPosition.y, 0f);
        else attackArea.transform.localPosition = new Vector3(attackLocalPosition.x, attackLocalPosition.y, 0f);
    }
    #endregion
}