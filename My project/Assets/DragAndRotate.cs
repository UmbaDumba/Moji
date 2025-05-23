using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ARDragAndRotate : MonoBehaviour
{
    [Header("회전 설정")]
    public float rotateSpeed = 150.0f;

    private bool rotating = false;
    private Vector2 lastInputPos;
    private Camera arCamera;

    // New Input System 변수들
    private Mouse mouse;
    private Touchscreen touchscreen;

    void Start()
    {
        // AR 카메라 찾기
        arCamera = Camera.main;
        if (arCamera == null)
        {
            arCamera = FindObjectOfType<Camera>();
        }

        // Collider 확인 및 추가
        if (GetComponent<Collider>() == null)
        {
            Debug.LogWarning($"{gameObject.name}에 Collider가 없습니다. BoxCollider를 추가합니다.");
            gameObject.AddComponent<BoxCollider>();
        }

        // Input 디바이스 초기화
        mouse = Mouse.current;
        touchscreen = Touchscreen.current;
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        // PC 마우스 입력
        if (mouse != null)
        {
            if (mouse.leftButton.wasPressedThisFrame)
            {
                CheckForObjectHit(mouse.position.ReadValue());
            }
            else if (mouse.leftButton.wasReleasedThisFrame)
            {
                rotating = false;
            }
        }

        // 모바일 터치 입력
        if (touchscreen != null && touchscreen.primaryTouch.press.isPressed)
        {
            var touch = touchscreen.primaryTouch;

            if (touch.press.wasPressedThisFrame)
            {
                CheckForObjectHit(touch.position.ReadValue());
            }
            else if (touch.press.wasReleasedThisFrame)
            {
                rotating = false;
            }
        }

        // 회전 처리
        if (rotating)
        {
            Vector2 currentInputPos = GetCurrentInputPosition();
            RotateObject(currentInputPos);
        }
    }

    Vector2 GetCurrentInputPosition()
    {
        if (mouse != null && mouse.leftButton.isPressed)
        {
            return mouse.position.ReadValue();
        }
        else if (touchscreen != null && touchscreen.primaryTouch.press.isPressed)
        {
            return touchscreen.primaryTouch.position.ReadValue();
        }
        return Vector2.zero;
    }


    void CheckForObjectHit(Vector2 screenPosition)
    {
        Ray ray = arCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                rotating = true;
                lastInputPos = screenPosition;
                Debug.Log("오브젝트 선택됨: " + gameObject.name);
            }
        }
    }

    void RotateObject(Vector2 currentInputPos)
    {
        Vector2 offset = currentInputPos - lastInputPos;

        // 수평 회전 (Y축)
        float rotationY = -offset.x * Time.deltaTime * rotateSpeed;
        // 수직 회전 (X축)
        float rotationX = offset.y * Time.deltaTime * rotateSpeed;

        // 월드 공간에서 회전 (AR에서 더 자연스러움)
        transform.Rotate(Vector3.up, rotationY, Space.World);
        transform.Rotate(Vector3.right, rotationX, Space.World);

        lastInputPos = currentInputPos;
    }
}