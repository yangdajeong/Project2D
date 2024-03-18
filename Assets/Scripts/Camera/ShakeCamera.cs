using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;

    private float shakeTime;
    private float shakeIntensity;

    // �ܺο��� ȣ���Ͽ� ī�޶� ��鵵�� �ϴ� �޼���
    public void Shake()
    {
        shakeTime = 0.2f;
        shakeIntensity = 18f;

        StartCoroutine(ShakeByPosition());
    }

    private IEnumerator ShakeByPosition()
    {
        virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = shakeIntensity;
        yield return new WaitForSeconds(shakeTime);
        virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
    }
}
