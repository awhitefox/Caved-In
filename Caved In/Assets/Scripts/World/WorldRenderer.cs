using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WorldGenerator))]
[RequireComponent(typeof(RoomRenderer))]
public class WorldRenderer : MonoBehaviour
{
    private WorldGenerator worldGenerator;
    private RoomRenderer roomRenderer;
    private Vector2Int playerMapPos;
    private List<Vector2Int> loadedRooms;

    public Transform player;

    private void Awake()
    {
        worldGenerator = GetComponent<WorldGenerator>();
        roomRenderer = GetComponent<RoomRenderer>();
        playerMapPos = Vector2Int.zero;
        loadedRooms = new List<Vector2Int>(5);
    }

    private void OnEnable()
    {
        worldGenerator.WorldGenerated += OnWorldGenerated;
    }

    private void OnDisable()
    {
        worldGenerator.WorldGenerated -= OnWorldGenerated;
    }

    private void FixedUpdate()
    {
        var newPlayerMapPos = RoomGrid.WorldToCellPosition(player.position);

        if (playerMapPos != newPlayerMapPos)
        {
            playerMapPos = newPlayerMapPos;
            if (worldGenerator.Generated)
            {
                UpdateTilemaps();
            }
        }
    }

    private void OnWorldGenerated()
    {
        for (int i = loadedRooms.Count - 1; i >= 0; i--)
        {
            roomRenderer.EraseRoom(loadedRooms.Pop(i));
        }
        UpdateTilemaps();
    }

    private void UpdateTilemaps()
    {
        if (!worldGenerator.MapData.TryGetValue(playerMapPos, out Room currentRoom))
            return;

        // Get rooms that should be loaded
        List<Vector2Int> roomsToLoad = new List<Vector2Int>(5) { playerMapPos };
        foreach (Vector2Int exitVector in currentRoom.Exits.ToVectorList())
        {
            roomsToLoad.Add(playerMapPos + exitVector);
        }

        // Check rooms that are already loaded
        for (int i = loadedRooms.Count - 1; i >= 0; i--)
        {
            Vector2Int roomPos = loadedRooms[i];
            if (roomsToLoad.Contains(roomPos))
            {
                roomsToLoad.Remove(roomPos);
            }
            else
            {
                roomRenderer.EraseRoom(roomPos);
                loadedRooms.RemoveAt(i);
            }
        }

        // Load remaining rooms
        foreach (Vector2Int roomPos in roomsToLoad)
        {
            Room room = worldGenerator.MapData[roomPos];
            roomRenderer.DrawRoom(roomPos, room);
            loadedRooms.Add(roomPos);
        }
    }
}
