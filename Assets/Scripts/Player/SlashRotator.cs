using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlashRotator : MonoBehaviour
{



    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }



    private void Update()
    {
        Vector2 mousePos = (Vector2)_camera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dirVec = mousePos - (Vector2)transform.position; //마우스가 바라보는 방향을 나타내는 벡터

        transform.right = (Vector3)dirVec.normalized;

        Vector2 scale = transform.localScale;
        if(dirVec.x < 0 )
        {
            scale.y = -1;
        }
        else if (dirVec.x > 0 ) 
        {
            scale.y = 1;
        }
        transform.localScale = scale;
    }




}

//private void OnEnable()
//{
//    Cursor.lockState = CursorLockMode.Confined;
//}

//private void OnDisable()
//{
//    Cursor.lockState = CursorLockMode.None;
//}
