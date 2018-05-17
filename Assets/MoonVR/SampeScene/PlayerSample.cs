using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSample : MonoBehaviour {

    public float moveSpeed = 5;
    public float rotateSpeed = 30;
    private bool moonGravity = true;

    private void Start()
    {
        GravityMenu.SetMoonGravity();
    }

    private void Update()
    {
        // 回転操作
        if (GvrControllerInput.ClickButtonDown)
        {
            Vector2 touchPos = GvrControllerInput.TouchPos;
            if (touchPos.x > 0.75f)
            {
                rotate(true);
            }
            else if (touchPos.x < 0.25f)
            {
                rotate(false);
            }
        }

        // 前進後退操作
        if (GvrControllerInput.ClickButton)
        {
            Vector2 touchPos = GvrControllerInput.TouchPos;
            if (touchPos.y < 0.25f)
            {
                move(true);
            }
            else if (touchPos.y > 0.75f)
            {
                move(false);
            }
        }

        // 重力変更
        if (GvrControllerInput.AppButtonDown)
        {
            moonGravity = !moonGravity;
            if (moonGravity)
            {
                GravityMenu.SetMoonGravity();
            }
            else
            {
                GravityMenu.SetEarthGravity();
            }
        }
    }

    private void rotate(bool right)
    {
        transform.Rotate(Vector3.up * rotateSpeed * (right? 1: -1));
    }

    private void move(bool foward)
    {
        transform.position += Camera.main.transform.forward * moveSpeed * Time.deltaTime * (foward ? 1 : -1);
    }
}
