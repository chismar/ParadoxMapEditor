using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class StatesMapMode : MapMode
{
    Province selectedProvince;
    static MapModesAndControls controls;
    static Controller controller;
    UnityEngine.UI.Text provinceSelection;
    UnityEngine.UI.Text stateSelection;
    void OnEnable()
    {
        if (controls == null && controller == null)
        {
            controls = FindObjectOfType<MapModesAndControls>();
            controller = FindObjectOfType<Controller>();
        }
        controls.RegisterCallback(() => controller.SelectMapMode(this), "States mode");

    }



    public override void Enable()
    {
        base.Enable();
        selectedProvince = null;
        provinceSelection = dataPanel.PostString("Province not selected yet");
        stateSelection = dataPanel.PostString("State not selected yet");
    }

    public override void Disable()
    {
        base.Disable();
        Destroy(provinceSelection.gameObject);
        Destroy(stateSelection.gameObject);
    }
    public override void OnLeft(int x, int y)
    {
        selectedProvince = Map.Tiles[x, y].Province;
        Renderer.LitUpProvince(selectedProvince);
        provinceSelection.text = "Province selected: " + selectedProvince.ID;
        if (selectedProvince.State != null)
            stateSelection.text = "State selected: " + selectedProvince.State.ID;
        else
            stateSelection.text = "Province has no state";
    }

    public override void OnRightClick(int x, int y)
    {
        if (selectedProvince == null)
            return;
        if(selectedProvince.State == null)
        {
            selectedProvince.State = new State();
            selectedProvince.State.StateCategory = "rural";
            selectedProvince.State.ID = Map.States.Count;
            Map.States.Add(selectedProvince.State);
        }
        stateSelection.text = "State selected: " + selectedProvince.State.ID;
        var targetProvince = Map.Tiles[x, y].Province;
        if(targetProvince.State != selectedProvince.State)
        {
            targetProvince.State = selectedProvince.State;
            foreach (var tile in targetProvince.Tiles)
                Renderer.Update(tile);
        }
    }
}