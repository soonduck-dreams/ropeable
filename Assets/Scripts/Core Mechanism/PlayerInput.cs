using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInput : MonoBehaviour
{
    public float horizontal { get; private set; }
    public float vertical { get; private set; }
    public float scroll { get; private set; }

    public delegate void clickDelegate(Vector3 pos);
    public event clickDelegate OnMouseLeftClick;
    public event clickDelegate OnMouseRightClick;

    private void Awake()
    {
        horizontal = 0f;
        vertical = 0f;
        scroll = 0f;
    }

    private void Update()
    {
        if(GameManager.instance.gameState != GameManager.GameState.Playing)
        {
            return;
        }

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        // 마우스가 'BlockPlayerInput' 태그된 UI 위에 있을 경우 Player Input 마우스 이벤트 차단
        if (BlockPlayerInput()) 
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

    private bool BlockPlayerInput()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (var r in results)
            {
                if (r.gameObject.tag == "BlockPlayerInput")
                {
                    return true;
                }
            }
        }

        return false;
    }
}