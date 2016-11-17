using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
public class MapLoader : MonoBehaviour {

    public LoaderProgressUI ProgressBar;
    public Texture2D DebugMap;
    public Project ChosenProject;
    public Map Map;
    public event System.Action FinishedLoadingMap;
    public int ProgressMax;
    public int CurrentProgress;
    public object ProgressLock = new object();
    bool finished = false;
    bool shouldContinue = true;
    Thread loadThread;
    void Start()
    {
        Map = ScriptableObject.CreateInstance<Map>();
        shouldContinue = true;
        finished = false;
        loadThread = new Thread(() => LoadFrom(ChosenProject.Directory));
        loadThread.Start();
        StartCoroutine(ThreadWatchdog());
    }

    IEnumerator ThreadWatchdog()
    {
        ProgressBar.Text = "Loading map data. Please wait...";
        while(!finished && shouldContinue)
        {
            yield return null;
        }
        loadThread.Join();
        Debug.Log("Finished loading and joined thread. Map is ready to be used");
        foreach ( var province in Map.Provinces)
            province.TemporaryRandomColor = Random.ColorHSV();
        if (FinishedLoadingMap != null)
            FinishedLoadingMap();
    }
    void OnDestroy()
    {
        shouldContinue = false;
    }
    void LoadFrom(string directory)
    {
        Debug.Log("Loading");
        var provincesFile = File.ReadAllLines(directory + "/definition.csv");
        var provincesMap = new Bitmap(directory + "/provinces.bmp");
        var specialAdjanciesFile = File.ReadAllLines(directory + "/adjacencies.csv");
        lock(Map)
        {
            Map.Height = provincesMap.Height;
            Map.Width = provincesMap.Width;
        }
        lock (ProgressBar)
            ProgressBar.MaxProgress = provincesMap.Width;

        List<Province> provinces = new List<Province>();
        Dictionary<int, Province> provincesByID = new Dictionary<int, Province>();
        Dictionary<System.Drawing.Color, Province> provincesByColor = new Dictionary<System.Drawing.Color, Province>();
        for ( int i = 0; i < provincesFile.Length; i++)
        {
            var province = new Province();
            var stringData = provincesFile[i].Split(';');
            
            province.ID = int.Parse(stringData[0]);
            var red = byte.Parse(stringData[1]);
            var green = byte.Parse(stringData[2]);
            var blue = byte.Parse(stringData[3]);

            province.Type = stringData[4] == "land" ? ProvinceType.Land : stringData[4] == "sea" ? ProvinceType.Sea : ProvinceType.Lake;
            province.SomeBool = stringData[5] == "true";
            province.OtherType = stringData[6];
            province.LastValue = int.Parse(stringData[7]);

            provinces.Add(province);
            var uniqueColor = System.Drawing.Color.FromArgb(red, green, blue);
            province.MapUniqueColor = uniqueColor;
            provincesByColor.Add(uniqueColor, province);
            provincesByID.Add(province.ID, province);

            if (!shouldContinue)
                return;
            //lock (ProgressLock)
            //    CurrentProgress++;
        }
        
        //DebugMap = ReadBMPToTexture(directory + "/provinces.bmp");
        Debug.Log("Loaded provinces, now loading map");
        
        var tiles = new Tile[provincesMap.Width, provincesMap.Height];
        Debug.Log(provincesMap.Width);
        Debug.Log(provincesMap.Height);
        for ( int x = 0; x < provincesMap.Width; x++)
        {
            for ( int y = 0; y < provincesMap.Height; y++)
            {
                var tile = new Tile();
                tiles[x, y] = tile;
                tile.X = x;
                tile.Y = y;
                tile.Province = provincesByColor[provincesMap.GetPixel(x, provincesMap.Height - 1 - y)];
            }
            if (!shouldContinue)
                return;

            lock (ProgressBar)
                ProgressBar.Progress = x;

        }

        Debug.Log("Loaded provinces map, reading adjacencies");

        try
        {
            //-1 because of the last dummy line
            for ( int i = 1; i < specialAdjanciesFile.Length - 1; i++)
        {
            var adjacency = new Adjacency();
            var lineData = specialAdjanciesFile[i].Split(';');
            adjacency.From = provincesByID[int.Parse(lineData[0])];
            adjacency.To = provincesByID[int.Parse(lineData[1])];
            adjacency.Type = lineData[2] == "sea" ? AdjacencyType.Sea : AdjacencyType.Land;
            int throughIndex = -1;
            if (int.TryParse(lineData[3], out throughIndex) && throughIndex != -1)
                adjacency.Through = provincesByID[throughIndex];
            int startX = -1;
            int startY = -1;
            int stopX = -1;
            int stopY = -1;
            if (int.TryParse(lineData[4], out startX) && int.TryParse(lineData[5], out startY) && int.TryParse(lineData[6], out stopX) && int.TryParse(lineData[7], out stopY))
            {
                if(startX != -1 && startY != -1)
                    adjacency.StartTile = tiles[startX, startY];
                if(stopX != -1 && stopY != -1)
                    adjacency.StopTile = tiles[stopX, stopY];
            }

            adjacency.AdjacencyRule = lineData[8];
            adjacency.Comment = lineData[9];
            adjacency.From.Adjacencies.Add(adjacency.To, adjacency);
            adjacency.To.Adjacencies.Add(adjacency.From, adjacency);
         

            if (!shouldContinue)
                return;
            //lock (ProgressLock)
            //    CurrentProgress++;
        }
        Debug.Log("Setting map data");

            Map.Provinces = provinces;
            Map.ColorCodedProvinces = provincesByColor;
            Map.Tiles = tiles;
            Map.Width = tiles.GetLength(0);
            Map.Height = tiles.GetLength(1);
        }
        catch ( System.Exception e)
        {
            Debug.Log(e);
        }

        Debug.Log("Map data set");
        shouldContinue = false;
        finished = true;
        provincesMap.Dispose();
    }

    Texture2D ReadBMPToTexture(string path)
    {
        Texture2D texture = new Texture2D(2, 2);
        Bitmap bitmap = new Bitmap(path);        
        byte[] result = null;
        using (MemoryStream stream = new MemoryStream())
        {
            bitmap.Save(stream, ImageFormat.Png);
            result = stream.ToArray();
        }
        texture.LoadImage(result);
        return texture;
    }
}
