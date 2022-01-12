using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public struct DungeonObject {
  public String name;
  public Vector2Int[] positions;
  public TileBase[] tiles;
  public int width;
  public int height;

  public bool canDraw(ref bool[,] grid, int gridWidth, int gridHeight, int x, int y) {
    for (int j = 0; j < height; ++j) {
      for (int i = 0; i < width; ++i) {
        if (x+i < gridWidth && y+j < gridHeight && grid[x+i, y+j] == false) {
          return false;
        }
      }
    }
    
    return true;
  }
}

public class DungeonGenerator : MonoBehaviour {
  public GameObject player;
  public Tilemap groundTilemap;
  public Tilemap collisionTilemap;
  public Tilemap decorativeTilemap;
  public TileBase[] groundTiles;
  public TileBase[] wallTiles;
  public DungeonObject[] groundObjects;
  public DungeonObject[] decrativeObjects;
  public int mapWidth = 100;
  public int mapHeight = 100;
  public int minNumberOfRooms = 5;
  public int maxNumberOfRooms = 10;
  public int minRoomWidth = 5;
  public int maxRoomWidth = 8;
  public int minRoomHeight = 5;
  public int maxRoomHeight = 8;
  public int minCorridorWidth = 1;
  public int maxCorridorWidth = 5;
  public int minCorridorLength = 2;
  public int maxCorridorLength = 8;
  public float objectFrequency = 0.01f;
  public float decorationFrequency = 0.1f;
  public bool allowRoomCollisions = false;

  // Start is called before the first frame update
  void Start() {
    // Generate dungeon.
    DungeonGraphSettings settings = new DungeonGraphSettings(
      new IntRange(minNumberOfRooms, maxNumberOfRooms),
      new IntRange(minRoomWidth, maxRoomWidth),
      new IntRange(minRoomHeight, maxRoomHeight),
      new IntRange(minCorridorWidth, maxCorridorWidth),
      new IntRange(minCorridorLength, maxCorridorLength),
      mapWidth, mapHeight, allowRoomCollisions
    );
    DungeonGraph graph = new DungeonGraph(settings);

    bool[,] grid = new bool[mapWidth, mapHeight];
    for (int y = 0; y < mapHeight; ++y) {
      for (int x = 0; x < mapWidth; ++x) {
        grid[x, y] = false;
      }
    }
    List<DungeonRoom> rooms = graph.getRooms();
    List<DungeonCorridor> corridors = graph.GetCorridors();


    //Debug.Log(string.Format("Found {0} rooms and {1} corridors", rooms.Count, corridors.Count));

    // Place player in the initial room of the dungeon.
    if (rooms.Count > 0) {
      int centerX = rooms[0].position.x + (rooms[0].width/2);
      int centerY = rooms[0].position.y + (rooms[0].height/2);
      //Instantiate(player, new Vector3(centerX, centerY, 0), Quaternion.identity);
      player.transform.position = new Vector3(centerX, centerY, 0);
    }

    // Draw the dungeon onto a boolean map.
    for (int i = 0; i < rooms.Count; ++i) {
      DungeonRoom room = rooms[i];
      //Debug.Log(string.Format("  Room {0} starts at position ({1},{2}) with a width of {3} and a height of {4}", i, room.position.x, room.position.y, room.width, room.height));
      for (int y = 0; y < room.height; ++y) {
        for (int x = 0; x < room.width; ++x) {
          grid[x+room.position.x, y+room.position.y] = true;
        }
      }
    }

    for (int i = 0; i < corridors.Count; ++i) {
      DungeonCorridor corridor = corridors[i];
      int height = corridor.getGridHeight();
      int width = corridor.getGridWidth();
      for (int y = 0; y < height; ++y) {
        for (int x = 0; x < width; ++x) {
          grid[x+corridor.position.x, y+corridor.position.y] = true;
        }
      }
    }

    Vector3Int size = groundTilemap.size;
    for (int i = 0; i < mapHeight; ++i) {
      for (int j = 0; j < mapWidth; ++j) {
        //if (i == 0 || i == mapHeight-1 || j == 0 || j == mapWidth-1) {
        //  this.tileMap.SetTile(new Vector3Int(j, i, 0), wallTiles[Random.Range(0, wallTiles.Length-1)]);
        //} else {
        //  this.tileMap.SetTile(new Vector3Int(j, i, 0), groundTiles[Random.Range(0, groundTiles.Length-1)]);
        //}
        
        this.groundTilemap.SetTile(new Vector3Int(j, i, 0), groundTiles[UnityEngine.Random.Range(0, groundTiles.Length-1)]);
        if (grid[j, i] == false) {
          this.collisionTilemap.SetTile(new Vector3Int(j, i, 0), wallTiles[UnityEngine.Random.Range(0, wallTiles.Length-1)]);
        }
      }
    }

    // Spawn objects.
    for (int y = 0; y < mapHeight; ++y) {
      for (int x = 0; x < mapWidth; ++x) {
        float randomValue = UnityEngine.Random.Range(0.0f, 1.0f);
        if (randomValue <= objectFrequency) {
          // Attempt to create a random object.
          DungeonObject randomObject = groundObjects[UnityEngine.Random.Range(0, groundObjects.Length-1)];
          if (randomObject.canDraw(ref grid, mapWidth, mapHeight, x, y)) {
            for (int i = 0; i < randomObject.positions.Length; ++i) {
              int xPos = x + randomObject.positions[i].x;
              int yPos = y + randomObject.positions[i].y;
              grid[xPos, yPos] = false; // TODO probably remove this and replace it with something better.
              this.collisionTilemap.SetTile(new Vector3Int(xPos, yPos, 0), randomObject.tiles[i]);
            }
          }
        }
      }
    }

    // Spawn decorations.
    for (int y = 0; y < mapHeight; ++y) {
      for (int x = 0; x < mapWidth; ++x) {
        float randomValue = UnityEngine.Random.Range(0.0f, 1.0f);
        if (randomValue <= decorationFrequency) {
          // Attempt to create a random object.
          DungeonObject randomObject = decrativeObjects[UnityEngine.Random.Range(0, decrativeObjects.Length-1)];
          if (randomObject.canDraw(ref grid, mapWidth, mapHeight, x, y)) {
            for (int i = 0; i < randomObject.positions.Length; ++i) {
              int xPos = x + randomObject.positions[i].x;
              int yPos = y + randomObject.positions[i].y;
              grid[xPos, yPos] = false; // TODO probably remove this and replace it with something better.
              this.decorativeTilemap.SetTile(new Vector3Int(xPos, yPos, 0), randomObject.tiles[i]);
            }
          }
        }
      }
    }
  }
  // Update is called once per frame
  void Update()
  {
      
  }
}
