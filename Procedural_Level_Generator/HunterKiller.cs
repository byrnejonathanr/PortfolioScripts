using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterKiller
{

    private Room[,] roomArray = null;
    private EnvironmentLibrary environmentLibrary;
    private int labRows, labColumns;
    private int currentRow = 0;
    private int currentColumn = 0;
    private int currentType = 0;
    private bool runComplete = false;

    public HunterKiller(Room[,] roomArray, EnvironmentLibrary environmentLibrary)
    {
        this.roomArray = roomArray;
        this.environmentLibrary = environmentLibrary;
        labRows = roomArray.GetLength(0);
        labColumns = roomArray.GetLength(1);
        currentType = Random.Range(0, 4);
        roomArray[0, 0].roomType = currentType;
    }

    public void HuntAndKill()
    {
        roomArray[currentRow, currentColumn].visited = true;
        while ( !runComplete )
        {
            Kill();
            Hunt();
        }
    }

    private void Kill()
    {
        while (AdjacentUnvisited(currentRow, currentColumn))
        {
            int direction = Random.Range(0, 4);

            switch (direction)
            {
                case 0: // North break
                    if (currentRow > 0 && !roomArray[currentRow - 1, currentColumn].visited)
                    {
                        roomArray[currentRow - 1, currentColumn].southWall = BreakWallIfExists(roomArray[currentRow - 1, currentColumn].southWall);
                        roomArray[currentRow - 1, currentColumn].roomType = currentType;
                        if (roomArray[currentRow - 1, currentColumn].southWall != null) roomArray[currentRow - 1, currentColumn].southWall.transform.SetParent(roomArray[currentRow - 1, currentColumn].transform);
                        roomArray[currentRow - 1, currentColumn].southOpen = true;
                        roomArray[currentRow, currentColumn].northOpen = true;
                        currentRow--;
                    }
                    break;
                case 1: // South break
                    if (currentRow < labRows - 1 && !roomArray[currentRow + 1, currentColumn].visited)
                    {
                        roomArray[currentRow, currentColumn].southWall = BreakWallIfExists(roomArray[currentRow, currentColumn].southWall);
                        roomArray[currentRow + 1, currentColumn].roomType = currentType;
                        if (roomArray[currentRow, currentColumn].southWall != null) roomArray[currentRow, currentColumn].southWall.transform.SetParent(roomArray[currentRow, currentColumn].transform);
                        roomArray[currentRow, currentColumn].southOpen = true;
                        roomArray[currentRow + 1, currentColumn].northOpen = true;
                        currentRow++;
                    }
                    break;
                case 2: // West break
                    if (currentColumn > 0 && !roomArray[currentRow, currentColumn - 1].visited)
                    {
                        roomArray[currentRow, currentColumn - 1].eastWall = BreakWallIfExists(roomArray[currentRow, currentColumn - 1].eastWall);
                        roomArray[currentRow, currentColumn - 1].roomType = currentType;
                        if (roomArray[currentRow, currentColumn - 1].eastWall != null) roomArray[currentRow, currentColumn - 1].eastWall.transform.SetParent(roomArray[currentRow, currentColumn - 1].transform);
                        roomArray[currentRow, currentColumn - 1].eastOpen = true;
                        roomArray[currentRow, currentColumn].westOpen = true;
                        currentColumn--;
                    }
                    break;
                case 3: // East break
                    if (currentColumn < labColumns - 1 && !roomArray[currentRow, currentColumn + 1].visited)
                    {
                        roomArray[currentRow, currentColumn].eastWall = BreakWallIfExists(roomArray[currentRow, currentColumn].eastWall);
                        roomArray[currentRow, currentColumn + 1].roomType = currentType;
                        if (roomArray[currentRow, currentColumn].eastWall != null) roomArray[currentRow, currentColumn].eastWall.transform.SetParent(roomArray[currentRow, currentColumn].transform);
                        roomArray[currentRow, currentColumn].eastOpen = true;
                        roomArray[currentRow, currentColumn + 1].westOpen = true;
                        currentColumn++;
                    }
                    break;
            }

            roomArray[currentRow, currentColumn].visited = true;
        }

    }

    private void Hunt()
    {
        runComplete = true;

        for (int r = 0; r < labRows; r++)
        {
            for (int c = 0; c < labColumns; c++)
            {
                if (AdjacentUnvisited(r, c))
                {
                    runComplete = false;
                    currentRow = r;
                    currentColumn = c;
                    currentType = roomArray[r, c].roomType;
                    return;
                }
            }
        }
    }

    private bool AdjacentUnvisited(int row, int column)
    {
        if (row > 0 && !roomArray[row - 1, column].visited) return true; // North check
        if (row < labRows - 1 && !roomArray[row + 1, column].visited) return true; // South check
        if (column > 0 && !roomArray[row, column - 1].visited) return true; // West check
        if (column < labColumns - 1 && !roomArray[row, column + 1].visited) return true; // East check
        return false;
    }

    private GameObject BreakWallIfExists(GameObject wall)
    {
        if (wall != null)
        {
            if (Random.Range(0, 100) < 25)
            {
                GameObject newDoor = Object.Instantiate(environmentLibrary.door, wall.transform.position, wall.transform.rotation);
                currentType = (currentType + 1) % 4;
                newDoor.name = wall.name;
                GameObject.Destroy(wall);
                return newDoor;
            }
            else
            {
                GameObject.Destroy(wall);
                return null;
            }
        }
        return null;
    }

}
