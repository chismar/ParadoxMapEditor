using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class Controller : MonoBehaviour
{
    public MapMode CurrentMapMode;

    bool loaded = false;
    MapLoader loader;
    void Start()
    {
        
        loader = FindObjectOfType<MapLoader>();
        loader.FinishedLoadingMap += OnMapLoad;
    }
    void OnMapLoad()
    {
        loader.FinishedLoadingMap -= OnMapLoad;
        loaded = true;
        SelectMapMode(GetComponent<ProvinceSelectionMapMode>()); CurrentMapMode.Map = loader.Map; EnableAllMapModes();
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            Application.Quit();
        if (Input.GetKeyUp(KeyCode.H))
            showCoords = !showCoords;
    }

    private void EnableAllMapModes()
    {
        var modes = GetComponents<MapMode>();
        foreach (var mode in modes)
        {

            mode.enabled = true;
            mode.Map = loader.Map;
        }
    }

    public void SelectMapMode(MapMode mode)
    {
        if (CurrentMapMode != null)
            CurrentMapMode.Disable();
        CurrentMapMode = mode;
        mode.Enable();
    }
    public void OnPointerDown(int x, int y, PointerEventData.InputButton button)
    {
        if (CurrentMapMode == null)
            return;
        if (button != PointerEventData.InputButton.Right)
            return;
        CurrentMapMode.OnRightStart(x, y);
    }

    public void OnPointerClick(int x, int y, PointerEventData.InputButton button)
    {
        //Debug.Log("click!");
        if (CurrentMapMode == null)
            return;
        if (button == PointerEventData.InputButton.Left)
            CurrentMapMode.OnLeft(x, y);
        else if (button == PointerEventData.InputButton.Right)
            CurrentMapMode.OnRightClick(x, y);
    }

    public void OnPointerUp(int x, int y, PointerEventData.InputButton button)
    {
        if (button != PointerEventData.InputButton.Right)
            return;
        if (CurrentMapMode == null)
            return;
        CurrentMapMode.OnRightStop(x, y);
    }

    internal void OnDrag(int x, int y, PointerEventData.InputButton button)
    {
        if (button != PointerEventData.InputButton.Right)
            return;
        if (CurrentMapMode == null)
            return;
        CurrentMapMode.OnRightDrag(x, y);
    }
    int curTileX = 0;
    int curTileY = 0;
    bool showCoords = false;

    public void UpdatePointer(int x, int y)
    {
        curTileX = x;
        curTileY = y;
    }
    private void OnGUI()
    {
        if(loaded)
        if(showCoords)
        {
            int curX = (int)Input.mousePosition.x;
            int curY = Screen.height - (int)Input.mousePosition.y;
            int offsetX = 50;
            int offsetY = 20;
            GUI.Label(Rect.MinMaxRect(curX + 20, curY, curX + 50 +  offsetX, curY + offsetY), "X: " + curTileX);
            GUI.Label(Rect.MinMaxRect(curX + 50 + offsetX, curY, curX + 50 +  offsetX * 2, curY + offsetY), "Y: " + (loader.Map.Height - curTileY));

        }
    }
}
