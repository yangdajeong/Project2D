using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;

    private float shakeTime;
    private float shakeIntensity;

    // 외부에서 호출하여 카메라를 흔들도록 하는 메서드
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
