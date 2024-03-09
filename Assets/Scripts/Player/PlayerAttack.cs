using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] GameObject Weapon;

    private void OnClickAttack(InputValue value)
    {
        ClickAttack();
    }

    private void ClickAttack()
    {
        Weapon.SetActive(true);
        animator.SetTrigger("IsSlash");
    }

    public void DeactivateWeapon()
    {
        Weapon.SetActive(false);
    }
}
