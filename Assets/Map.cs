using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Map : ScriptableObject
{
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
            emptyProvinces.Push(prevOwner);
            Provinces.Remove(prevOwner);
            ProvincesByID.Remove(prevOwner.ID);
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

    Stack<Province> emptyProvinces = new Stack<Province>();
    public Province CreateNewProvince(ProvinceType type, bool someBool, string otherType, int continent)
    {
        Province p = null;
        if (emptyProvinces.Count > 0)
            p = emptyProvinces.Pop();
        else
        {
            p = new Province();
            p.Type = type;
            p.SomeBool = someBool;
            p.OtherType = otherType;
            p.Continent = continent;
            p.ID = NextID++;
            p.MapUniqueColor = p.SerializedColor();
            Provinces.Add(p);
            ProvincesByID.Add(p.ID, p);
            ColorCodedProvinces.Add(p.MapUniqueColor, p);
        }

        return p;
    }
    
}
