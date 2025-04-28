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
        Debug.Log("Player Attack Enter");
        player.PlayAnimation("Attack" + attackCount);
    }

    public override void StateExit()
    {
        Debug.Log("Player Attack Exit");
        attackCount = 1;
    }

    public override void StateUpdate()
    {
        Debug.Log("Player Attack Update");

        if(player.CheckAnimationEnd())
        {
            if(!player.Input.IsAttack)
            {
                player.SetStateIdle();
            }
            else
            {
                attackCount = attackCount == maxAttackCount + 1 ? 1 : ++attackCount;
                player.PlayAnimation("Attack" + attackCount);
            }
        }
    }

    public override void StateFixedUpdate()
    {
        // 사용 안함
    }
}
