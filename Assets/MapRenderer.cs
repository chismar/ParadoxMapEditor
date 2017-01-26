using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;
using System.IO;
public class MapRenderer : MonoBehaviour
{
    public MapLoader MapLoader;
    public LoaderProgressUI ProgressBar;
    public Material mapMaterial;
    int materialProvinceProp;
    int materialStateProp;
    int materialRegionProp;
    int materialSupplyProp;

    int stateOverlayProp;

	public enum RenderMode { Normal, ProvinceType, StateType, Owner }
	Dictionary<string, Color32> typeColor = new Dictionary<string, Color32>();
    void Awake()
    {
		MapLoader.FinishedLoadingMap += () => { map = MapLoader.Map; LoadColors(); FullRedraw(); };
        materialProvinceProp = Shader.PropertyToID("_SelectedProvinceColor");
        materialStateProp = Shader.PropertyToID("_SelectedStateColor");
        materialRegionProp = Shader.PropertyToID("_SelectedRegionColor");
        materialSupplyProp = Shader.PropertyToID("_SelectedSupplyColor");
        stateOverlayProp = Shader.PropertyToID("_StateOverlay");
	}

	void LoadColors()
	{

		var pathToColors = MapLoader.ChosenProjectDir + "map/MAP_EDITOR_TYPE_COLORS.txt";
		var lines = File.ReadAllLines (pathToColors);
		foreach (var line in lines) {
			var part = line.Split (' ');
			Color32 c = new Color32 (byte.Parse (part [1]), byte.Parse (part [2]), byte.Parse (part [3]), 255);
			typeColor.Add (part [0], c);
		}
	}
	RenderMode mode = RenderMode.Normal;
	public void ChangeMode(RenderMode mode)
	{
		this.mode = mode;
		if (mode != RenderMode.Normal)
			LitUpProvince (null);
		FullRedraw ();
	}
    void Update()
    {
		if (Input.GetKeyUp (KeyCode.Alpha0))
			ChangeMode (RenderMode.Normal);
		if(Input.GetKeyUp(KeyCode.Alpha9))
			ChangeMode(RenderMode.StateType);
		if(Input.GetKeyUp(KeyCode.Alpha8))
			ChangeMode(RenderMode.ProvinceType);
		if(Input.GetKeyUp(KeyCode.Alpha7))
			ChangeMode(RenderMode.Owner);
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
    public int chunkSize = 512;
    IEnumerator CurrentCoroutine;

    IEnumerator CreateChunks()
    {
        chunksReady = false;
        foreach (var chunkGO in chunkGOs)
            Destroy(chunkGO);
        chunkGOs.Clear();
        int chunksWidth = (map.Width / chunkSize + (map.Width % chunkSize == 0 ? 0 : 1));
        int chunksHeight = (map.Height / chunkSize + (map.Height % chunkSize == 0 ? 0 : 1));
        chunks = new Texture2D[chunksWidth, chunksHeight];
        var time = Time.realtimeSinceStartup;
        for (int i = 0; i < chunksWidth; i++)
            for (int j = 0; j < chunksHeight; j++)
            {
                var chunkTex = new Texture2D(chunkSize, chunkSize);
                chunkTex.filterMode = FilterMode.Point;
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
        Camera.main.transform.position = new Vector3(map.Width/2, map.Height/2, -10);
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
                Update(tile.X, tile.Y);
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
    public void FullRedraw()
    {
        ready = false;
        if (CurrentCoroutine != null)
            StopCoroutine(CurrentCoroutine);
        StartCoroutine(CurrentCoroutine = RedrawCoroutine());
    }

    public void Update(Tile tile)
    {
        Update(tile.X, tile.Y);
    }
    public void Update(int x, int y)
    {
        if (y == -1 && y == map.Height)
            return;
        x = x == -1 ? map.Width - 1 : (x == map.Width ? 0 : x);
        int chunkOffsetX;
        int chunkOffsetY;
        var chunkTex = GetChunk(x, y, out chunkOffsetX, out chunkOffsetY);

        Color color = GetColorForTile(map.Tiles[x,y]);

        chunkTex.SetPixel(chunkOffsetX, chunkOffsetY, color);
        
        forUpdate.Add(chunkTex);

    }

    Color32 GetColorForTile(Tile tile)
    {
        Color32 color = Color.clear;
		if (mode == RenderMode.Normal) {
			if (tile.BorderCount > 0) {
				if (tile.Province.StrategicRegion != null && tile.Province.State != null && tile.Province.State.Supply != null) {

					tile.Province.StrategicRegion.TextureColor (ref color);
					tile.Province.State.Supply.TextureColor (ref color);
				}
			} else {
				tile.Province.TextureColor (ref color);
				if (tile.Province.State != null)
					tile.Province.State.TextureColor (ref color);
				else
					color.a = 255;
			}
		} else {
			if (tile.BorderCount == 0)
				color = Color.black;
			else {
				if (mode == RenderMode.ProvinceType)
					typeColor.TryGetValue (tile.Province.OtherType, out color);
				else if (mode == RenderMode.Owner)
				if (tile.Province.State != null && tile.Province.State.Owner != null)
					typeColor.TryGetValue (tile.Province.State.Owner.Tag, out color);
				else if (tile.Province.State != null)
					typeColor.TryGetValue (tile.Province.State.StateCategory, out color);
			}
		}
        return color;
    }
    Texture2D GetChunk(int x, int y, out int chunkOffsetX, out int chunkOffsetY)
    {
        int chunkX = x / chunkSize;
        chunkOffsetX = x - chunkX * chunkSize;
        int chunkY = y / chunkSize;
        chunkOffsetY = y - chunkY * chunkSize;
        return chunks[chunkX, chunkY];
    }

    StringBuilder builder = new StringBuilder();
    public void LitUpProvince(Province province)
    {
        builder.Length = 0;
        builder.Append("Province = ").Append(province.ID).Append(" ");
		if (mode != RenderMode.Normal)
			return;
        if(province != null)
        {
            Color32 color = Color.clear;
            province.TextureColor(ref color);
            mapMaterial.SetColor(materialProvinceProp, color);
            if(province.State != null)
            {

                builder.Append("State = ").Append(province.State.ID).Append(" ");
                province.State.TextureColor(ref color);
                mapMaterial.SetColor(materialStateProp, color);
                mapMaterial.SetFloat(stateOverlayProp, 1f);
                if (province.State.Supply != null && province.StrategicRegion != null)
                {

                    builder.Append("Supply Area = ").Append(province.State.Supply.ID).Append(" ");
                    builder.Append("Strategic Region = ").Append(province.StrategicRegion.ID).Append(" ");
                    province.State.Supply.TextureColor(ref color);
                    mapMaterial.SetColor(materialSupplyProp, color);
                    province.StrategicRegion.TextureColor(ref color);
                    mapMaterial.SetColor(materialRegionProp, color);
                }
                else
                {
                    color = Color.white;
                    mapMaterial.SetColor(materialSupplyProp, color);
                    mapMaterial.SetColor(materialRegionProp, color);
                }
            }
            else
            {
                color = Color.clear;
                mapMaterial.SetColor(materialStateProp, color);
                mapMaterial.SetFloat(stateOverlayProp, 0f);
            }
            
            

        }
        else
        {

            Color32 color = Color.clear;
            mapMaterial.SetColor(materialProvinceProp, color);
            mapMaterial.SetColor(materialStateProp, color);
            color = Color.white;
            mapMaterial.SetColor(materialSupplyProp, color);
            mapMaterial.SetColor(materialRegionProp, color);
            mapMaterial.SetFloat(stateOverlayProp, 0f);
        }
        Debug.Log(builder.ToString());
    }

}
