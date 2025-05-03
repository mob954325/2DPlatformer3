using System;
using UnityEngine;

public class HitFx : MonoBehaviour, IPoolable
{
    Animator animator;

    public Action ReturnAction { get; set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(CheckAnimationEnd())
        {
            this.gameObject.SetActive(false);
        }
    }

    public bool CheckAnimationEnd()
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
    }

    public void OnSpawn()
    {
        
    }

    public void OnDespawn()
    {
        
    }
}
