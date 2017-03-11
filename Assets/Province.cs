using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Province {

    public Map Map;
    public int ID; 
    public System.Drawing.Color MapUniqueColor;
    public string Category;
    public bool SomeBool;
    public string Type;
    public int Continent; // Probably continent
    public HashSet<Tile> Tiles = new HashSet<Tile>();
    public HashSet<Chunk> Chunks = new HashSet<Chunk>();
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
    public static Stack<Chunk> chunkPool = new Stack<Chunk>();
    public void AttachTile(Tile tile)
    {
        if(Tiles.Add(tile))
        {
            tile.Province = this;
            AssignChunk(tile);
        }
    }

    void AssignChunk(Tile tile)
    {
        if (tile.X > 0)
        {
            var nextTile = Map.Tiles[tile.X - 1, tile.Y];
            if (nextTile.Province == tile.Province && nextTile.Chunk != null)
            {
                tile.Chunk = nextTile.Chunk;
            }
        }
        if (tile.X < Map.Width - 1)
        {
            var nextTile = Map.Tiles[tile.X + 1, tile.Y];
            if (nextTile.Province == tile.Province && nextTile.Chunk != null)
            {
                if (Chunks.Contains(tile.Chunk) && tile.Chunk != nextTile.Chunk)
                    MergeChunks(tile.Chunk, nextTile.Chunk);
                else
                    tile.Chunk = nextTile.Chunk;
            }
        }
        if (tile.Y > 0)
        {
            var nextTile = Map.Tiles[tile.X, tile.Y - 1];
            if (nextTile.Province == tile.Province && nextTile.Chunk != null)
            {
                if (Chunks.Contains(tile.Chunk) && tile.Chunk != nextTile.Chunk)
                    MergeChunks(tile.Chunk, nextTile.Chunk);
                else
                    tile.Chunk = nextTile.Chunk;
            }
        }
        if (tile.Y < Map.Height - 1)
        {
            var nextTile = Map.Tiles[tile.X, tile.Y + 1];
            if (nextTile.Province == tile.Province && nextTile.Chunk != null)
            {
                if (Chunks.Contains(tile.Chunk) && tile.Chunk != nextTile.Chunk)
                    MergeChunks(tile.Chunk, nextTile.Chunk);
                else
                    tile.Chunk = nextTile.Chunk;
            }
        }
        if (tile.Chunk == null)
        {
            /*if (chunkPool.Count > 0)
            {

                tile.Chunk = chunkPool.Pop();
                tile.Chunk.Clear();
            }
            else*/
                tile.Chunk = new Chunk();
            Chunks.Add(tile.Chunk);
        }
    }
    static List<Tile> cachedTiles = new List<Tile>();
    private void MergeChunks(Chunk chunk1, Chunk chunk2)
    {
        Chunk from = null;
        Chunk to = null;
        if(chunk1.Size > chunk2.Size)
        {
            from = chunk2;
            to = chunk1;
        }
        else
        {
            from = chunk1;
            to = chunk2;
        }
        chunkPool.Push(from);
        cachedTiles.Clear();
        foreach (var t in from.Tiles)
            cachedTiles.Add(t);
        foreach (var t in cachedTiles)
            t.Chunk = to;


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
