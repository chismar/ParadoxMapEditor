using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BucketMapMode : MapMode
{
    Province selectedProvince;
    
    static MapModesAndControls controls;
    static Controller controller;
    UnityEngine.UI.Text provinceSelection;
    void OnEnable()
    {
        if (controls == null && controller == null)
        {
            controls = FindObjectOfType<MapModesAndControls>();
            controller = FindObjectOfType<Controller>();
        }
        controls.RegisterCallback(() => controller.SelectMapMode(this), "Bucket mode");

    }

  

    public override void Enable()
    {
        base.Enable();
        selectedProvince = null;
        provinceSelection = dataPanel.PostString("Province not selected yet");
    }

    public override void Disable()
    {
        base.Disable();
        Destroy(provinceSelection.gameObject);
    }
    public override void OnLeft(int x, int y)
    {
        selectedProvince = Map.Tiles[x, y].Province;
        Renderer.LitUpProvince(selectedProvince);
        provinceSelection.text = "Province selected: " + selectedProvince.ID;

    }
    Queue<Tile> q = new Queue<Tile>();
    List<Tile> updateTiles = new List<Tile>();
    HashSet<Tile> updateBorderTiles = new HashSet<Tile>();
    public override void OnRightClick(int x, int y)
    {
        if (selectedProvince == null)
            return;
        updateTiles.Clear(); updateBorderTiles.Clear();
        q.Clear();
        var startTile = Map.Tiles[x, y];
        var targetProvince = startTile.Province;
        if (targetProvince == selectedProvince)
            return;
        q.Enqueue(startTile);
        while (q.Count > 0)
        {
            var curTile = q.Dequeue();
            if (curTile.Province != targetProvince)
                continue;
            updateTiles.Add(curTile);
            
            bool updateBorder = curTile.BorderCount > 0;
            Map.AssignTileTo(curTile.X, curTile.Y, selectedProvince);
            if (updateBorder)
            {
                if (curTile.X > 0)
                    updateBorderTiles.Add(Map.Tiles[curTile.X - 1, curTile.Y]);
                if (curTile.X < Map.Width - 1)
                    updateBorderTiles.Add(Map.Tiles[curTile.X + 1, curTile.Y]);
                if (curTile.Y > 0)
                    updateBorderTiles.Add(Map.Tiles[curTile.X, curTile.Y - 1]);
                if (curTile.Y < Map.Height - 1)
                    updateBorderTiles.Add(Map.Tiles[curTile.X, curTile.Y + 1]);
            }
            if (curTile.X < Map.Width - 1)
            {
                var nextTile = Map.Tiles[curTile.X + 1, curTile.Y];
                if (nextTile.Province == targetProvince)
                    q.Enqueue(nextTile);
            }
            if (curTile.X > 0)
            {
                var nextTile = Map.Tiles[curTile.X - 1, curTile.Y];
                if (nextTile.Province == targetProvince)
                    q.Enqueue(nextTile);
            }
            if (curTile.Y < Map.Height - 1)
            {
                var nextTile = Map.Tiles[curTile.X, curTile.Y + 1];
                if (nextTile.Province == targetProvince)
                    q.Enqueue(nextTile);
            }
            if (curTile.Y > 0)
            {
                var nextTile = Map.Tiles[curTile.X, curTile.Y - 1];
                if (nextTile.Province == targetProvince)
                    q.Enqueue(nextTile);
            }
        }

        for ( int i = 0; i < updateTiles.Count; i++)
        {
            var tile = updateTiles[i];
            Renderer.Update(tile);
        }

        foreach (var tile in updateBorderTiles)
            Renderer.Update(tile);
        updateTiles.Clear();
        updateBorderTiles.Clear();
    }
 
}
