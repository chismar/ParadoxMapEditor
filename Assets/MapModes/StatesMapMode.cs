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
    UnityEngine.UI.Text makeNewNote;
    UnityEngine.UI.Text removeNote;
    UnityEngine.UI.Text provinceSelection;
    UnityEngine.UI.Text stateSelection;
    bool RemoveState = false;
    bool MakeNew = false;
    void OnEnable()
    {
        if (controls == null && controller == null)
        {
            controls = FindObjectOfType<MapModesAndControls>();
            controller = FindObjectOfType<Controller>();
        }
        controls.RegisterCallback(() => controller.SelectMapMode(this), "States mode");

    }

    void Update()
    {
        if(enabled)
        {

            if (Input.GetKeyUp(KeyCode.R))
            {
                RemoveState = !RemoveState;

                makeNewNote.text = ("Always remove on right click: " + RemoveState);
            }
            if (Input.GetKeyUp(KeyCode.N))
            {
                MakeNew = !MakeNew;

                makeNewNote.text = ("Always make new on right click: " + MakeNew);
            }
        }
    }


    public override void Enable()
    {
        base.Enable();
        selectedProvince = null;
        provinceSelection = dataPanel.PostString("Province not selected yet");
        makeNewNote = dataPanel.PostString("Always make new on right click: " + MakeNew);
        stateSelection = dataPanel.PostString("State not selected yet");
        removeNote = dataPanel.PostString("Always remove on right click: " + RemoveState);
    }

    public override void Disable()
    {
        base.Disable();
        Destroy(provinceSelection.gameObject);
        Destroy(stateSelection.gameObject);
        Destroy(makeNewNote.gameObject);
        Destroy(removeNote.gameObject);
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
        if(RemoveState)
        {

            var province = Map.Tiles[x, y].Province;
            province.State = null;
            Renderer.Update(province);
            return;
        }
        if (selectedProvince == null)
            return;
        if(selectedProvince.State == null || MakeNew)
        {
            selectedProvince.State = new State();
            selectedProvince.State.StateCategory = "rural";
            selectedProvince.State.ID = Map.States.Count;
            Map.States.Add(selectedProvince.State);
            foreach (var tile in selectedProvince.Tiles)
                Renderer.Update(tile);
            Renderer.LitUpProvince(selectedProvince);
        }
        stateSelection.text = "State selected: " + selectedProvince.State.ID;
        var targetProvince = Map.Tiles[x, y].Province;
        if(targetProvince.State != selectedProvince.State)
        {
            targetProvince.State = selectedProvince.State;
            foreach (var tile in targetProvince.Tiles)
                Renderer.Update(tile);
            foreach (var tile in selectedProvince.Tiles)
                Renderer.Update(tile);
        }
    }
}