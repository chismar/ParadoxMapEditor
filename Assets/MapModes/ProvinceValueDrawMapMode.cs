using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
public class ProvinceValueDrawMapMode : MapMode
{

    static MapModesAndControls controls;
    static Controller controller;
	Dropdown fieldDropdown;
	Dropdown valueDropdown;
	InputField fieldValue;
    Text curValue;
	void OnEnable()
    {
        if (controls == null && controller == null)
        {
            controls = FindObjectOfType<MapModesAndControls>();
            controller = FindObjectOfType<Controller>();
        }
        controls.RegisterCallback(() => controller.SelectMapMode(this), "Data painter");
		fields.Add (StateManpower);
		fields.Add (AreaSupply);
		fields.Add (ProvinceCategory);
		fields.Add (StateCategory);
		fields.Add (StateName);
		fields.Add (RegionName);
		fields.Add (SupplyAreaName);
        fields.Add(ProvinceType);
        //fields.Add (StateOwner);
    }

	const string StateManpower = "State manpower";
	const string AreaSupply = "Supply area value";
	const string ProvinceCategory = "Province category";
	const string StateCategory = "State category";
	const string StateName = "State name";
	const string RegionName = "Strategic region name";
	const string SupplyAreaName = "Supply name";
	const string StateOwner = "Owner";
    const string ProvinceType = "Province type";
    void Update()
    {
		if (Input.GetKeyUp (KeyCode.O)) {
			//Create new owner from selected one
		}

    }
	List<string> fields = new List<string>();
    public override void Enable()
    {
        base.Enable();
		fieldDropdown = dataPanel.PostDropdown ("State field");
		fieldDropdown.onValueChanged.AddListener (DropdownValue);
		fieldValue = dataPanel.PostInput ("Field value");
        fieldValue.placeholder.GetComponent<Text>().text = "Custom value";
		fieldValue.onValueChanged.AddListener (ValueChanged);
		fieldDropdown.ClearOptions ();
		valueDropdown = dataPanel.PostDropdown ("Value dropdown");
		valueDropdown.ClearOptions ();
		valueDropdown.gameObject.SetActive (false);
		fieldDropdown.AddOptions (fields);
        curValue = dataPanel.PostString("current value");
        DropdownValue(0);
    }

	int intValue = 0;
	string text;
	void ValueChanged(string value)
	{
		text = value;
		int.TryParse (text, out intValue);
	}

	void DropdownValue(int value)
	{
        fieldValue.gameObject.SetActive(false);
        valueDropdown.ClearOptions();
        if (value == 2)
            valueDropdown.AddOptions(Map.provinceTypes);
        else if (value == 3)
            valueDropdown.AddOptions(Map.stateTypes);
        else if (value == 7)
            valueDropdown.AddOptions(Map.provinceCategories);
        else
        {

            fieldValue.gameObject.SetActive(true);
            valueDropdown.gameObject.SetActive(false);
            return;
        }

		valueDropdown.gameObject.SetActive (true);
	}
    public override void Disable()
    {
        base.Disable();
		Destroy (fieldDropdown.gameObject);
		Destroy (fieldValue.gameObject);
		Destroy (valueDropdown.gameObject);
        Destroy(curValue.gameObject);
	}
    public override void OnLeft(int x, int y)
    {

        curValue.text = "";
        var option = fieldDropdown.value;
        switch (option)
        {
            case 0:
                //State manpower
                var stateM = Map.Tiles[x, y].Province.State;
                if (stateM == null)
                    break;
                curValue.text = stateM.Manpower.ToString();
                break;
            case 1:
                //Supply area value
                var stateA = Map.Tiles[x, y].Province.State;
                if (stateA == null || stateA.Supply == null)
                    break;
                curValue.text = stateA.Supply.SupplyValue.ToString() ;
                break;
            case 2:
                //Province category
                var prov = Map.Tiles[x, y].Province;

                curValue.text = prov.Type;
                break;
            case 3:
                //State category

                
                var stateC = Map.Tiles[x, y].Province.State;
                if (stateC == null)
                    break;
                curValue.text = stateC.StateCategory ;
                break;
            case 4:
                //State name
                
                var stateN = Map.Tiles[x, y].Province.State;
                if (stateN == null)
                    break;
                curValue.text = stateN.Name;
                break;
            case 5:
                //Region name;
                
                var region = Map.Tiles[x, y].Province.StrategicRegion;
                if (region == null)
                    break;
                curValue.text = region.Name;
                break;
            case 6:
                //Area name

                var stateAN = Map.Tiles[x, y].Province.State;
                if (stateAN == null || stateAN.Supply == null)
                    break;
                curValue.text = stateAN.Supply.Name;
                break;
            case 7:
                //Province category
                var provC = Map.Tiles[x, y].Province;

                curValue.text = provC.Category;
                break;
        }
    }
    public override void OnRightClick(int x, int y)
    {
        OnRightDrag(x, y);
    }
    public override void OnRightDrag(int x, int y)
    {
		var option = fieldDropdown.value;
		switch (option) {
		case 0:
			//State manpower
			var stateM = Map.Tiles [x, y].Province.State;
			if (stateM == null)
				break;
			stateM.Manpower = intValue;
			break;
		case 1:
			//Supply area value
			var stateA = Map.Tiles [x, y].Province.State;
			if (stateA == null || stateA.Supply == null)
				break;
			stateA.Supply.SupplyValue = intValue;
                break;
		case 2:
			//Province category
			var prov = Map.Tiles [x, y].Province;
            
			prov.Type = Map.provinceTypes [valueDropdown.value];
                Renderer.Update(prov);
                break;
		case 3:
			//State category

			var stateC = Map.Tiles [x, y].Province.State;
			if (stateC == null)
				break;
			stateC.StateCategory = Map.stateTypes [valueDropdown.value];

                Renderer.Update(stateC);
                break;
		case 4:
			//State name

			var stateN = Map.Tiles [x, y].Province.State;
			if (stateN == null)
				break;
			Map.SetLoc (stateN.Name, text);
			break;
		case 5:
			//Region name;

			var region = Map.Tiles [x, y].Province.StrategicRegion;
			if (region == null)
				break;
			Map.SetLoc (region.Name, text);
			break;
		case 6:
			//Area name

			var stateAN = Map.Tiles [x, y].Province.State;
			if (stateAN == null || stateAN.Supply == null)
				break;
			Map.SetLoc (stateAN.Supply.Name, text);
			break;
        case 7:
            //Province category
            var provC = Map.Tiles[x, y].Province;

            provC.Category = Map.provinceCategories[valueDropdown.value];
            Renderer.Update(provC);
            break;
        }
        OnLeft(x, y);

    }
}
