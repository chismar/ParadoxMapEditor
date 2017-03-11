using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Tile
{
    public int X { get; set; }
    public int Y { get; set; }
    Province owner;
    public Province Province { get { return owner; } set
        {
            if (owner == value)
                return;
            if (owner != null)
                owner.DetachTile(this);
            if (chunk != null && chunk.Size == 1)
            {
                owner.Chunks.Remove(chunk);
                Province.chunkPool.Push(chunk);
            }
            owner = value;
            if (!owner.HasTile(this))
                owner.AttachTile(this);
        }
    }
    public int BorderCount = 0;
    Chunk chunk;
    public Chunk Chunk {  get { return chunk; } set
        {
            if (chunk == value)
                return;
            if (chunk != null)
                chunk.DetachTile(this);
            chunk = value;
            if (!chunk.HasTile(this))
                chunk.AttachTile(this);
        }
    }
}

public class Chunk
{
    HashSet<Tile> tiles = new HashSet<Tile>();
    public void AttachTile(Tile tile)
    {
        if (tiles.Add(tile))
            tile.Chunk = this;
    }
    public void DetachTile(Tile tile)
    {
        tiles.Remove(tile);
    }
    public bool HasTile(Tile tile)
    {
        return tiles.Contains(tile);
    }
    public void Clear()
    {
        tiles.Clear();
    }
    public int Size { get { return tiles.Count; } }
    public IEnumerable<Tile> Tiles {  get { return tiles;  } }
}

