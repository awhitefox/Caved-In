using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Map))]
public class MapRenderer : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private Transform player;
    [SerializeField] private int drawDistance;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private TileBase groundTile;
    [SerializeField] private TileSelectorBase wallTileSelector;
#pragma warning restore 0649

    private Map map;
    private Vector2Int playerPos;
    private List<Vector2Int> loadedTiles; // TODO Consider using HashSet

    private void Awake()
    {
        map = GetComponent<Map>();
        playerPos = new Vector2Int(int.MaxValue, int.MaxValue); // FIXME
        loadedTiles = new List<Vector2Int>();
    }

    private void Update()
    {
        if (map.TileCount == 0)
            return;

        var newPlayerPos = new Vector2Int((int)player.position.x, (int)player.position.y);
        if (newPlayerPos != playerPos)
        {
            playerPos = newPlayerPos;
            UpdateTilemaps();
        }
    }

    private void UpdateTilemaps()
    {
        // Get tiles that should be loaded
        var tilesToLoad = new List<Vector2Int>();
        for (int y = playerPos.y - drawDistance; y <= playerPos.y + drawDistance; y++)
        {
            for (int x = playerPos.x - drawDistance; x <= playerPos.x + drawDistance; x++)
            {
                tilesToLoad.Add(new Vector2Int(x, y));
            }
        }

        // Check tiles that are already loaded
        for (int i = loadedTiles.Count - 1; i >= 0; i--)
        {
            int j = tilesToLoad.IndexOf(loadedTiles[i]);
            if (j >= 0)
            {
                tilesToLoad.RemoveAt(j);
            }
            else
            {
                EraseTile(loadedTiles[i]);
                loadedTiles.RemoveAt(i);
            }
        }

        // Load remaining tiles
        foreach (Vector2Int pos in tilesToLoad)
        {
            DrawTile(pos);
            loadedTiles.Add(pos);
        }
    }

    private void DrawTile(Vector2Int tilePos)
    {
        if (!map.TryGetTile(tilePos, out TileType tileType))
            return;
        var pos = (Vector3Int)tilePos;
        switch (tileType)
        {
            case TileType.Ground:
                groundTilemap.SetTile(pos, groundTile);
                break;
            case TileType.Wall:
                groundTilemap.SetTile(pos, groundTile);
                wallTilemap.SetTile(pos, wallTileSelector.Pick(tilePos, map));
                break;
        }
    }

    private void EraseTile(Vector2Int tilePos)
    {
        var pos = (Vector3Int)tilePos;
        groundTilemap.SetTile(pos, null);
        wallTilemap.SetTile(pos, null);
    }
}
