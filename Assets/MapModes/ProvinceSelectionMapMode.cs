using UnityEngine;
using System.Collections;
using System;

public class ProvinceSelectionMapMode : MapMode
{

    Province selectedProvince;

    int brushSize = 3;

    public override void OnChoseMapMode()
    {
        selectedProvince = null;
    }
    public override void OnLeft(int x, int y)
    {
        selectedProvince = Map.Tiles[x, y].Province;

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
