using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalChunkMergingTool : MapMode
{
    static MapModesAndControls controls;
    static Controller controller;
    
    void OnEnable()
    {
        if (controls == null && controller == null)
        {
            controls = FindObjectOfType<MapModesAndControls>();
            controller = FindObjectOfType<Controller>();
        }
        controls.RegisterCallback(() => controller.SelectMapMode(this), "Global chunk merging tool");

    }
    List<Tile> cacheTilesList = new List<Tile>();
    List<Chunk> cacheList = new List<Chunk>();
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.LeftAlt))
        {
            foreach(var p in Map.Provinces)
            {
                Debug.LogFormat("{0} - {1}", p.ID, p.Chunks.Count);
            }
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.Space))
        {
            for (int pi = 0; pi < Map.Provinces.Count; pi++)
            {
                var p = Map.Provinces[pi];
                if (p.Chunks == null || (p.Chunks.Count == 1 && p.Tiles.Count >= 16))
                {
                    continue;
                }
                else if (p.Chunks.Count == 1 && p.Tiles.Count < 16)
                {
                    //We should remove this province entirely
                    cacheList.Clear();
                    foreach (var chunk in p.Chunks)
                        cacheList.Add(chunk);
                    foreach (var chunk in cacheList)
                        MergeChunk(chunk, p);
                    pi--;
                }
                else if (p.Chunks.Count > 1)
                {
                    Chunk maxChunk = null;
                    int maxSize = -1;
                    foreach (var chunk in p.Chunks)
                    {
                        if (chunk.Size > maxSize)
                        {
                            maxChunk = chunk;
                            maxSize = chunk.Size;
                        }
                    }
                    if (maxSize < 16)
                    {
                        //Again, we should remove every chunk and this province entirely
                        cacheList.Clear();
                        foreach (var chunk in p.Chunks)
                            cacheList.Add(chunk);
                        foreach (var chunk in cacheList)
                        {
                            MergeChunk(chunk, p);
                        }
                        pi--;
                    }
                    else
                    {
                        //We should remove or separate all other chunks
                        //p.Chunks.Remove(maxChunk);
                        cacheList.Clear();
                        foreach (var chunk in p.Chunks)
                            if (chunk != maxChunk)
                                cacheList.Add(chunk);
                        foreach (var chunk in cacheList)
                        {
                            MergeChunk(chunk, p);
                        }
                    }

                }
            }
            Renderer.ChangeMode(Renderer.mode);

        }
    }
    void MergeChunk(Chunk chunk, Province origin)
    {
        if(chunk.Size >= 16)
        {
            var p = Map.CreateNewProvince(origin.Category, origin.SomeBool, origin.Type, origin.Continent, origin);
            foreach (var tile in chunk.Tiles)
            {
                Map.AssignTileTo(tile.X, tile.Y, p);
            }

            return;
        }

        List<Province> neighbours = new List<Province>();
        foreach(var tile in chunk.Tiles)
        {
            if(tile.X > 0)
            {
                var nextTile = Map.Tiles[tile.X - 1, tile.Y];
                if (nextTile.Province != tile.Province)
                    neighbours.Add(nextTile.Province);
            }
            if(tile.X < Map.Width - 1)
            {
                var nextTile = Map.Tiles[tile.X + 1, tile.Y];
                if (nextTile.Province != tile.Province)
                    neighbours.Add(nextTile.Province);
            }
            if (tile.Y > 0)
            {
                var nextTile = Map.Tiles[tile.X, tile.Y - 1];
                if (nextTile.Province != tile.Province)
                    neighbours.Add(nextTile.Province);
            }
            if (tile.Y < Map.Height - 1)
            {
                var nextTile = Map.Tiles[tile.X, tile.Y + 1];
                if (nextTile.Province != tile.Province)
                    neighbours.Add(nextTile.Province);
            }
        }
        Province minNeighbour = null;
        int minSize = int.MaxValue;
        foreach(var p in neighbours)
        {
            if(minSize > p.Tiles.Count)
            {
                if (minNeighbour != null && minNeighbour.Category == origin.Category && p.Category != origin.Category)
                    continue;
                    minNeighbour = p;
                    minSize = minNeighbour.Tiles.Count;
            }
        }
        cacheTilesList.Clear();
        foreach (var tile in chunk.Tiles)
        {
            cacheTilesList.Add(tile);
            
        }
        foreach (var tile in cacheTilesList)
        {
            Map.AssignTileTo(tile.X, tile.Y, minNeighbour);
        }
    }

    public override void Enable()
    {
        base.Enable();
    }

    public override void Disable()
    {
        base.Disable();
    }
    public override void OnLeft(int x, int y)
    {
        
    }

    public override void OnRightClick(int x, int y)
    {
        
    }
}
