using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SplitProvinceMapMode : MapMode
{
    

    float intermediateSplitCount = 3;
    int splitCount = 3;
    static MapModesAndControls controls;
    static Controller controller;
    UnityEngine.UI.Text brushSizeText;
    void OnEnable()
    {
        if (controls == null && controller == null)
        {
            controls = FindObjectOfType<MapModesAndControls>();
            controller = FindObjectOfType<Controller>();
        }
        controls.RegisterCallback(() => controller.SelectMapMode(this), "Province splitter");

    }

    void Update()
    {
        if (enabled)
        {
            var scroll = Input.mouseScrollDelta.y;
            if (scroll > 0)
            {
                splitCount = (int)(splitCount * (1 + scroll));
            }
            intermediateSplitCount += scroll;
            var newBrush = (int)intermediateSplitCount;
            if (newBrush != splitCount)
                brushSizeText.text = "Split count: " + newBrush;
            splitCount = newBrush;
            if(splitCount <= 0)
            {
                splitCount = 1;
                intermediateSplitCount = 1f;
                brushSizeText.text = "Split count: " + splitCount;
            }
            

        }

    }

    public override void Enable()
    {
        base.Enable();
        brushSizeText = dataPanel.PostString("Split count: " + splitCount.ToString());
    }

    public override void Disable()
    {
        base.Disable();
        Destroy(brushSizeText.gameObject);
    }

    List<Vector2> splitCenters = new List<Vector2>();
    List<Province> splitProvinces = new List<Province>();
    struct TileData
    {
        public Tile Tile;
        public int SplitCenter;
        public float Distance;
    }
    List<TileData> tilesData = new List<TileData>();
    public override void OnLeft(int x, int y)
    {
        var selectedProv = Map.Tiles[x, y].Province;
        int midX = 0;
        int midY = 0;
        foreach (var tile in selectedProv.Tiles)
        {
            midX += tile.X;
            midY += tile.Y;
        }
        midX /= selectedProv.Tiles.Count;
        midY /= selectedProv.Tiles.Count;
        Vector2 startPoint = new Vector2(x, y);
        Vector2 pivot = new Vector2(midX, midY);
        float stepAngle = 360f / splitCount;
        splitCenters.Clear();
        splitCenters.Add(startPoint);
        for (int i = 1; i < splitCount; i++)
        {
            Vector2 otherPoint = RotatePointAroundPivot(startPoint, pivot, stepAngle * i);
            splitCenters.Add(otherPoint);
        }

        tilesData.Clear();
        foreach (var tile in selectedProv.Tiles)
            tilesData.Add(new TileData() { Tile = tile, SplitCenter = 0, Distance = float.MaxValue });
        for (int i = 0; i < tilesData.Count; i++)
        {
            var tileData = tilesData[i];
            for (int j = 0; j < splitCenters.Count; j++)
            {
                var splitCenter = splitCenters[j];
                float distance = Mathf.Abs((float)tileData.Tile.X - splitCenter.x) + Mathf.Abs((float)tileData.Tile.Y - splitCenter.y);
                if (distance < tileData.Distance)
                {
                    tileData.Distance = distance;
                    tileData.SplitCenter = j;
                }
            }
        }
        splitProvinces.Clear();
        for (int i = 0; i < splitCount; i++)
            splitProvinces.Add(Map.CreateNewProvince(selectedProv.Type, selectedProv.SomeBool, selectedProv.OtherType, selectedProv.Continent));

        for (int i = 0; i < tilesData.Count; i++)
        {
            var tileData = tilesData[i];
            Map.AssignTileTo(tileData.Tile.X, tileData.Tile.Y, splitProvinces[tileData.SplitCenter]);
        }


    }
    public Vector2 RotatePointAroundPivot(Vector2 point, Vector2 pivot, float angle)
    {
        Vector2 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(new Vector3(0, 0, angle)) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }
}
