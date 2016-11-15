using UnityEngine;
using System.Collections;

public class Adjacency
{
    public Province From;
    public Province To;
    public Province Through;
    public AdjacencyType Type;
    public Tile StartTile;
    public Tile StopTile;
    public object AdjacencyRule;
    public string Comment;
    
}

public enum AdjacencyType { Land, Sea }