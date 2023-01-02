using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInput : MonoBehaviour
{
    public float horizontal { get; private set; }
    public float vertical { get; private set; }
    public float scroll { get; private set; }
    public bool keyR { get; private set; }
    public bool keyX { get; private set; }

    public delegate void clickDelegate(Vector3 pos);
    public event clickDelegate OnMouseLeftClick;
    public event clickDelegate OnMouseRightClick;

    private void Awake()
    {
        horizontal = 0f;
        vertical = 0f;
        scroll = 0f;
        keyR = false;
        keyX = false;
    }

    private void Update()
    {
        if(GameManager.instance.gameState != GameManager.GameState.Playing)
        {
            return;
        }

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        keyR = Input.GetKeyDown(KeyCode.R);
        keyX = Input.GetKeyDown(KeyCode.X);

        // 마우스가 UI 위에 있을 경우 Player Input으로 인식하지 않도록 조치
        if (EventSystem.current.IsPointerOverGameObject()) 
        {
            return;
        }

        scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickPos.z = 0;

            OnMouseLeftClick(clickPos);
        }
        else if(Input.GetMouseButtonDown(1))
        {
            Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickPos.z = 0;

            OnMouseRightClick(clickPos);
        }
    }
}