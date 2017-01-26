using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MergeProvinces : MapMode
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
        controls.RegisterCallback(() => controller.SelectMapMode(this), "Merge mode");

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
    Stack<Tile> tiles = new Stack<Tile>();
    public override void OnRightClick(int x, int y)
    {
        if (selectedProvince == null)
            return;
        if(selectedProvince.Tiles.Count == 0)
            return;
        var province = Map.Tiles[x, y].Province;
        if (province == selectedProvince)
            return;
        foreach (var tile in province.Tiles)
            tiles.Push(tile);
        while (tiles.Count > 0)
        {
            var tile = tiles.Pop();
            bool updateBorder = tile.BorderCount > 0;
            Map.AssignTileTo(tile.X, tile.Y, selectedProvince);

            Renderer.Update(tile);
            if(updateBorder)
            {
                if (tile.X > 0)
                    Renderer.Update(tile.X - 1, tile.Y);
                if (tile.X < Map.Width - 1)
                    Renderer.Update(tile.X + 1, tile.Y);
                if (tile.Y > 0)
                    Renderer.Update(tile.X, tile.Y - 1);
                if (tile.Y < Map.Height - 1)
                    Renderer.Update(tile.X, tile.Y + 1);
            }
           
        }
        
    }
}
