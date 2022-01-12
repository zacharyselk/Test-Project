// Creates a graph for a dungeon using a simple drunkard walk PCG method.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Direction {
  public const int North = 0;
  public const int East = 1;
  public const int South = 2;
  public const int West = 3;

  public static int OppositeDirection(int d) {
    return (d+2)%4;
  }

  public static int Random() {
    return UnityEngine.Random.Range(0, 4);
  }
}

public class IntRange {
  public readonly int upperBound;
  public readonly int lowerBound;

  public IntRange(int lower, int upper) {
    lowerBound = lower;
    upperBound = upper;
  }
  public int getValue() {
    return UnityEngine.Random.Range(lowerBound, upperBound);
  }
}

public struct DungeonGraphSettings {
  public IntRange numOfRooms;
  public IntRange roomWidth;
  public IntRange roomHeight;
  public IntRange corridorLength;
  public IntRange corridorWidth;

  public int gridWidth;
  public int gridHeight;
  public bool allowRoomCollisions;

  public DungeonGraphSettings(IntRange numOfRooms, IntRange roomWidth, IntRange roomHeight, IntRange corridorWidth,
                              IntRange corridorLength, int gridWidth, int gridHeight, bool allowRoomCollisions) {
    this.numOfRooms = numOfRooms;
    this.roomWidth = roomWidth;
    this.roomHeight = roomHeight;
    this.corridorLength = corridorLength;
    this.corridorWidth = corridorWidth;
    this.gridWidth = gridWidth;
    this.gridHeight = gridHeight;
    this.allowRoomCollisions = allowRoomCollisions;
  }
}

public struct DungeonRoom {
  public int width;
  public int height;
  public Vector2Int position;

  public DungeonRoom(int width=1, int height=1, Vector2Int? position=null) {
    this.width = width;
    this.height = height;

    if (position == null) {
      this.position = Vector2Int.zero;
    } else {
      this.position = position.Value;
    }
  }

  public int getDimentionOfSide(int direction) {
    if (direction == Direction.North || direction == Direction.South) {
      return width;
    } else {
      return height;
    }
  }
}

public struct DungeonCorridor {
  public int width;
  public int length;
  public int direction;
  public Vector2Int position;
  public DungeonRoom startRoom;
  public DungeonRoom? endRoom;

  public DungeonCorridor(int width, int length, int direction,
                         Vector2Int position, DungeonRoom startRoom, 
                         DungeonRoom? endRoom=null) {
    this.width = width;
    this.length = length;
    this.direction = direction;
    this.position = position;
    this.startRoom = startRoom;

    if (endRoom == null) {
      this.endRoom = new DungeonRoom();
    } else {
      this.endRoom = endRoom.Value;
    }
  }

  public int getGridWidth() {
    if (direction == Direction.North || direction == Direction.South) {
      return width;
    } else {
      return length;
    }
  }

  public int getGridHeight() {
    if (direction == Direction.North || direction == Direction.South) {
      return length;
    } else {
      return width;
    }
  }
}

public class DungeonGraphNode {
  private DungeonRoom room;
  private List<DungeonCorridor> corridors;
}
public class DungeonGraph {
  private DungeonGraphSettings settings;
  private bool[,] grid;
  private List<DungeonRoom> rooms;
  private List<DungeonCorridor> corridors;
  public DungeonGraph(DungeonGraphSettings s) {
    settings = s;

    // Initalize and clear grid.
    grid = new bool[settings.gridWidth, settings.gridHeight];
    for (int y = 0; y < settings.gridHeight; ++y) {
      for (int x = 0; x < settings.gridWidth; ++x) {
        grid[x,y] = false;
      }
    }

    rooms = new List<DungeonRoom>();
    corridors = new List<DungeonCorridor>();

    Assert.IsTrue(settings.gridWidth >= settings.roomWidth.upperBound);
    Assert.IsTrue(settings.gridHeight >= settings.roomHeight.upperBound);
    Assert.IsTrue(settings.corridorWidth.upperBound <= Mathf.Min(settings.roomWidth.lowerBound, settings.roomHeight.lowerBound));

    int currentRoomWidth = settings.roomWidth.getValue();
    int currentRoomHeight = settings.roomHeight.getValue();
    // Top left corner of start room.
    Vector2Int currentRoomPosition = new Vector2Int(
      UnityEngine.Random.Range(Mathf.RoundToInt((settings.gridWidth)/4.0f), Mathf.RoundToInt((settings.gridWidth-currentRoomWidth)/4.0f*3.0f)),
      UnityEngine.Random.Range(Mathf.RoundToInt((settings.gridHeight)/4.0f), Mathf.RoundToInt((settings.gridHeight-currentRoomHeight)/4.0f*3.0f))
    );
    DungeonRoom currentRoom = new DungeonRoom(currentRoomWidth, currentRoomHeight, currentRoomPosition);
    if (!checkGrid(ref currentRoom)) {
      // Exit early if no valid room can be generated.
      Debug.LogWarning("Inital room placed in invalid location!");
      return;
    }
    fillGrid(ref currentRoom);
    rooms.Add(currentRoom);

    int numberOfRooms = settings.numOfRooms.getValue();
    for (int i = 0; i < numberOfRooms; ++i) {
      Tuple<DungeonCorridor, DungeonRoom, bool> result = getAvailableCorridorAndRoom(ref currentRoom);
      if (result.Item3 == false) {
        // Exit early if no valid room can be generated.
        Debug.LogWarning("Exiting generation early!");
        break;
      }
      
      currentRoom = result.Item2;
      fillGrid(ref currentRoom);
      DungeonCorridor c = result.Item1;
      DungeonRoom r = result.Item2;
      corridors.Add(c);
      rooms.Add(r);
    }
  }

