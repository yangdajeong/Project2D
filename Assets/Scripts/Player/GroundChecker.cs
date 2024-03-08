using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] LayerMask groundCheckLayer;
    private bool isGround;
    private int groundCount;
    public bool IsGround { get { return isGround; } }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (groundCheckLayer.Contain(collision.gameObject.layer))
        {
            {

                groundCount++;
                isGround = groundCount > 0;
                animator.SetBool("IsGround", isGround);

            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (groundCheckLayer.Contain(collision.gameObject.layer))
        {
            groundCount--;
            isGround = groundCount > 0;
            animator.SetBool("IsGround", isGround);
        }

    }
}
