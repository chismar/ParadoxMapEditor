using UnityEngine;
using System.Collections;
using System;

public class ProvinceSelectionMapMode : MapMode
{

    Province selectedProvince;

    float intermediateBrushSize = 3;
    int brushSize = 3;
    public RectTransform Border;
    static MapModesAndControls controls;
    static Controller controller;
    UnityEngine.UI.Text provinceSelection;
    UnityEngine.UI.InputField provinceSearch;
    UnityEngine.UI.Text brushSizeText;
    void OnEnable()
    {
        if (controls == null && controller == null)
        {
            controls = FindObjectOfType<MapModesAndControls>();
            controller = FindObjectOfType<Controller>();
        }
        controls.RegisterCallback(() => controller.SelectMapMode(this), "Province selection");

    }
    int findProvince;
    void Update()
    {
        if (enabled)
        {
            var scroll = Input.mouseScrollDelta.y;
            if (scroll > 0)
            {
                brushSize = (int)(brushSize * (1 + scroll));
            }
            intermediateBrushSize += scroll;
            var newBrush = (int)intermediateBrushSize;
            if (newBrush != brushSize)
                brushSizeText.text = "Brush size: " + newBrush;
            brushSize = newBrush;
            if (brushSize <= 0)
            {
                brushSize = 1;
                intermediateBrushSize = 1f;
                brushSizeText.text = "Brush size: " + brushSize;
            }

            float scale = brushSize / Border.sizeDelta.x;
            Border.localScale = new Vector3(scale, scale, 1) * Renderer.transform.localScale.x;
            Border.position = Input.mousePosition;
            if(int.TryParse(provinceSearch.text, out findProvince))
            {
                if(selectedProvince == null || selectedProvince.ID != findProvince)
                {
                    Province p = null;
                    Map.ProvincesByID.TryGetValue(findProvince, out p);
                    if(p != null)
                    {
                        selectedProvince = p;
                        Renderer.LitUpProvince(selectedProvince);
                        provinceSelection.text = "Province selected: " + selectedProvince.ID;
                        Camera.main.transform.position = new Vector3(selectedProvince.Anchor.x - Renderer.chunkSize/2, selectedProvince.Anchor.y - Renderer.chunkSize / 2, -10);
                    }
                }
            }
        }

    }

    public override void Enable()
    {
        base.Enable();
        Border.gameObject.SetActive(true);
        selectedProvince = null;
        brushSizeText = dataPanel.PostString("Brush size: " + brushSize.ToString());
        provinceSelection = dataPanel.PostString("Province not selected yet");
        provinceSearch = dataPanel.PostInput("FindProvince");
    }

    public override void Disable()
    {
        base.Disable();
        Border.gameObject.SetActive(false);
        Destroy(brushSizeText.gameObject);
        Destroy(provinceSelection.gameObject);
        Destroy(provinceSearch.gameObject);
    }
    public override void OnLeft(int x, int y)
    {
        selectedProvince = Map.Tiles[x, y].Province;
        Renderer.LitUpProvince(selectedProvince);
        provinceSelection.text = "Province selected: " + selectedProvince.ID;

    }
    public override void OnRightClick(int x, int y)
    {
        OnRightDrag(x, y);
    }
    public override void OnRightDrag(int x, int y)
    {
        if (selectedProvince == null)
            return;
        int leftX = x - (brushSize - 1) / 2;
        int rightX = x + brushSize / 2;
        int lowY = y - (brushSize - 1) / 2;
        int highY = y + brushSize / 2;
        for (int i = leftX; i <= rightX; i++)
        {
            for (int j = lowY; j <= highY; j++)
            {
                if(i < Map.Width && i >= 0 && j < Map.Height && j >= 0)
                Map.AssignTileTo(i, j, selectedProvince);
            }
        }
        for (int i = leftX - 1; i <= rightX + 1; i++)
            for (int j = lowY - 1; j <= highY + 1; j++)
                if (i < Map.Width && i >= 0 && j < Map.Height && j >= 0)
                    Renderer.Update(i, j);
    }
}
