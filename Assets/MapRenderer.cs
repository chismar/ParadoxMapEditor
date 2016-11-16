using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MapRenderer : MonoBehaviour
{
    public MapLoader MapLoader;
    public LoaderProgressUI ProgressBar;
    void Awake()
    {
        MapLoader.FinishedLoadingMap += () => { map = MapLoader.Map; FullRedraw(); };
    }

    void Update()
    {
        if(map == null)
        {
            if (MapLoader.Map != null)
                lock (MapLoader.Map)
                {
                    if (MapLoader.Map.Width > 0 && MapLoader.Map.Height > 0)
                    {
                        map = MapLoader.Map;
                        StartCoroutine(CreateChunks());
                    }
                }
        }
        else if (ready)
        {
            foreach (var chunk in forUpdate)
                chunk.Apply();
            forUpdate.Clear();
        }
    }
    public GameObject ChunkProto;
    Map map;
    bool chunksReady = false;
    bool ready = false;
    Texture2D[,] chunks;
    List<GameObject> chunkGOs = new List<GameObject>();
    HashSet<Texture2D> forUpdate = new HashSet<Texture2D>();
    int chunkSize = 64;
    IEnumerator CurrentCoroutine;

    IEnumerator CreateChunks()
    {
        chunksReady = false;
        foreach (var chunkGO in chunkGOs)
            Destroy(chunkGO);
        chunkGOs.Clear();
        int chunksWidth = (map.Width / chunkSize + 1);
        int chunksHeight = (map.Height / chunkSize + 1);
        chunks = new Texture2D[chunksWidth, chunksHeight];
        var time = Time.realtimeSinceStartup;
        for (int i = 0; i < chunksWidth; i++)
            for (int j = 0; j < chunksHeight; j++)
            {
                var chunkTex = new Texture2D(chunkSize, chunkSize);
                chunks[i, j] = chunkTex;
                GameObject chunkGo = GameObject.Instantiate(ChunkProto);
                var chunkData = chunkGo.GetComponent<ChunkMapController>();
                chunkData.MapOffsetX = i * chunkSize;
                chunkData.MapOffsetY = j * chunkSize;
                chunkData.Size = chunkSize;
                var image = chunkGo.GetComponent<Image>();
                image.sprite = Sprite.Create(chunkTex, Rect.MinMaxRect(0, 0, chunkSize, chunkSize), Vector2.zero);
                chunkGo.transform.SetParent(transform, false);
                chunkGOs.Add(chunkGo);
                if(Time.realtimeSinceStartup - time > 12f)
                    yield return null;
            }
        chunksReady = true;
    }
    IEnumerator RedrawCoroutine()
    {
        ProgressBar.Text = "Creating chunks. Please wait...";
        ProgressBar.MaxProgress = chunks.GetLength(0)*chunks.GetLength(1);
        ProgressBar.Progress = 0;
        while (!chunksReady)
        {
            ProgressBar.Progress = chunkGOs.Count;
            yield return null;
        }
        ProgressBar.Text = "Writing provinces pixel data. Please wait...";
        ProgressBar.MaxProgress = map.Provinces.Count;
        ProgressBar.Progress = 0;
        foreach (var province in map.Provinces)
        {
            foreach (var tile in province.Tiles)
            {
                WritePixelTo(tile.X, tile.Y, province.TemporaryRandomColor);
            }
            ProgressBar.Progress++;
        }
        yield return null;
        ProgressBar.Text = "Creating textures. Please wait...";
        ProgressBar.MaxProgress = forUpdate.Count;
        ProgressBar.Progress = 0;
        foreach (var texToUpdate in forUpdate)
        {
            texToUpdate.Apply();
            ProgressBar.Progress++;
            yield return null;
        }
        ProgressBar.MaxProgress = 0;
        forUpdate.Clear();
        ready = true;
    }
    void FullRedraw()
    {
        ready = false;
        if (CurrentCoroutine != null)
            StopCoroutine(CurrentCoroutine);
        StartCoroutine(CurrentCoroutine = RedrawCoroutine());
    }

    public void WritePixelTo(int x, int y, Color color)
    {
        int chunkX = x / chunkSize;
        int chunkOffsetX = x - chunkX * chunkSize;
        int chunkY = y / chunkSize;
        int chunkOffsetY = y - chunkY * chunkSize;

        var chunkTex = chunks[chunkX, chunkY];
        chunkTex.SetPixel(chunkOffsetX, chunkOffsetY, color);
        forUpdate.Add(chunkTex);

    }
    
    
}
