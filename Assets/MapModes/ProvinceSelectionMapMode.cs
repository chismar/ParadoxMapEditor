using UnityEngine;
using System.Collections;
using System;

public class ProvinceSelectionMapMode : MapMode
{

    Province selectedProvince;

    int brushSize = 3;

    static MapModesAndControls controls;
    static Controller controller;
    UnityEngine.UI.Text provinceSelection;
    UnityEngine.UI.Text brushSizeText;
    void Awake()
    {
        base.Awake();
        if (controls == null && controller == null)
        {
            controls = FindObjectOfType<MapModesAndControls>();
            controller = FindObjectOfType<Controller>();
        }
        controls.RegisterCallback(() => controller.SelectMapMode(this), "Province selection");

    }

    void Update()
    {

    }

    public override void Enable()
    {
        base.Enable();
        selectedProvince = null;
        brushSizeText = dataPanel.PostString(brushSize.ToString());
        provinceSelection = dataPanel.PostString("Province not selected yet");
    }

    public override void Disable()
    {
        base.Disable();
        Destroy(brushSizeText.gameObject);
        Destroy(provinceSelection.gameObject);
    }
    public override void OnLeft(int x, int y)
    {
        selectedProvince = Map.Tiles[x, y].Province;
        provinceSelection.text = "Province selected: " +  selectedProvince.ID;

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
        for ( int i = leftX; i <= rightX; i++)
        {
            for ( int j = lowY; j <= highY; j++)
            {
                Map.Tiles[i, j].Province = selectedProvince;
                Renderer.WritePixelTo(i, j, selectedProvince.TemporaryRandomColor);
            }
        }    
    }
}
