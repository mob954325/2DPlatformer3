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
        //Debug.Log("Player Attack Enter");
        player.MoveStop();
        player.PlayAnimation("Attack" + attackCount);
    }

    public override void StateExit()
    {
        //Debug.Log("Player Attack Exit");
        attackCount = 1;
    }

    public override void StateUpdate()
    {
        //Debug.Log("Player Attack Update");

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
        // 사용 안함
    }
}
