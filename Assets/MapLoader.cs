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
        var dir = PlayerPrefs.GetString("directory");
        loadThread = new Thread(() => LoadFrom(dir));
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

        if (FinishedLoadingMap != null)
            FinishedLoadingMap();
    }
    void OnDestroy()
    {
        shouldContinue = false;
    }

    List<State> LoadStates(string basePath, Dictionary<int, Province> provinces)
    {
        List<State> states = new List<State>();
        Debug.Log("Loading states");
        var files = Directory.GetFiles(basePath + "/history/states");
        lock (ProgressBar)
        {
            ProgressBar.MaxProgress = files.Length;
            ProgressBar.Progress = 0;
        }
            
        foreach ( var file in files)
        {
            try
            {
                State state = new State();
                var table = ScriptsLoader.LoadScript(file);
                //Debug.Log(table);
                state.ID = table.Get<ScriptValue>("id").IntValue();
                state.Name = table.Get<ScriptValue>("name").StringValue();
                state.StateCategory = table.Get<ScriptValue>("state_category").StringValue();
                state.Manpower = table.Get<ScriptValue>("manpower").IntValue();

                var provincesList = table.Get<ScriptList>("provinces").AllData;
                foreach (var id in provincesList)
                {
                    Province p = null;
                    if (provinces.TryGetValue(id.Key.IntValue(), out p))
                    {

                        //state.Provinces.Add(p);
                        p.State = state;
                    }
                }
                states.Add(state);
            }
            catch(System.Exception e)
            {
                Debug.LogFormat("Can't parse file {0}", file);
                Debug.Log(e);
            }
            lock (ProgressBar)
                ProgressBar.Progress++;
        }
        return states;
    }

    List<StrategicRegion> LoadRegions(string basePath, Dictionary<int, Province> provinces)
    {
        Debug.Log("Loading regions");
        List<StrategicRegion> regions = new List<StrategicRegion>();
        var files = Directory.GetFiles(basePath + "/map/strategicregions");
        lock (ProgressBar)
        {
            ProgressBar.MaxProgress = files.Length;
            ProgressBar.Progress = 0;
        }
        foreach ( var file in files)
        {
            try
            { 
                StrategicRegion region = new StrategicRegion();
                var table = ScriptsLoader.LoadScript(file);
                region.ID = table.Get<ScriptValue>("id").IntValue();
                region.Name = table.Get<ScriptValue>("name").StringValue();
                var provincesList = table.Get<ScriptList>("provinces").AllData;
                foreach (var id in provincesList)
                {
                    Province p = null;
                    if (provinces.TryGetValue(id.Key.IntValue(), out p))
                    {

                        //region.Provinces.Add(p);
                        p.StrategicRegion = region;
                    }
                }
                regions.Add(region);
            }
                catch (System.Exception e)
            {
                Debug.LogFormat("Can't parse file {0}", file);
                Debug.Log(e);
            }
        lock (ProgressBar)
                ProgressBar.Progress++;
        }
        return regions;
    }

    List<SupplyArea> LoadAreas(string basePath, Dictionary<int, State> states)
    {

        Debug.Log("Loading areas");
        List<SupplyArea> areas = new List<SupplyArea>();
        var files = Directory.GetFiles(basePath + "/map/supplyareas");
        lock (ProgressBar)
        {
            ProgressBar.MaxProgress = files.Length;
            ProgressBar.Progress = 0;
        }
        foreach (var file in files)
        {
            try
            {

                SupplyArea area = new SupplyArea();
                var table = ScriptsLoader.LoadScript(file);
                area.ID = table.Get<ScriptValue>("id").IntValue();
                area.SupplyValue = table.Get<ScriptValue>("value").IntValue();
                area.Name = table.Get<ScriptValue>("name").StringValue();
                var state = table.Get<ScriptList>("states").AllData;
                foreach (var id in state)
                {
                    State s = null;
                    if (states.TryGetValue(id.Key.IntValue(), out s))
                    {
                        //area.States.Add(s);
                        s.Supply = area;
                    }
                }
                areas.Add(area);
            }
            catch (System.Exception e)
            {
                Debug.LogFormat("Can't parse file {0}", file);
                Debug.Log(e);
            }
            lock (ProgressBar)
                ProgressBar.Progress++;
        }

        return areas;
    }
    void LoadFrom(string directory)
    {
        try
        {
            Debug.Log("Loading");
            var provincesFile = File.ReadAllLines(directory + "/map/definition.csv");
            var provincesMap = new Bitmap(directory + "/map/provinces.bmp");
            var specialAdjanciesFile = File.ReadAllLines(directory + "/map/adjacencies.csv");
            lock(Map)
            {
                Map.Height = provincesMap.Height;
                Map.Width = provincesMap.Width;
            }

            List<Province> provinces = new List<Province>();
            Dictionary<int, Province> provincesByID = new Dictionary<int, Province>();
            Dictionary<System.Drawing.Color, Province> provincesByColor = new Dictionary<System.Drawing.Color, Province>();
            int maxID = 0;
            HashSet<Color32> textureColors = new HashSet<Color32>();
            for ( int i = 0; i < provincesFile.Length; i++)
            {
                var province = new Province();
                var stringData = provincesFile[i].Split(';');
            
                province.ID = int.Parse(stringData[0]);
                if (province.ID > maxID)
                    maxID = province.ID;
                var red = byte.Parse(stringData[1]);
                var green = byte.Parse(stringData[2]);
                var blue = byte.Parse(stringData[3]);

                province.Type = stringData[4] == "land" ? ProvinceType.Land : stringData[4] == "sea" ? ProvinceType.Sea : ProvinceType.Lake;
                province.SomeBool = stringData[5] == "true";
                province.OtherType = stringData[6];
                province.Continent = int.Parse(stringData[7]);

                provinces.Add(province);
                var uniqueColor = System.Drawing.Color.FromArgb(red, green, blue);
                province.MapUniqueColor = uniqueColor;
                provincesByColor.Add(uniqueColor, province);
                provincesByID.Add(province.ID, province);
                Color32 color = new Color32(0, 0, 0, 0);
                province.TextureColor(ref color);
                if(!textureColors.Add(color))
                {
                    Debug.LogFormat("Duplicate {0} = {1} {2}", province.ID, color.r, color.g);
                }
                //Debug.LogFormat("{0} = {1} {2}", province.ID, color.r, color.g);
                if (!shouldContinue)
                    return;
                //lock (ProgressLock)
                //    CurrentProgress++;
            }
            var states = LoadStates(directory, provincesByID);
            var regions = LoadRegions(directory, provincesByID);
            Dictionary<int, State> statesByID = new Dictionary<int, State>();
            foreach (var state in states)
                statesByID.Add(state.ID, state);
            var areas = LoadAreas(directory, statesByID);
            //DebugMap = ReadBMPToTexture(directory + "/provinces.bmp");
            Debug.Log("Loaded provinces, now loading map");

            lock (ProgressBar)
                ProgressBar.MaxProgress = provincesMap.Width;
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
                }
                if (!shouldContinue)
                    return;

                lock (ProgressBar)
                    ProgressBar.Progress = x;

            }

            Debug.Log("Loaded provinces, assigning map");
            Map.Tiles = tiles;
            
            for (int x = 0; x < provincesMap.Width; x++)
            {
                for (int y = 0; y < provincesMap.Height; y++)
                {
                    var color = provincesMap.GetPixel(x, provincesMap.Height - 1 - y);
                    Province prov = null;
                    if(provincesByColor.TryGetValue(color, out prov))
                        Map.AssignTileTo(x, y, prov);
                    else
                    {
                        UnityEngine.Debug.LogWarningFormat("Cant find a province with a color: {0} {1} {2} in position: {3} {4}", color.R, color.G, color.B, x, y);
                    }
                }
                lock (ProgressBar)
                    ProgressBar.Progress = x;
            }
                
            
        
            Debug.Log("Loaded provinces map, reading adjacencies");

        
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
            provincesByColor.Clear();
            foreach ( var province in provinces)
            {
                province.MapUniqueColor = province.SerializedColor();
                provincesByColor.Add(province.MapUniqueColor, province);
            }
            Debug.Log("Setting map data");

            Map.Provinces = provinces;
            Map.ColorCodedProvinces = provincesByColor;
            Map.ProvincesByID = provincesByID;
            Map.NextID = maxID + 1;
            Map.SupplyAreas = areas;
            Map.StrategicRegions = regions;
            Map.States = states;
            Debug.Log("Map data set");
            shouldContinue = false;
            finished = true;
            provincesMap.Dispose();
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }

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
