using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTest : EnemyBase
{
    private void OnEnable()
    {
        Initialize();
        OnHitPerformed += OnTestEnemyHit;        
    }

    private void OnDisable()
    {
        OnHitPerformed -= OnTestEnemyHit;        
    }

    protected override void BeforeSpawn()
    {
        //OnHitPerformed += OnTestEnemyHit;
    }

    protected override void BeforeDespawn()
    {
        //OnHitPerformed -= OnTestEnemyHit;
    }

    private void OnTestEnemyHit()
    {
        StopAllCoroutines();
        StartCoroutine(ColorChangeProcess());
    }

    IEnumerator ColorChangeProcess()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        spriteRenderer.color = Color.white;
    }
}