  public List<DungeonRoom> getRooms() {
    return rooms;
  }

  public List<DungeonCorridor> GetCorridors() {
    return corridors;
  }

  // Calculates the position of a corridor that branches off of a starting room with a given offset from the
  // room corner.
  private Vector2Int findCorridorPosition(ref DungeonRoom startRoom, ref DungeonCorridor corridor, int offset) {
    int x = 0;
    int y = 0;
    // Choose top-left corner.
    switch (corridor.direction) {
      case Direction.North:
        // Attach south side of corridor to north side of room.
        x = startRoom.position.x + offset;
        y = startRoom.position.y - corridor.length;
        break;
      case Direction.South:
        // Attach north side of corridor to south side of room.
        x = startRoom.position.x + offset;
        y = startRoom.position.y + startRoom.height;
        break;
      case Direction.West:
        // Attach east side of corridor to west side of room.
        x = startRoom.position.x - corridor.length;
        y = startRoom.position.y + offset;
        break;
      case Direction.East:
        // Attach west side of corridor to east side of room.
        x = startRoom.position.x + startRoom.width;
        y = startRoom.position.y + offset;
        break;
      default:
        // Should be unreachable.
        Assert.IsTrue(false);
        break;
    }

    return new Vector2Int(x, y);
  }

  // Calculates the position of a room that attaches to the end of a corridor, with a given offset from the
  // corridor corner.
  private Vector2Int findEndRoomPosition(ref DungeonCorridor corridor, ref DungeonRoom endRoom, int offset) {
    int x = 0;
    int y = 0;
    // Choose top-left corner.
    switch (corridor.direction) {
      case Direction.North:
        // Attach south side of room to north side of corridor.
        x = corridor.position.x - offset;
        y = corridor.position.y - endRoom.height;
        break;
      case Direction.South:
      // Attach north side of room to south side of corridor.
        x = corridor.position.x - offset;
        y = corridor.position.y + corridor.length;
        break;
      case Direction.West:
      // Attach east side of room to west side of corridor.
        x = corridor.position.x - endRoom.width;
        y = corridor.position.y - offset;
        break;
      case Direction.East:
      // Attach west side of room to east side of corridor.
        x = corridor.position.x + corridor.length;
        y = corridor.position.y - offset;
        break;
      default:
        // Should be unreachable.
        Assert.IsTrue(false);
        break;
    }

    return new Vector2Int(x, y);
  }

  private Tuple<DungeonCorridor, DungeonRoom, bool> getAvailableCorridorAndRoom(ref DungeonRoom startRoom) {
    int randomDirection = Direction.Random();
    int corridorWidth = settings.corridorWidth.getValue();
    int corridorLength = settings.corridorLength.getValue();
    int roomWidth = settings.roomWidth.getValue();
    int roomHeight = settings.roomHeight.getValue();
    DungeonCorridor corridor = new DungeonCorridor(corridorWidth, corridorLength, randomDirection, new Vector2Int(0,0), startRoom);
    DungeonRoom endRoom = new DungeonRoom(roomWidth, roomHeight, new Vector2Int(0,0));
    
    for (int i = 0; i < 4; ++i) {
      int direction = (randomDirection+i)%4;
      int startRoomDifference = startRoom.getDimentionOfSide(direction) - corridorWidth;
      int endRoomDifference = endRoom.getDimentionOfSide(Direction.OppositeDirection(direction)) - corridorWidth;
      int startRoomOffset = UnityEngine.Random.Range(0, startRoomDifference);
      int endRoomOffset = UnityEngine.Random.Range(0, endRoomDifference);
      corridor.direction = direction;
      corridor.position = findCorridorPosition(ref startRoom, ref corridor, startRoomOffset);
      endRoom.position = findEndRoomPosition(ref corridor, ref endRoom, endRoomOffset);
      if (checkGrid(ref endRoom)) {
        return new Tuple<DungeonCorridor, DungeonRoom, bool>(corridor, endRoom, true);
      }
    }

    return new Tuple<DungeonCorridor, DungeonRoom, bool>(corridor, endRoom, false);
  }

  private bool checkGrid(ref DungeonRoom room) {
    for (int y = 0; y < room.height; ++y) {
      for (int x = 0; x < room.width; ++x) {
        int xPos = x + room.position.x;
        int yPos = y + room.position.y;
        if (xPos < 0 || yPos < 0 || xPos >= settings.gridWidth || yPos >= settings.gridHeight || (!settings.allowRoomCollisions && grid[xPos, yPos])) {
          return false;
        }
      }
    }
    return true;
  }

  private bool checkGrid(ref DungeonCorridor corridor) {
    int width = corridor.getGridWidth();
    int height = corridor.getGridHeight();
    for (int y = 0; y < height; ++y) {
      for (int x = 0; x < width; ++x) {
        int xPos = x + corridor.position.x;
        int yPos = y + corridor.position.y;
        if (xPos < 0 || yPos < 0 || xPos >= settings.gridWidth || yPos <= settings.gridHeight || (!settings.allowRoomCollisions && grid[xPos, yPos])) {
          return false;
        }
      }
    }
    return true;
  }

  private void fillGrid(ref DungeonRoom room) {
    for (int y = 0; y < room.height; ++y) {
      for (int x = 0; x < room.width; ++x) {
        grid[x+room.position.x, y+room.position.y] = true;
      }
    }
  }

  private void fillGrid(ref DungeonCorridor corridor) {
    int width = corridor.getGridWidth();
    int height = corridor.getGridHeight();
    for (int y = 0; y < height; ++y) {
      for (int x = 0; x < width; ++x) {
        grid[x+corridor.position.x, y+corridor.position.y] = true;
      }
    }
  }
}
