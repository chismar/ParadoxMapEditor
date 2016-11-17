using UnityEngine;
using System.Collections;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System;
using System.Diagnostics;
public class ProjectSaver : MonoBehaviour
{

    void Awake()
    {
        FindObjectOfType<MapLoader>().FinishedLoadingMap += () =>
        FindObjectOfType<MapModesAndControls>().RegisterCallback(SaveProject, "Save Project");
    }
    public Map Map;
    public Project Project;
    public void SaveProject()
    {
        Map = FindObjectOfType<MapLoader>().Map;

        //Magick starts here
        Bitmap pixels = new Bitmap(Map.Width, Map.Height);
        for ( int i = 0; i < Map.Tiles.GetLength(0);i++)
            for ( int j = 0; j < Map.Tiles.GetLength(1); j++)
            {
                var tile = Map.Tiles[i, j];
                pixels.SetPixel(tile.X, Map.Height - 1 - tile.Y, tile.Province.MapUniqueColor);
            }
        pixels.Save(Project.Directory + "/provinces.png", ImageFormat.Png);
        var path = Application.dataPath + "/StreamingAssets/";
        Process process = new Process();
        process.StartInfo.FileName = path + "PngToBmpEncoder.exe";
        process.StartInfo.Arguments = String.Format("{0} {1} {2}", "\"" + Project.Directory + "\"", "provinces.png", "provinces.bmp");
        UnityEngine.Debug.Log(process.StartInfo.Arguments);
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
        process.Start();
        process.Exited += (o,e) => File.Delete(Project.Directory + "/provinces.png");
        //Ends here



        var defStream = File.Create(Project.Directory + "/definition.csv");
        var adjStream = File.Create(Project.Directory + "/adjacencies.csv");
        
        var encoder = new UTF8Encoding();
        StringBuilder builder = new StringBuilder();
        foreach(var province in Map.Provinces)
        {
            builder.Length = 0;
            builder.Append(province.ID).Append(';');
            builder.Append(province.MapUniqueColor.R).Append(';');
            builder.Append(province.MapUniqueColor.G).Append(';');
            builder.Append(province.MapUniqueColor.B).Append(';');
            builder.Append(province.Type == ProvinceType.Lake? "lake" : (province.Type == ProvinceType.Land? "land" : "sea")).Append(';');
            builder.Append(province.SomeBool?"true":"false").Append(';');
            builder.Append(province.OtherType).Append(';');
            builder.Append(province.LastValue).AppendLine();
            var lineBytes = encoder.GetBytes(builder.ToString());
            defStream.Write(lineBytes,0, lineBytes.Length);
        }
        defStream.Close();

        builder.Length = 0;
        builder.Append("From;To;Type;Through;start_x;start_y;stop_x;stop_y;adjacency_rule_name;Comment").AppendLine();
        var header = encoder.GetBytes(builder.ToString());
        adjStream.Write(header, 0, header.Length);
        HashSet<Adjacency> alreadySaved = new HashSet<Adjacency>();
        foreach ( var province in Map.Provinces)
        {
            foreach ( var adjacency in province.Adjacencies.Values)
            {
                if(alreadySaved.Add(adjacency))
                {
                    builder.Length = 0;
                    builder.Append(adjacency.From.ID).Append(';');
                    builder.Append(adjacency.To.ID).Append(';');
                    builder.Append(adjacency.Type == AdjacencyType.Sea ? "sea" : "").Append(';');
                    builder.Append(adjacency.Through == null? -1 : adjacency.Through.ID).Append(';');
                    builder.Append(adjacency.StartTile == null? -1 : adjacency.StartTile.X).Append(';').Append(adjacency.StartTile == null ? -1 : adjacency.StartTile.Y).Append(';');
                    builder.Append(adjacency.StopTile == null ? -1 : adjacency.StopTile.X).Append(';').Append(adjacency.StopTile == null ? -1 : adjacency.StopTile.Y).Append(';');
                    builder.Append(adjacency.AdjacencyRule == null ? "" : adjacency.AdjacencyRule).Append(';');
                    builder.Append(adjacency.Comment).AppendLine();
                    var lineBytes = encoder.GetBytes(builder.ToString());
                    adjStream.Write(lineBytes, 0, lineBytes.Length);
                }
            }
        }
        var lastLine = encoder.GetBytes("-1;-1;;-1;-1;-1;-1;-1;-1");
        adjStream.Write(lastLine, 0, lastLine.Length);
        adjStream.Close();
    }
}
