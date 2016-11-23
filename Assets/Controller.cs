﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class Controller : MonoBehaviour
{
    public MapMode CurrentMapMode;

    MapLoader loader;
    void Start()
    {
        
        loader = FindObjectOfType<MapLoader>();
        loader.FinishedLoadingMap += () => { SelectMapMode(GetComponent<ProvinceSelectionMapMode>()); CurrentMapMode.Map = loader.Map; EnableAllMapModes(); };
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            Application.Quit();
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
}
