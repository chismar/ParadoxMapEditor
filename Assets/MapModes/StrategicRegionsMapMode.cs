using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class StrategicRegionsMapMode : MapMode
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
        controls.RegisterCallback(() => controller.SelectMapMode(this), "Regions mode");

    }
    private void Update()
    {
        if(enabled)
        {
            if(Input.GetKeyUp(KeyCode.N) && selectedProvince != null)
            {
                selectedProvince.StrategicRegion = new StrategicRegion();
                selectedProvince.StrategicRegion.ID = Map.StrategicRegions.Count;
                Map.StrategicRegions.Add(selectedProvince.StrategicRegion);
                foreach (var tile in selectedProvince.Tiles)
                    Renderer.Update(tile);

                Renderer.LitUpProvince(selectedProvince);
            }
            if (Input.GetKeyUp(KeyCode.R))
            {
                var state = Map.States.Find(s => s.Provinces.Count > 0 && s.Provinces.Find(p => p.StrategicRegion != null && p.StrategicRegion != s.Provinces[0].StrategicRegion) != null);
                if (state != null)
                {
                    selectedProvince = state.Provinces[0];
                    Renderer.LitUpProvince(selectedProvince);
                    provinceSelection.text = "Province selected: " + selectedProvince.ID;
                    if (selectedProvince.StrategicRegion != null)
                        regionSelection.text = "Region selected: " + selectedProvince.StrategicRegion.ID;
                    else
                        regionSelection.text = "Province has no Region";
                    Camera.main.transform.position = new Vector3(selectedProvince.Anchor.x - Renderer.chunkSize / 2, selectedProvince.Anchor.y - Renderer.chunkSize / 2, -10);

                }

            }
            if(Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyUp(KeyCode.D))
            {
                StrategicRegion region = null;
                do
                {
                    region = Map.StrategicRegions.Find(r => r.Provinces.Count > 0 && r.Provinces.Find(p => p.Category != r.Provinces[0].Category) != null);
                    if (region == null)
                        break;
                    var newRegion = new StrategicRegion();
                    newRegion.ID = Map.StrategicRegions.Count;
                    Map.StrategicRegions.Add(newRegion);
                    var baseCategory = region.Provinces[0].Category;
                    oldProvinces.Clear();
                    foreach (var regionProv in region.Provinces)
                        oldProvinces.Add(regionProv);
                    foreach (var oldProv in oldProvinces)
                        if (oldProv.Category != baseCategory)
                        {
                            oldProv.StrategicRegion = newRegion;
                        }
                } while (true);
            }
        }
        
    }



    List<Province> oldProvinces = new List<Province>();
    public override void Enable()
    {
        base.Enable();
        selectedProvince = null;
        provinceSelection = dataPanel.PostString("Province not selected yet");
        regionSelection = dataPanel.PostString("Region not selected yet");
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
        if (selectedProvince.StrategicRegion != null)
            regionSelection.text = "Region selected: " + selectedProvince.StrategicRegion.ID;
        else
            regionSelection.text = "Province has no Region";
    }

    public override void OnRightClick(int x, int y)
    {
        if (selectedProvince == null)
            return;
        if (selectedProvince.StrategicRegion == null)
        {
            selectedProvince.StrategicRegion = new StrategicRegion();
            selectedProvince.StrategicRegion.ID = Map.StrategicRegions.Count;
            Map.StrategicRegions.Add(selectedProvince.StrategicRegion);
            foreach (var tile in selectedProvince.Tiles)
                Renderer.Update(tile);

            Renderer.LitUpProvince(selectedProvince);
        }
        regionSelection.text = "Region selected: " + selectedProvince.StrategicRegion.ID;
        var targetProvince = Map.Tiles[x, y].Province;
        targetProvince.StrategicRegion = selectedProvince.StrategicRegion;
        Renderer.Update(targetProvince);
    }
}