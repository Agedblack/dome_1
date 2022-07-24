using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject playerHandle;
    private GameObject cameraHandle;
    private GameObject player;
    private new GameObject camera;
    private float tempEulerX;
    private Vector3 cameraDampVelocity;


    private void Awake()
    {
        cameraHandle = transform.parent.gameObject;
        playerHandle = cameraHandle.transform.parent.gameObject;
        tempEulerX = 20;
        player = playerHandle.GetComponent<PlayerMove>().player;
        camera = Camera.main.gameObject;
        Cursor.visible = false;//隐藏指针
        Cursor.lockState = CursorLockMode.Locked; //将光标锁定到游戏窗口的中心
    }
    private void Update()
    {
        
    }
    private void FixedUpdate()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");
        Vector3 tempMpdelEuler = player.transform.eulerAngles;
        playerHandle.transform.Rotate(Vector3.up, mouseX * 80.0f * Time.fixedDeltaTime);
        tempEulerX -= mouseY * 80.0f * Time.fixedDeltaTime;
        tempEulerX = Mathf.Clamp(tempEulerX, -20, 30);
        cameraHandle.transform.localEulerAngles = new Vector3(tempEulerX, 0, 0);
        player.transform.eulerAngles = tempMpdelEuler;
        camera.transform.position = Vector3.SmoothDamp(camera.transform.position, transform.position, ref cameraDampVelocity, 0.2f);
        camera.transform.eulerAngles = transform.eulerAngles;
    }
}
