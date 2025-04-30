using UnityEditor;
using UnityEngine;

public class PlayerAttack : StateBase
{
    Player player;

    [SerializeField] private int attackCount = 1;
    private int maxAttackCount = 3;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    public override void StateEnter()
    {
        player.MoveStop();
        player.PlayAnimation("Attack" + attackCount);

        IDamageable target = player.ActiveAttackArea(); // NOTE 이거 작동 순서 꼬임
        player.OnAttack(target);
    }

    public override void StateExit()
    {
        attackCount = 1;
        player.DeactiveAttackArea();
    }

    public override void StateUpdate()
    {

        if(player.CheckAnimationEnd())
        {
            if(!player.Input.IsAttack)
            {
                player.SetActionState(PlayerActionState.None);
            }
            else
            {
                if(attackCount == maxAttackCount + 1)
                {
                    attackCount = 1;
                }

                player.PlayAnimation("Attack" + attackCount++);
            }
        }
    }

    public override void StateFixedUpdate()
    {
    }
}
