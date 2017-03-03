using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class CreateNewProvinceMapMode : MapMode
{
    Province selectedProvince;

    public string Type;
    public bool SomeBool;
    public int Continent;
    string OtherType;

    float intermediateBrushSize = 3;
    int brushSize = 3;
    public RectTransform Border;
    static MapModesAndControls controls;
    static Controller controller;
    UnityEngine.UI.Text provinceSelection;
    UnityEngine.UI.Text brushSizeText;
    UnityEngine.UI.Text initialFillerText;
    void OnEnable()
    {
        if (controls == null && controller == null)
        {
            controls = FindObjectOfType<MapModesAndControls>();
            controller = FindObjectOfType<Controller>();
        }
        controls.RegisterCallback(() => controller.SelectMapMode(this), "Province creation");

    }
    bool CopyAndFillSome = false;
    void Update()
    {
        if (enabled)
        {
            if (Input.GetKeyUp(KeyCode.F))
            {
                CopyAndFillSome = !CopyAndFillSome;
                initialFillerText.text = "[F]Do initial filling = " + CopyAndFillSome;
            }
            var scroll = Input.mouseScrollDelta.y;
            if (scroll > 0)
            {
                brushSize = (int)(brushSize * (1 + scroll));
            }
            intermediateBrushSize += scroll;
            var newBrush = (int)intermediateBrushSize;
            if (newBrush != brushSize)
                brushSizeText.text = "Brush size: " + newBrush;
            brushSize = newBrush;
            if (brushSize <= 0)
            {
                brushSize = 1;
                intermediateBrushSize = 1f;
                brushSizeText.text = "Brush size: " + brushSize;
            }

            float scale = brushSize / Border.sizeDelta.x;
            Border.localScale = new Vector3(scale, scale, 1) * Renderer.transform.localScale.x;
            Border.position = Input.mousePosition;

        }

    }

    public override void Enable()
    {
        base.Enable();
        Border.gameObject.SetActive(true);
        selectedProvince = null;
        brushSizeText = dataPanel.PostString("Brush size: " + brushSize.ToString());
        provinceSelection = dataPanel.PostString("Province not selected yet");
        initialFillerText = dataPanel.PostString("[F]Do initial filling = " + CopyAndFillSome);
    }

    public override void Disable()
    {
        base.Disable();
        Border.gameObject.SetActive(false);
        Destroy(brushSizeText.gameObject);
        Destroy(provinceSelection.gameObject);
        Destroy(initialFillerText.gameObject);
    }
    public override void OnLeft(int x, int y)
    {
        if (CopyAndFillSome)
        {
            var selectedProv = Map.Tiles[x, y].Province;
            selectedProvince = Map.CreateNewProvince(selectedProv.Category, selectedProv.SomeBool, selectedProv.Type, selectedProv.Continent, selectedProv);

            Renderer.LitUpProvince(selectedProvince);
            provinceSelection.text = "Province created: " + selectedProvince.ID;

            InitialFill(x, y, selectedProv, selectedProvince);
            Renderer.Update(selectedProv);
            Renderer.Update(selectedProvince);
        }
        else
        {
            selectedProvince = Map.CreateNewProvince(Type, SomeBool, OtherType, Continent);
            Renderer.LitUpProvince(selectedProvince);
            provinceSelection.text = "Province created: " + selectedProvince.ID;
            OnRightDrag(x, y);
        }


    }
    System.Random rand = new System.Random();
    int[] dirs = new int[4] { 0, 1, 2, 3 };
    void ShuffleDirs()
    {
        int n = dirs.Length;
        while (n > 1)
        {
            n--;
            int k = rand.Next(n + 1);
            int value = dirs[k];
            dirs[k] = dirs[n];
            dirs[n] = value;
        }
    }

    List<Tile> nonFrontierTiles = new List<Tile>();
    List<Tile> frontierTiles = new List<Tile>();
    HashSet<Tile> usedTiles = new HashSet<Tile>();
    List<Tile> newFrontierTiles = new List<Tile>();
    private void InitialFill(int startX, int startY, Province startProvince, Province newProvince)
    {
        int count = startProvince.Tiles.Count / 2;
        //newProvince.AttachTile();
        nonFrontierTiles.Clear();
        usedTiles.Clear();
        frontierTiles.Clear();
        frontierTiles.Add(Map.Tiles[startX, startY]);
        usedTiles.Add(frontierTiles[0]);
        while ((frontierTiles.Count + nonFrontierTiles.Count) < count)
        {
            //newFrontierTiles.Clear();
            int i = rand.Next(0, frontierTiles.Count);
            var tile = frontierTiles[i];
            //for (int i = 0; i < frontierTiles.Count && (frontierTiles.Count + nonFrontierTiles.Count + newFrontierTiles.Count) < count; i++)
            //{
            //    var tile = frontierTiles[i];
                Tile nextTile = null;
                int tries = 0;
                ShuffleDirs();
                while(tries < 4 && nextTile == null)
                {
                    int dir = dirs[tries++];    
                    switch (dir)
                    {
                        case 0:
                            if (Map.Width == tile.X)
                                break;
                            nextTile = Map.Tiles[tile.X + 1, tile.Y];
                            break;
                        case 1:
                            if (Map.Width == 0)
                                break;
                            nextTile = Map.Tiles[tile.X - 1, tile.Y];
                            break;
                        case 2:
                            if (Map.Height == tile.Y)
                                break;
                            nextTile = Map.Tiles[tile.X, tile.Y + 1];
                            break;
                        case 3:
                            if (Map.Height == 0)
                                break;
                            nextTile = Map.Tiles[tile.X, tile.Y - 1];
                            break;
                    }


                //if (nextTile.Province.Type != startProvince.Type)
                if (nextTile.Province != startProvince)
                {
                        nextTile = null;
                    }

                    if (usedTiles.Contains(nextTile))
                        nextTile = null;
                }
                if (nextTile == null)
                {
                    nonFrontierTiles.Add(tile);
                    frontierTiles.RemoveAt(i);
                    i--;
                    continue;
                }
                else
                {
                    usedTiles.Add(nextTile);
                    frontierTiles.Add(nextTile);
                }
            //}
            //foreach (var newTile in newFrontierTiles)
            //    frontierTiles.Add(newTile);
            //newFrontierTiles.Clear();
        }


        foreach (var tile in nonFrontierTiles)
            Map.AssignTileTo(tile.X, tile.Y, newProvince);

        foreach (var tile in frontierTiles)
            Map.AssignTileTo(tile.X, tile.Y, newProvince);
    }


    public override void OnRightClick(int x, int y)
    {
        OnRightDrag(x, y);
    }
    public override void OnRightDrag(int x, int y)
    {
        if (selectedProvince == null)
            return;
        int leftX = x - (brushSize - 1) / 2;
        int rightX = x + brushSize / 2;
        int lowY = y - (brushSize - 1) / 2;
        int highY = y + brushSize / 2;
        for (int i = leftX; i <= rightX; i++)
        {
            for (int j = lowY; j <= highY; j++)
            {
                Map.AssignTileTo(i, j, selectedProvince);
            }
        }
        for (int i = leftX - 1; i <= rightX + 1; i++)
            for (int j = lowY - 1; j <= highY + 1; j++)
                Renderer.Update(i, j);
    }
}
