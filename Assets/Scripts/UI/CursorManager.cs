using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D cursorGeneral;
    [SerializeField] private Texture2D cursorRopeable;
    [SerializeField] private Texture2D cursorNotRopeable;

    [SerializeField] private CameraMover cameraMover;
    [SerializeField] private PlayerRopeShooter playerRopeShooter;

    private Vector2 cursorHotspotCenter;
    private Vector2 cursorHotspotUpperleft;

    private Vector2 cursorPos;

    private void Start()
    {
        cursorHotspotCenter = new Vector2(cursorGeneral.width / 2, cursorGeneral.height / 2);
        cursorHotspotUpperleft = new Vector2(0f, 0f);
    }

    private void Update()
    {
        SetCursor();
    }

    private void SetCursor()
    {
        cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (GameManager.instance.gameState != GameManager.GameState.Playing)
        {
            SetCursorGeneral();
            return;
        }

        switch (playerRopeShooter.PerformRaycast(cursorPos))
        {
            case PlayerRopeShooter.RaycastResult.NoObject:
            case PlayerRopeShooter.RaycastResult.NotRopeableObject:
                SetCursorNotRopeable();
                break;

            case PlayerRopeShooter.RaycastResult.RopeableObject:
                SetCursorRopeable();
                break;
        }
    }

    private void SetCursorGeneral()
    {
        Cursor.SetCursor(cursorGeneral, cursorHotspotUpperleft, CursorMode.Auto);
    }

    private void SetCursorRopeable()
    {
        Cursor.SetCursor(cursorRopeable, cursorHotspotCenter, CursorMode.Auto);
    }

    private void SetCursorNotRopeable()
    {
        Cursor.SetCursor(cursorNotRopeable, cursorHotspotCenter, CursorMode.Auto);
    }
}
