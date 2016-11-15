using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MapRenderer : MonoBehaviour
{
    public GameObject ChunkProto;
    Map map;
    public Map Map { get { return map; } set { map = value; if(map != null) FullRedraw(); } }
    Texture2D[,] chunks;
    List<GameObject> chunkGOs = new List<GameObject>();
    HashSet<Texture2D> forUpdate = new HashSet<Texture2D>();
    int chunkSize = 32;
    void FullRedraw()
    {
        foreach (var chunkGO in chunkGOs)
            Destroy(chunkGO);
        chunkGOs.Clear();
        int chunksWidth = (map.Width / chunkSize + 1) * chunkSize; 
        int chunksHeight = (map.Height / chunkSize + 1) * chunkSize;
        chunks = new Texture2D[chunksWidth, chunksHeight];
        for ( int i = 0; i <chunksWidth; i++)
            for ( int j = 0; j < chunksHeight; j++)
            {
                var chunkTex = new Texture2D(chunkSize, chunkSize);
                chunks[i, j] = chunkTex;
                GameObject chunkGo = GameObject.Instantiate(ChunkProto);
                var image = chunkGo.GetComponent<Image>();
                var rect = chunkGo.GetComponent<RectTransform>();
                rect.position = new Vector3(i * chunkSize, j * chunkSize, 0);
                rect.sizeDelta = new Vector2(chunkSize, chunkSize);
                image.sprite = Sprite.Create(chunkTex, Rect.MinMaxRect(0, 0, chunkSize, chunkSize), Vector2.zero);
                rect.SetParent(transform, false);
            }
        
        foreach(var province in map.Provinces)
        {
            foreach ( var tile in province.Tiles)
            {
                WritePixelTo(tile.X, tile.Y, province.TemporaryRandomColor);
            }
        }
        foreach (var texToUpdate in forUpdate)
            texToUpdate.Apply();
        forUpdate.Clear();
    }

    void WritePixelTo(int x, int y, Color color)
    {
        int chunkX = x / chunkSize;
        int chunkOffsetX = x - chunkX * chunkSize;
        int chunkY = y / chunkSize;
        int chunkOffsetY = y - chunkY * chunkSize;

        var chunkTex = chunks[chunkX, chunkY];
        chunkTex.SetPixel(chunkOffsetX, chunkOffsetY, color);
        forUpdate.Add(chunkTex);

    }

    void Update()
    {
        if(map != null)
        for ( int i =0; i < map.Width; i++)
            for(int j = 0; j < map.Height; j++)
            {
                var tile = map.Tiles[i, j];
                if(tile.Changed)
                {
                    tile.Changed = false;
                    WritePixelTo(i, j, tile.Province.TemporaryRandomColor);
                }
            }
    }
    
}
