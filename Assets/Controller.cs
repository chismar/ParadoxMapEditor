using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class Controller : MonoBehaviour
{
    public MapMode CurrentMapMode;
    

    void Start()
    {
        
        var loader = FindObjectOfType<MapLoader>();
        loader.FinishedLoadingMap += () => { CurrentMapMode = gameObject.AddComponent<ProvinceSelectionMapMode>(); CurrentMapMode.Map = loader.Map; };
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
        if (CurrentMapMode == null)
            return;
        if (button == PointerEventData.InputButton.Left)
            CurrentMapMode.OnLeft(x, y);
        else if (button == PointerEventData.InputButton.Right)
            CurrentMapMode.OnRightClick(x, y);
    }

    public void OnPointerUp(int x, int y, PointerEventData.InputButton button)
    {
        if (CurrentMapMode == null)
            return;
        CurrentMapMode.OnRightStop(x, y);
    }

    internal void OnDrag(int x, int y, PointerEventData.InputButton button)
    {
        if (CurrentMapMode == null)
            return;
        CurrentMapMode.OnRightDrag(x, y);
    }
}
