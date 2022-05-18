using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Room : MonoBehaviourPun
{
    /* The Lab is generated as a 2D grid of these Room Objects.
     * The Room itself holds several variables that are used during the initial maze generation,
     * as well as for determining what kind of room interior it should spawn. 
     */

    [HideInInspector] public bool visited = false; // Becomes true when the HunterKiller visits this room during its run. Once all rooms have been visited, the HunterKiller has finished its task.
    [HideInInspector] public GameObject northWall, southWall, eastWall, westWall; // Stores the wall objects that are tied to the room. This allows for easy access by the HunterKiller when it needs to destroy/replace walls. Most only have an East and South wall for optimization purposes.
    public int numberOfEntrances; // Count for the number of entrances. Set before determining the layout of the exits to optimize the procedure.
    public bool northOpen, southOpen, eastOpen, westOpen; // Set to true when the corresponding direction is fully open or is a door.
    [HideInInspector] public EnvironmentLibrary environmentLibrary = null; // A link to the EnvironmentLibrary. Called upon when the interior type has been determined.
    public int roomType = 0; // The general flavor of the room. Changes whenever a door is spawned by the HunterKiller. 0 = Hallway, 1 = Offices, 2 = Research, 3 = Security. (4 = Nexus if overridden)

    // Start is called before the first frame update
    void Start()
    {
        InitializeRoom(); // This process takes place after the Lab and HunterKiller have completed the initial layout generation.
    }

    public void InitializeRoom()
    {
        // Room has been made into a nexus and interior is already assigned. Initialization can be skipped.
        if (roomType == 4)
        {
            return;
        }
        CountEntrances(); // Count the number of entrances.
        int rotationMultiple = 0; // Values 0 - 3, number of 90 degree rotations necessary with "East" being zero.
        int exitType = 0; // 0 = End Rooms, 1 = L Rooms, 2 = I Rooms, 3 = T Rooms, 4 = X Rooms.

        // rotationMultiple and exitType are determined by the number of entrances, ensures that only relevant room configurations are considered.
        switch (numberOfEntrances)
        {
            case 1: // End rooms
                // East end room R0
                if (southOpen) // South end room R1
                {
                    rotationMultiple = 1;
                }
                else if (westOpen) // West end room R2
                {
                    rotationMultiple = 2;
                }
                else if (northOpen) // North end room R3
                {
                    rotationMultiple = 3;
                }
                break;
            case 2: // L and I rooms
                if (eastOpen)
                {
                    if (southOpen) // East-South L room R0
                    {
                        exitType = 1;
                    }
                    else if (westOpen) // East-West I room R0
                    {
                        exitType = 2;
                    }
                    else // North-East L room R3
                    {
                        rotationMultiple = 3;
                        exitType = 1;
                    }
                }
                else if (southOpen)
                {
                    if (westOpen) // South-West L room R1
                    {
                        rotationMultiple = 1;
                        exitType = 1;
                    }
                    else // North-South I room R1
                    {
                        rotationMultiple = 1;
                        exitType = 2;
                    }
                }
                else // West-North L room R2
                {
                    rotationMultiple = 2;
                    exitType = 1;
                }
                break;
            case 3: // T rooms
                // East-South-West T room R0
                if (!eastOpen) // South-West-North T room R1
                {
                    rotationMultiple = 1;
                }
                else if (!southOpen) // West-North-East T room R2
                {
                    rotationMultiple = 2;
                }
                else if (!westOpen) // North-East-South T room R3
                {
                    rotationMultiple = 3;
                }
                exitType = 3;
                break;
            case 4: // X rooms
                // X room R0-3
                rotationMultiple = Random.Range(0, 4); // X Rooms can face any direction since orientation doesn't matter.
                exitType = 4;
                break;
        }

        // Request a RoomInterior from the environmentLibrary based on the roomType and exitType, rotated about the Y axis by rotationMultiple * 90.
        RoomInterior spawnedRoom = Instantiate(environmentLibrary.RequestInterior(roomType, exitType), this.transform.position, Quaternion.AngleAxis(90.0f * rotationMultiple, Vector3.up));
        spawnedRoom.transform.SetParent(this.transform);

        // For each largeObstacleSocket, requestLargeObstacle from the environmentLibrary.
        foreach (Transform largeObstacleSocket in spawnedRoom.largeObstacleSockets)
        {
            environmentLibrary.RequestLargeObstacle(largeObstacleSocket);
        }

        // For each smallObstacleSocket, requestSmallObstacle from the environmentLibrary.
        foreach (Transform smallObstacleSocket in spawnedRoom.smallObstacleSockets)
        {
            environmentLibrary.RequestSmallObstacle(smallObstacleSocket);
        }

        // For each itemSocket, requestItem from the environmentLibrary.
        foreach (ItemSocket itemSocket in spawnedRoom.itemSockets)
        {
            itemSocket.environmentLibrary = this.environmentLibrary;
        }
    }

    // Counts how many of the open booleans are true and sets numberOfEntrances to that value.
    private void CountEntrances()
    {
        int count = 0;
        bool[] cardinalOpens = { northOpen, southOpen, eastOpen, westOpen };
        foreach (bool isOpen in cardinalOpens)
        {
            if (isOpen) count++;
        }
        numberOfEntrances = count;
    }
}
