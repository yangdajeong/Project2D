using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallChecker : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] LayerMask WallCheckLayer;
    private bool isWall;
    private int groundCount;
    public bool IsWall { get { return isWall; } }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (WallCheckLayer.Contain(collision.gameObject.layer))
        {
            {
                groundCount++;
                isWall = groundCount > 0;
                animator.SetBool("IsWall", isWall);

            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (WallCheckLayer.Contain(collision.gameObject.layer))
        {
            groundCount--;
            isWall = groundCount > 0;
            animator.SetBool("IsWall", isWall);
        }

    }
}
