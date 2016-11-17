using UnityEngine;
using System.Collections;

public abstract class MapMode : MonoBehaviour
{
    public Map Map;
    public MapRenderer Renderer;
    protected bool enabled = false;
    static protected MapModeDataPanel dataPanel;
    public void Awake()
    {
        if(dataPanel == null)
            dataPanel = FindObjectOfType<MapModeDataPanel>();
        Renderer = FindObjectOfType<MapRenderer>();
    }
    
    public abstract void OnLeft(int x, int y);
    public virtual void OnRightStart(int x, int y) { }
    public virtual void OnRightStop(int x, int y) { }
    public virtual void OnRightClick(int x, int y) { }
    public virtual void OnRightDrag(int x, int y) { }
    UnityEngine.UI.Text nameText;
    public virtual void Enable() { enabled = true; nameText = dataPanel.PostString(this.GetType().Name); nameText.color = Color.yellow; }
    public virtual void Disable() { enabled = false; Destroy(nameText.gameObject); }
}

