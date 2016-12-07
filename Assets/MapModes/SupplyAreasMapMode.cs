using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SupplyAreasMapMode : MapMode
{
    Province selectedProvince;
    static MapModesAndControls controls;
    static Controller controller;
    UnityEngine.UI.Text provinceSelection;
    UnityEngine.UI.Text regionSelection;
    void OnEnable()
    {
        if (controls == null && controller == null)
        {
            controls = FindObjectOfType<MapModesAndControls>();
            controller = FindObjectOfType<Controller>();
        }
        controls.RegisterCallback(() => controller.SelectMapMode(this), "Supply mode");

    }



    public override void Enable()
    {
        base.Enable();
        selectedProvince = null;
        provinceSelection = dataPanel.PostString("Province not selected yet");
        regionSelection = dataPanel.PostString("Supply area not selected yet");
    }

    public override void Disable()
    {
        base.Disable();
        Destroy(provinceSelection.gameObject);
        Destroy(regionSelection.gameObject);
    }
    public override void OnLeft(int x, int y)
    {
        selectedProvince = Map.Tiles[x, y].Province;
        Renderer.LitUpProvince(selectedProvince);
        provinceSelection.text = "Province selected: " + selectedProvince.ID;
        if (selectedProvince.State != null && selectedProvince.State.Supply != null)
            regionSelection.text = "Supply area selected: " + selectedProvince.State.Supply.ID;
        else if (selectedProvince.State != null)
            regionSelection.text = "Province state has no Supply area";
        else
            regionSelection.text = "Province has no state";
    }

    public override void OnRightClick(int x, int y)
    {
        if (selectedProvince == null || selectedProvince.State == null)
            return;
        if (selectedProvince.State.Supply == null)
        {
            selectedProvince.State.Supply = new SupplyArea();
            selectedProvince.State.Supply.SupplyValue = 10;
            selectedProvince.State.Supply.ID = Map.SupplyAreas.Count;
            Map.SupplyAreas.Add(selectedProvince.State.Supply);
            foreach (var tile in selectedProvince.Tiles)
                Renderer.Update(tile);

            Renderer.LitUpProvince(selectedProvince);
        }
        regionSelection.text = "Supply area selected: " + selectedProvince.State.Supply.ID;
        var targetProvince = Map.Tiles[x, y].Province;
        if (targetProvince.State == null)
            return;
        if (targetProvince.State.Supply != selectedProvince.State.Supply)
        {
            targetProvince.State.Supply = selectedProvince.State.Supply;
            foreach (var tile in targetProvince.Tiles)
                Renderer.Update(tile);
            foreach (var tile in selectedProvince.Tiles)
                Renderer.Update(tile);
        }
    }
}