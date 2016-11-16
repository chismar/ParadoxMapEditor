using UnityEngine;
using System.Collections;

public abstract class MapMode : MonoBehaviour
{
    public Map Map;
    public MapRenderer Renderer;

    void Awake()
    {
        Renderer = FindObjectOfType<MapRenderer>();
    }
    
    public abstract void OnLeft(int x, int y);
    public virtual void OnRightStart(int x, int y) { }
    public virtual void OnRightStop(int x, int y) { }
    public virtual void OnRightClick(int x, int y) { }
    public virtual void OnRightDrag(int x, int y) { }

    public abstract void OnChoseMapMode();
}

