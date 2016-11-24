using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class Province {

    
    public int ID; 
    public System.Drawing.Color MapUniqueColor;
    public ProvinceType Type;
    public bool SomeBool;
    public string OtherType;
    public int Continent; // Probably continent
    public HashSet<Tile> Tiles = new HashSet<Tile>();
    public Dictionary<Province, Adjacency> Adjacencies = new Dictionary<Province, Adjacency>();
    StrategicRegion region;
    public StrategicRegion StrategicRegion { get { return region; } set { if (region == value) return; if (region != null) region.Provinces.Remove(this); region = value; if(region != null) region.Provinces.Add(this); } }
    State state;
    public State State { get { return state; } set { if (state == value) return; if (state != null) state.Provinces.Remove(this); state = value; if (state != null)  state.Provinces.Add(this); } }
    public Vector2 Anchor {  get
        {
            int maxX = int.MinValue;
            int maxY = int.MinValue;
            int minX = int.MaxValue;
            int minY = int.MaxValue;
            foreach (var tile in Tiles)
            {
                if (tile.X > maxX)
                    maxX = tile.X;
                if (tile.Y > maxY)
                    maxY = tile.Y;
                if (tile.X < minX)
                    minX = tile.X;
                if (tile.Y < minY)
                    minY = tile.Y;
            }
            int midX = (maxX + minX) / 2;
            int midY = (maxY + minY) / 2;
            return new Vector2(midX, midY);
        }
    }
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

    public System.Drawing.Color SerializedColor()
    {
        int r = (ID >> 16) & 255;
        int g = (ID >> 8) & 255;
        int b = ID & 255;
        var color = System.Drawing.Color.FromArgb(r, g, b);
        return color;
    }

    public void TextureColor(ref Color32 color)
    {
        color.r = (byte)(ID & 255);
        color.g = (byte)((ID >> 8) & 255);
    }



}

public enum ProvinceType { Land, Sea, Lake }