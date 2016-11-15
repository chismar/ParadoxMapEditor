using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Map : ScriptableObject
{
    public List<Province> Provinces;
    public Dictionary<int, Province> ProvincesByID;
    public Dictionary<System.Drawing.Color, Province> ColorCodedProvinces;
    public Tile[,] Tiles;
    public int Width;
    public int Height;
    
}
