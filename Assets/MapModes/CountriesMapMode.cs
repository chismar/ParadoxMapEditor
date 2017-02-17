using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


public class CountriesMapMode : MapMode
{

	static MapModesAndControls controls;
	static Controller controller;


	Country selectedCountry;

	Text selectedTag;
	Dropdown actionType; //set capital, copy this country with new tag or add state to country
	InputField copyNewTagValue;
	Dropdown allTags;
	Text reminderToUseO;
	void OnEnable()
	{
		if (controls == null && controller == null)
		{
			controls = FindObjectOfType<MapModesAndControls>();
			controller = FindObjectOfType<Controller>();
		}
		controls.RegisterCallback(() => controller.SelectMapMode(this), "Countries map mode");
	}

	void Update()
	{
		if (Input.GetKeyUp (KeyCode.O)) {
			//Create new owner from selected one
			var tag = copyNewTagValue.text;
			if (!Regex.IsMatch (tag, "[A-Z][A-Z][A-Z]"))
				return;
			if (selectedCountry == null)
				return;
			selectedCountry = Map.World.Create (tag, selectedCountry);
		}

	}
	public override void Enable()
	{
		base.Enable();
		var list = Map.World.CountriesTags;
		selectedTag = dataPanel.PostString ("selected tag");
		actionType = dataPanel.PostDropdown ("action type");
        actionType.ClearOptions();
		actionType.AddOptions (new List<string> (new string[]{"Add state to country", "Set capital"}));
		reminderToUseO = dataPanel.PostString ("reminder");
		reminderToUseO.text = "Press [O]wner to copy the selected country with a new tag";
		copyNewTagValue = dataPanel.PostInput ("new tag");
        copyNewTagValue.placeholder.GetComponent<Text>().text = "New TAG";
       
		allTags = dataPanel.PostDropdown ("all tags");
        allTags.ClearOptions();
		allTags.AddOptions (list);
	}

	public override void Disable()
	{
		base.Disable();
		Destroy (selectedTag.gameObject);
		Destroy (actionType.gameObject);
		Destroy (copyNewTagValue.gameObject);
		Destroy (allTags.gameObject);
		Destroy (reminderToUseO.gameObject);
	}
	public override void OnLeft(int x, int y)
	{
		var ownedState = Map.Tiles [x, y].Province.State;
		if (ownedState == null) {
			selectedTag.text = "province has no state: " + Map.Tiles[x,y].Province.ID;
			selectedCountry = null;
			return;
		}
		selectedCountry = ownedState.Owner;
		selectedTag.text = selectedCountry != null ? selectedCountry.Tag : "no owner";
	}
	public override void OnRightClick(int x, int y)
	{
		OnRightDrag(x, y);
	}
	public override void OnRightDrag(int x, int y)
	{
		if (selectedCountry == null)
			return;
		var ownedState = Map.Tiles [x, y].Province.State;
		if (ownedState == null)
			return;
		switch (actionType.value) {
		case 0:
			//add state to country
			ownedState.Owner = selectedCountry;
			Renderer.Update (ownedState);
			break;
		case 1:
			//set capital of a country
			if(ownedState.Owner == selectedCountry)
				selectedCountry.Capital = ownedState;
			break;
		}


	}
}


