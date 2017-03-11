using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Map : ScriptableObject
{
	World world;
	public World World {
		get { return world; }
		set {
			world = value;
			world.Map = this;
		}
	}
    public List<Province> Provinces;
    public Dictionary<int, Province> ProvincesByID;
    public Dictionary<System.Drawing.Color, Province> ColorCodedProvinces;
    public List<State> States;
    public List<StrategicRegion> StrategicRegions;
    public List<SupplyArea> SupplyAreas;
    public Tile[,] Tiles;
    public int Width;
    public int Height;
    public int NextID;
	public List<string> provinceTypes = new List<string>();
	public List<string> stateTypes = new List<string>();
    public List<string> provinceCategories = new List<string>();
	public Dictionary<string, string> Localisation = new Dictionary<string, string>();
	public void SetLoc(string id, string text)
	{
		if (Localisation.ContainsKey (id))
			Localisation [id] = text;
		else
			Localisation.Add (id, text);
	}
    public void AssignTileTo(int x, int y, Province province)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return;
        var tile = Tiles[x, y];
        var prevOwner = tile.Province;
        if (prevOwner == province)
            return;
        tile.Province = province;
        if(prevOwner != null && prevOwner.Tiles.Count == 0)
        {
            Provinces.Remove(prevOwner);
            ProvincesByID.Remove(prevOwner.ID);
            prevOwner.State = null;
            prevOwner.StrategicRegion = null;
            ColorCodedProvinces.Remove(prevOwner.MapUniqueColor);
        }
        //change borders
        tile.BorderCount = 0;
        
        var nextTile = Tiles[(x + 1) % Width, y];
        if(nextTile.Province != province)
        {
            tile.BorderCount++;
            if(nextTile.Province == prevOwner)
                nextTile.BorderCount++;
        }
        else
            nextTile.BorderCount--;
        nextTile = Tiles[x == 0? Width-1 : x - 1, y];
        if (nextTile.Province != province)
        {
            tile.BorderCount++;
            if (nextTile.Province == prevOwner)
                nextTile.BorderCount++;
        }
        else
            nextTile.BorderCount--;

        if (y != Height - 1)
        {
            nextTile = Tiles[x, y + 1];
            if (nextTile.Province != province)
            {
                tile.BorderCount++;
                if (nextTile.Province == prevOwner)
                    nextTile.BorderCount++;
            }
            else
                nextTile.BorderCount--;
        }
        
        if(y != 0)
        {
            nextTile = Tiles[x, y - 1];
            if (nextTile.Province != province)
            {
                tile.BorderCount++;
                if (nextTile.Province == prevOwner)
                    nextTile.BorderCount++;
            }
            else
                nextTile.BorderCount--;
        }
        
    }

    public Province CreateNewProvince(string type, bool someBool, string otherType, int continent, Province fromProvince = null)
    {
        Province p = new Province();
        p.Map = this;
        p.ID = NextID++;
        p.MapUniqueColor = p.SerializedColor();

        Provinces.Add(p);
        ProvincesByID.Add(p.ID, p);
        ColorCodedProvinces.Add(p.MapUniqueColor, p);
        p.Category = type;
        p.SomeBool = someBool;
        p.Type = otherType;
        p.Continent = continent;
        if (fromProvince != null)
        {
            p.State = fromProvince.State;
            p.StrategicRegion = fromProvince.StrategicRegion;
        }
        return p;
    }
	GameObject go;
	void Awake()
	{
		go = new GameObject ("IconsUpdater");
		//go.AddComponent<Dummy> ().StartCoroutine(IconsUpdater());

	}


	void OnDestroy()
	{
		Destroy (go);
	}
	IEnumerator IconsUpdater()
	{
		int index = 0;

		while (true) {
			if (States == null || States.Count == 0)
				yield return null;
            if (States == null)
                continue;
			if (index > States.Count)
				index = 0;
			var state = States [index];
			Vector2 middle = Vector2.zero;
			for (int i = 0; i < state.Provinces.Count; i++)
				middle += state.Provinces [i].Anchor;
			middle /= state.Provinces.Count;
            if (state.IconGO != null)
                state.IconGO.transform.position = middle;
            else
                state.IconGO = new GameObject("State " + state.ID);
            index++;
			yield return null;
		}
	}
}

public class Dummy : MonoBehaviour
{
}