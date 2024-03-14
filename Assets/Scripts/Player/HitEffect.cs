using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HitEffect : MonoBehaviour
{
    [SerializeField] PlayerAttack playerAttack;
    [SerializeField] ObjectPool pooler;
    [SerializeField] float Speed;
    [SerializeField] Camera mainCamera;

    private void OnClickAttack(InputValue value)
    {
        ClickAttack();
    }

    private void ClickAttack()
    {
        // 카메라가 없으면 현재 활성화된 메인 카메라를 찾음
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // 화면의 가장자리에 위치한 지점을 가져옴
        Vector3 screenBorder = new Vector3(Screen.width, Screen.height, 0f);
        Vector3 worldBorder = mainCamera.ScreenToWorldPoint(screenBorder);

        // Object Pool에서 오브젝트를 가져옴
        PooledObject instance = pooler.GetPool(worldBorder, Quaternion.identity);

        // 오브젝트의 Rigidbody2D 컴포넌트를 가져옴
        Rigidbody2D rb = instance.GetComponent<Rigidbody2D>();

        // PlayerAttack의 DirVec 방향대로 힘을 가함
        if (rb != null)
        {
            instance.transform.right = (Vector3)playerAttack.DirVec.normalized;
            rb.AddForce(playerAttack.DirVec.normalized * Speed, ForceMode2D.Impulse);
        }
    }
}