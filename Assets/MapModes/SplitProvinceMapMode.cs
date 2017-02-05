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

        Debug.Log("Splitting province");
        var selectedProv = Map.Tiles[x, y].Province;
        int maxX = int.MinValue;
        int maxY = int.MinValue;
        int minX = int.MaxValue;
        int minY = int.MaxValue;
        foreach (var tile in selectedProv.Tiles)
        {
            if (tile.X > maxX)
                maxX = tile.X;
            if (tile.Y > maxY)
                maxY = tile.Y;
            if (tile.X < minX)
                minX = tile.X;
            if (tile.Y < minY)
                minY = tile.Y;
        }
        int midX = (maxX + minX) / 2;
        int midY = (maxY + minY) / 2;
        Vector2 startPoint = new Vector2(x, y);
        Vector2 pivot = new Vector2(midX, midY);
        float stepAngle = 360f / splitCount;
        Debug.LogFormat("Tiles count: {0}, Start point: {1}, Pivot: {2}, Split count: {3}", selectedProv.Tiles.Count, startPoint, pivot, splitCount);
        splitCenters.Clear();
        splitCenters.Add(startPoint);
        for (int i = 1; i < splitCount; i++)
        {
            Vector2 otherPoint = RotatePointAroundPivot(startPoint, pivot, stepAngle * i);
            splitCenters.Add(otherPoint);
            Debug.Log(otherPoint);
        }
        var lr = GetComponent<LineRenderer>();
        lr.sortingOrder = 1;
		lr.numPositions = splitCenters.Count * 3;
        for (int j = 0; j < splitCenters.Count; j++)
        {
            lr.SetPosition(j * 3, pivot - Vector2.one * Renderer.chunkSize/2);
            lr.SetPosition(j * 3 + 1, splitCenters[j] - Vector2.one * Renderer.chunkSize / 2);
            lr.SetPosition(j * 3 + 2, pivot - Vector2.one * Renderer.chunkSize / 2);
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
                float distance = (new Vector2(tileData.Tile.X, tileData.Tile.Y) - splitCenter).magnitude;
                if (distance < tileData.Distance)
                {
                    tileData.Distance = distance;
                    tileData.SplitCenter = j;
                }
            }
            tilesData[i] = tileData;
            //Debug.Log(tileData.SplitCenter);
        }
        splitProvinces.Clear();
        for (int i = 0; i < splitCount; i++)
            splitProvinces.Add(Map.CreateNewProvince(selectedProv.Type, selectedProv.SomeBool, selectedProv.OtherType, selectedProv.Continent, selectedProv));

        for (int i = 0; i < tilesData.Count; i++)
        {
            var tileData = tilesData[i];
            Map.AssignTileTo(tileData.Tile.X, tileData.Tile.Y, splitProvinces[tileData.SplitCenter]);
        }
        for ( int i =0; i < tilesData.Count; i++)
            Renderer.Update(tilesData[i].Tile);


    }
    public Vector2 RotatePointAroundPivot(Vector2 point, Vector2 pivot, float angle)
    {
        Vector2 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(new Vector3(0, 0, angle)) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }
}
