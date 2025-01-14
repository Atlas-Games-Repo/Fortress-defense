using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour, IKeyboardCall
{
    public void KeyDown(KeyCode keyCode)
    {
        Move();
    }
    public KeyCode[] KeyType { get { return new KeyCode[] { KeyCode.Space }; } }
    public int KeyObjectID { get { return gameObject.GetInstanceID(); } }
    public float limitLeft = -6;
    public float limitRight = 1000;

    public float moveSpeed = 2;
    public float distanceScale = 1;
    float beginX;
    float beginCamreaPosX;
    bool isDragging = false;
    Vector3 target = new Vector3(-1, 0, 0);
    bool allowWorking = false;
    public Vector3 target_right = new Vector3(-10, 0, 0);
    public Vector3 target_left = new Vector3(-1, 0, 0);
    public bool allowTouch = false, is_left = true;
    public Image CameraMove;
    public Sprite Left, Right;

    IEnumerator Start()
    {
        yield return null;
        beginCamreaPosX = transform.position.x;
        target = transform.position;
        target.x = Mathf.Clamp(transform.position.x, limitLeft + CameraHalfWidth, limitRight - CameraHalfWidth);
        allowWorking = true;
        Move();
    }

    public void Move()
    {
        float left_camera = Camera.main.ScreenToWorldPoint(Vector2.zero).x;
        float right_camera = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).x;
        right_camera = Mathf.Abs(limitRight) - Mathf.Abs(right_camera);
        left_camera = Mathf.Abs(limitLeft) - Mathf.Abs(left_camera);
        if (is_left) // is moving to right
        {
            target_right.x = transform.position.x + right_camera;
            target = target_right;
            CameraMove.sprite = Right;
            is_left = false;
        }
        else // is moving to left
        {
            target_left.x = transform.position.x - left_camera;
            target = target_left;
            CameraMove.sprite = Left;
            is_left = true;
        }
    }
    void Update()
    {
        if (!allowTouch)
        {
            transform.position = Vector3.Lerp(transform.position, target, moveSpeed * Time.fixedDeltaTime);
            return;
        }
        if (!allowWorking)
            return;
        transform.position = Vector3.Lerp(transform.position, target, moveSpeed * Time.fixedDeltaTime);
        if (GameManager.Instance.State != GameManager.GameState.Playing)
            return;
        if (!isDragging)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                beginX = Input.mousePosition.x;
                beginCamreaPosX = transform.position.x;
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }
            else
            {
                target = new Vector3(beginCamreaPosX + (beginX - Input.mousePosition.x) * distanceScale * 0.01f, transform.position.y, transform.position.z);

                target.x = Mathf.Clamp(target.x, limitLeft + CameraHalfWidth, limitRight - CameraHalfWidth);
            }
        }
    }

    public float CameraHalfWidth
    {
        get { return (Camera.main.orthographicSize * ((float)Screen.width / (float)Screen.height)); }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.color = Color.yellow;
        Vector2 boxSize = new Vector2(limitRight - limitLeft, Camera.main.orthographicSize * 2);
        Vector2 center = new Vector2((limitRight + limitLeft) * 0.5f, transform.position.y);
        Gizmos.DrawWireCube(center, boxSize);
    }
}
