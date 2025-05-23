using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shooter : MonoBehaviour
{
    [Header("파이어볼 설정")]
    public GameObject fireballPrefab;
    public float shotSpeed = 10f;

    [Header("Image Target 설정")]
    public Transform imageTarget; // Image Target Transform
    public Vector3 localShootDirection = Vector3.up; // Image Target 기준 Y방향

    [Header("디버그")]
    public bool showDebugLog = true;

    private Mouse mouse;

    void Start()
    {
        // Input System 초기화
        mouse = Mouse.current;

        // Image Target 자동 찾기 (할당 안되어 있을 경우)
        if (imageTarget == null)
        {
            // 부모에서 Image Target 찾기
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
                Debug.Log($"Image Target 자동 찾음: {imageTarget.name}");
            }
            else
            {
                Debug.LogWarning("Image Target을 찾을 수 없습니다! Inspector에서 수동으로 할당해주세요.");
            }
        }

        // 필수 컴포넌트 체크
        if (fireballPrefab == null)
        {
            Debug.LogError("fireballPrefab이 할당되지 않았습니다!");
        }
        else
        {
            // Prefab에 Rigidbody가 있는지 확인
            Rigidbody prefabRb = fireballPrefab.GetComponent<Rigidbody>();
            if (prefabRb == null)
            {
                Debug.LogWarning("fireballPrefab에 Rigidbody가 없습니다. 자동으로 추가됩니다.");
            }
        }
    }

    void Update()
    {
        // 회전
        transform.Rotate(Vector3.forward, 1.0f);

        // 마우스 입력 처리 (New Input System)
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            if (showDebugLog) Debug.Log("마우스 클릭 감지!");
            Shoot();
        }
    }

    public void Shoot()
    {
        if (fireballPrefab == null)
        {
            Debug.LogError("fireballPrefab이 null입니다!");
            return;
        }

        if (imageTarget == null)
        {
            Debug.LogError("Image Target이 설정되지 않았습니다!");
            return;
        }

        if (showDebugLog) Debug.Log("파이어볼 발사!");

        // 파이어볼 생성
        GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);

        // Rigidbody 확인 및 추가
        Rigidbody rb = fireball.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = fireball.AddComponent<Rigidbody>();
            if (showDebugLog) Debug.Log("Rigidbody 자동 추가됨");
        }

        // Image Target 기준으로 방향 계산
        // Image Target의 로컬 좌표계에서 Y방향을 월드 좌표계로 변환
        Vector3 shootDirection = imageTarget.TransformDirection(localShootDirection);

        // 힘 가하기
        rb.AddForce(shootDirection.normalized * shotSpeed, ForceMode.Impulse);

        if (showDebugLog)
        {
            Debug.Log($"파이어볼 발사!");
            Debug.Log($"Image Target 회전: {imageTarget.rotation.eulerAngles}");
            Debug.Log($"발사 방향 (월드): {shootDirection}");
            Debug.Log($"발사 방향 (로컬): {localShootDirection}");
        }

        // 2초 후 제거
        Destroy(fireball, 2.0f);
    }

    // 테스트용 - 스페이스바로도 발사 가능
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 30), "발사 테스트"))
        {
            Shoot();
        }

        GUI.Label(new Rect(10, 50, 200, 20), "좌클릭 또는 버튼으로 발사");
    }
}