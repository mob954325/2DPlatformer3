using UnityEngine;

public class Spike : MonoBehaviour
{
    AttackArea attackArea;

    private float delayTimer = 0f;
    private float maxDelay = 1f;

    private void Awake()
    {
        attackArea = GetComponent<AttackArea>();
    }

    private void Update()
    {
        if(attackArea.TargetList.Count > 0 && delayTimer <= 0f)
        {
            foreach(IDamageable target in attackArea.TargetList)
            {
                if(target as Player)
                {
                    target.TakeDamage(1);
                    delayTimer = maxDelay;
                }
            }
        }

        if (delayTimer > 0f) delayTimer -= Time.deltaTime;
    }
}
