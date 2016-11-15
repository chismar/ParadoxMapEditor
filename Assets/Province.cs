using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Province {

    public Color TemporaryRandomColor;
    public int LineNumberThatMightBeImportant;
    public int FirstNumber; // possibly ID
    public byte SecondNumber; // red
    public byte ThirdNumber; // green
    public byte FourthNumber; // blue I believe
    public ProvinceType Type;
    public bool SomeBool;
    public string OtherType;
    public int LastValue;
    public HashSet<Tile> Tiles = new HashSet<Tile>();
    public Dictionary<Province, Adjacency> Adjacencies = new Dictionary<Province, Adjacency>();
    public void AttachTile(Tile tile)
    {
        if(Tiles.Add(tile))
            tile.Province = this;
    }

    public bool HasTile(Tile tile)
    {
        return Tiles.Contains(tile);
    }

    public void DetachTile(Tile tile)
    {
        Tiles.Remove(tile);
    }
	
}

public enum ProvinceType { Land, Sea, Lake }