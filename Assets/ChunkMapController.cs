using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class ChunkMapController : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler, IDragHandler
{
    RectTransform transform;
    public Texture2D TargetTexture;
    public int MapOffsetX;
    public int MapOffsetY;
    public int Size;
    static Controller Controller;

    void Start()
    {
        transform = GetComponent<RectTransform>();
        transform.sizeDelta = new Vector2(Size, Size);
        transform.position = new Vector3(MapOffsetX , MapOffsetY , 0);
        if (Controller == null)
            Controller = FindObjectOfType<Controller>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 localCursor;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out localCursor))
            return;
        localCursor = new Vector2(localCursor.x/ transform.sizeDelta.x + 0.5f, localCursor.y/transform.sizeDelta.y + 0.5f) * Size;
        int x = (int)localCursor.x;
        int y = (int)localCursor.y;
        
        Controller.OnPointerDown(MapOffsetX + x, MapOffsetY + y, eventData.button);
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        Vector2 localCursor;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out localCursor))
            return;
        localCursor = new Vector2(localCursor.x / transform.sizeDelta.x + 0.5f, localCursor.y / transform.sizeDelta.y + 0.5f) * Size;
        int x = (int)localCursor.x;
        int y = (int)localCursor.y;
        Controller.OnPointerClick(MapOffsetX + x, MapOffsetY + y, eventData.button);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Vector2 localCursor;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out localCursor))
            return;
        localCursor = new Vector2(localCursor.x / transform.sizeDelta.x + 0.5f, localCursor.y / transform.sizeDelta.y + 0.5f) * Size;
        int x = (int)localCursor.x;
        int y = (int)localCursor.y;
        Controller.OnPointerUp(MapOffsetX + x, MapOffsetY + y, eventData.button);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localCursor;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out localCursor))
            return;
        localCursor = new Vector2(localCursor.x / transform.sizeDelta.x + 0.5f, localCursor.y / transform.sizeDelta.y + 0.5f) * Size;
        int x = (int)localCursor.x;
        int y = (int)localCursor.y;
        Controller.OnDrag(MapOffsetX + x, MapOffsetY + y, eventData.button);
    }
}
