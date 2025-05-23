using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shooter : MonoBehaviour
{
    [Header("���̾ ����")]
    public GameObject fireballPrefab;
    public float shotSpeed = 10f;

    [Header("Image Target ����")]
    public Transform imageTarget; // Image Target Transform
    public Vector3 localShootDirection = Vector3.up; // Image Target ���� Y����

    [Header("�����")]
    public bool showDebugLog = true;

    private Mouse mouse;

    void Start()
    {
        // Input System �ʱ�ȭ
        mouse = Mouse.current;

        // Image Target �ڵ� ã�� (�Ҵ� �ȵǾ� ���� ���)
        if (imageTarget == null)
        {
            // �θ𿡼� Image Target ã��
            Transform parent = transform.parent;
            while (parent != null)
            {
                if (parent.name.Contains("ImageTarget") || parent.GetComponent<Vuforia.ObserverBehaviour>() != null)
                {
                    imageTarget = parent;
                    break;
                }
                parent = parent.parent;
            }

            if (imageTarget != null)
            {
                Debug.Log($"Image Target �ڵ� ã��: {imageTarget.name}");
            }
            else
            {
                Debug.LogWarning("Image Target�� ã�� �� �����ϴ�! Inspector���� �������� �Ҵ����ּ���.");
            }
        }

        // �ʼ� ������Ʈ üũ
        if (fireballPrefab == null)
        {
            Debug.LogError("fireballPrefab�� �Ҵ���� �ʾҽ��ϴ�!");
        }
        else
        {
            // Prefab�� Rigidbody�� �ִ��� Ȯ��
            Rigidbody prefabRb = fireballPrefab.GetComponent<Rigidbody>();
            if (prefabRb == null)
            {
                Debug.LogWarning("fireballPrefab�� Rigidbody�� �����ϴ�. �ڵ����� �߰��˴ϴ�.");
            }
        }
    }

    void Update()
    {
        // ȸ��
        transform.Rotate(Vector3.forward, 1.0f);

        // ���콺 �Է� ó�� (New Input System)
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            if (showDebugLog) Debug.Log("���콺 Ŭ�� ����!");
            Shoot();
        }
    }

    public void Shoot()
    {
        if (fireballPrefab == null)
        {
            Debug.LogError("fireballPrefab�� null�Դϴ�!");
            return;
        }

        if (imageTarget == null)
        {
            Debug.LogError("Image Target�� �������� �ʾҽ��ϴ�!");
            return;
        }

        if (showDebugLog) Debug.Log("���̾ �߻�!");

        // ���̾ ����
        GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);

        // Rigidbody Ȯ�� �� �߰�
        Rigidbody rb = fireball.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = fireball.AddComponent<Rigidbody>();
            if (showDebugLog) Debug.Log("Rigidbody �ڵ� �߰���");
        }

        // Image Target �������� ���� ���
        // Image Target�� ���� ��ǥ�迡�� Y������ ���� ��ǥ��� ��ȯ
        Vector3 shootDirection = imageTarget.TransformDirection(localShootDirection);

        // �� ���ϱ�
        rb.AddForce(shootDirection.normalized * shotSpeed, ForceMode.Impulse);

        if (showDebugLog)
        {
            Debug.Log($"���̾ �߻�!");
            Debug.Log($"Image Target ȸ��: {imageTarget.rotation.eulerAngles}");
            Debug.Log($"�߻� ���� (����): {shootDirection}");
            Debug.Log($"�߻� ���� (����): {localShootDirection}");
        }

        // 2�� �� ����
        Destroy(fireball, 2.0f);
    }

    // �׽�Ʈ�� - �����̽��ٷε� �߻� ����
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 30), "�߻� �׽�Ʈ"))
        {
            Shoot();
        }

        GUI.Label(new Rect(10, 50, 200, 20), "��Ŭ�� �Ǵ� ��ư���� �߻�");
    }
}