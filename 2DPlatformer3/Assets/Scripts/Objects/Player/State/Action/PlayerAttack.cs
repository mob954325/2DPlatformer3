using System.Collections;
using UnityEngine;

public class PlayerAttack : StateBase
{
    Player player;
    WaitForFixedUpdate waitFixedUpdate;

    [SerializeField] private int attackCount = 1;
    private int maxAttackCount = 3;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
        waitFixedUpdate = new WaitForFixedUpdate();
    }

    public override void StateEnter()
    {
        player.MoveStop();
        player.PlayAnimation("Attack" + attackCount);
        player.ActiveAttackArea();
        StartCoroutine(AttackProcess());
    }

    public override void StateExit()
    {
        attackCount = 1;
        player.DeactiveAttackArea();
    }

    public override void StateUpdate()
    {
        if (player.CheckAnimationEnd())
        {
            if (!player.Input.IsAttack)
            {
                player.SetActionState(PlayerActionState.None);
            }
            else
            {
                if (attackCount == maxAttackCount + 1)
                {
                    attackCount = 1;
                }

                player.PlayAnimation("Attack" + attackCount++);
                StartCoroutine(AttackProcess());
            }
        }
    }

    public override void StateFixedUpdate()
    {
    }

    private IEnumerator AttackProcess()
    {
        yield return waitFixedUpdate;
        player.TryAttack();
    }
}
