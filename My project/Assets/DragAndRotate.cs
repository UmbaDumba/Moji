using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ARDragAndRotate : MonoBehaviour
{
    [Header("ȸ�� ����")]
    public float rotateSpeed = 150.0f;

    private bool rotating = false;
    private Vector2 lastInputPos;
    private Camera arCamera;

    // New Input System ������
    private Mouse mouse;
    private Touchscreen touchscreen;

    void Start()
    {
        // AR ī�޶� ã��
        arCamera = Camera.main;
        if (arCamera == null)
        {
            arCamera = FindObjectOfType<Camera>();
        }

        // Collider Ȯ�� �� �߰�
        if (GetComponent<Collider>() == null)
        {
            Debug.LogWarning($"{gameObject.name}�� Collider�� �����ϴ�. BoxCollider�� �߰��մϴ�.");
            gameObject.AddComponent<BoxCollider>();
        }

        // Input ����̽� �ʱ�ȭ
        mouse = Mouse.current;
        touchscreen = Touchscreen.current;
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        // PC ���콺 �Է�
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

        // ����� ��ġ �Է�
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

        // ȸ�� ó��
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
                Debug.Log("������Ʈ ���õ�: " + gameObject.name);
            }
        }
    }

    void RotateObject(Vector2 currentInputPos)
    {
        Vector2 offset = currentInputPos - lastInputPos;

        // ���� ȸ�� (Y��)
        float rotationY = -offset.x * Time.deltaTime * rotateSpeed;
        // ���� ȸ�� (X��)
        float rotationX = offset.y * Time.deltaTime * rotateSpeed;

        // ���� �������� ȸ�� (AR���� �� �ڿ�������)
        transform.Rotate(Vector3.up, rotationY, Space.World);
        transform.Rotate(Vector3.right, rotationX, Space.World);

        lastInputPos = currentInputPos;
    }
}