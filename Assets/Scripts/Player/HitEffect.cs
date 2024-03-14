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
        // ī�޶� ������ ���� Ȱ��ȭ�� ���� ī�޶� ã��
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // ȭ���� �����ڸ��� ��ġ�� ������ ������
        Vector3 screenBorder = new Vector3(Screen.width, Screen.height, 0f);
        Vector3 worldBorder = mainCamera.ScreenToWorldPoint(screenBorder);

        // Object Pool���� ������Ʈ�� ������
        PooledObject instance = pooler.GetPool(worldBorder, Quaternion.identity);

        // ������Ʈ�� Rigidbody2D ������Ʈ�� ������
        Rigidbody2D rb = instance.GetComponent<Rigidbody2D>();

        // PlayerAttack�� DirVec ������ ���� ����
        if (rb != null)
        {
            instance.transform.right = (Vector3)playerAttack.DirVec.normalized;
            rb.AddForce(playerAttack.DirVec.normalized * Speed, ForceMode2D.Impulse);
        }
    }
}