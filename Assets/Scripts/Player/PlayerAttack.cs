using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] Animator WeaponAnimator;
    [SerializeField] Animator PlayerAnimator;
    [SerializeField] SpriteRenderer Weapon;
    [SerializeField] Transform WeaponTransform;

    private Camera _camera;

    private void Update()
    {
        Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dirVec = mousePos - (Vector2)transform.position; //마우스가 바라보는 방향을 나타내는 벡터

        WeaponTransform.transform.right = (Vector3)dirVec.normalized;

        Vector2 scale = transform.localScale;
        if (dirVec.x < 0)
        {
            scale.y = -1;
        }
        else if (dirVec.x > 0)
        {
            scale.y = 1;
        }
        WeaponTransform.transform.localScale = scale;
    }


    private void OnClickAttack(InputValue value)
    {
        ClickAttack();
    }

    private void ClickAttack()
    {

        WeaponAnimator.SetTrigger("IsSlash");


        PlayerAnimator.SetBool("IsAttack", true);

        Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dirVec = mousePos - (Vector2)transform.position; //마우스가 바라보는 방향을 나타내는 벡터

        transform.Translate(dirVec.normalized);

    }

    public void NotIsAttack()
    {
        PlayerAnimator.SetBool("IsAttack", false);
    }
}
