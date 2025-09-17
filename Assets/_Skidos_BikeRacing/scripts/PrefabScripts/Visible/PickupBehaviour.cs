namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;

public class PickupBehaviour : MonoBehaviour
{

    Animator animator;

    void Awake()
    {
        animator = transform.parent.Find("Visual").GetComponent<Animator>();
    }

    public void MarkAsCollected()
    {
        animator.SetBool("collected", true);
    }

    public void Reset()
    {

        if (animator != null)
        {
            animator.SetBool("collected", false);
            animator.Play("Idle", -1, 0);
        }
    }

}

}
