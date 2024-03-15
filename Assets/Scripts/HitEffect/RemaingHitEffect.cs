using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RemaingHitEffect : MonoBehaviour
{

    [SerializeField] Image effetImage;
    [SerializeField] float maxTime;
    private float timeRemaining;

    private void Start()
    {
        timeRemaining = maxTime;
    }

    private void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            effetImage.fillAmount = timeRemaining / maxTime;
        }
    }

}
