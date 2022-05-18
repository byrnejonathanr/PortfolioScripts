using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Lab : MonoBehaviourPun
{
    public int seed = 0;
    public int labRows, labColumns;
    private float size = 20.0f;
    [SerializeField] private Room room = null;
    [SerializeField] private EnvironmentLibrary environmentLibrary = null;
    [SerializeField] private GameObject gasEffect = null;
    private Room[,] roomArray;

    // Start is called before the first frame update
    void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            seed = Random.Range(int.MinValue, int.MaxValue);
            this.photonView.RPC("SetSeed", RpcTarget.All, seed);
        }
    }

    private void InitializeLab()
    {
        roomArray = new Room[labRows, labColumns];

        for (int r = 0; r < labRows; r++)
        {
            for (int c = 0; c < labColumns; c++)
            {
                roomArray[r, c] = Instantiate(room, new Vector3(r * size, 0, c * size), Quaternion.identity);
                roomArray[r, c].transform.SetParent(this.transform);
                roomArray[r, c].name = "Room " + r + "," + c;
                roomArray[r, c].environmentLibrary = this.environmentLibrary;

            }
        }

        for (int r = 0; r < labRows; r++)
        {
            for (int c = 0; c < labColumns; c++)
            {
                // West walls
                if (c == 0)
                {
                    roomArray[r, c].westWall = Instantiate(environmentLibrary.wall, new Vector3(r * size, 0f, (c * size) - (size/2.0f)), Quaternion.identity);
                    roomArray[r, c].westWall.name = "West Wall " + r + "," + c;
                    roomArray[r, c].westWall.transform.SetParent(roomArray[r, c].transform);
                }

                // East walls
                if (c < labColumns - 1 && Random.Range(0, 100) < 20)
                {
                    roomArray[r, c].eastWall = Instantiate(environmentLibrary.door, new Vector3(r * size, 0f, (c * size) + (size / 2.0f)), Quaternion.identity);
                    roomArray[r, c].eastOpen = true;
                    roomArray[r, c + 1].westOpen = true;
                    roomArray[r, c].eastWall.name = "East Wall " + r + "," + c;
                    roomArray[r, c].eastWall.transform.SetParent(roomArray[r, c].transform);
                }
                else
                {
                    roomArray[r, c].eastWall = Instantiate(environmentLibrary.wall, new Vector3(r * size, 0f, (c * size) + (size / 2.0f)), Quaternion.identity);
                    roomArray[r, c].eastWall.name = "East Wall " + r + "," + c;
                    roomArray[r, c].eastWall.transform.SetParent(roomArray[r, c].transform);
                }

                // North walls
                if (r == 0)
                {
                    roomArray[r, c].northWall = Instantiate(environmentLibrary.wall, new Vector3((r * size) - (size / 2.0f), 0f, c * size), Quaternion.AngleAxis(90.0f, Vector3.up));
                    roomArray[r, c].northWall.name = "North Wall " + r + "," + c;
                    roomArray[r, c].northWall.transform.SetParent(roomArray[r, c].transform);
                }

                // South walls
                if (r < labRows - 1 && Random.Range(0, 100) < 25)
                {
                    roomArray[r, c].southWall = Instantiate(environmentLibrary.door, new Vector3((r * size) + (size / 2.0f), 0f, c * size), Quaternion.AngleAxis(90.0f, Vector3.up));
                    roomArray[r, c].southOpen = true;
                    roomArray[r + 1, c].northOpen = true;
                    roomArray[r, c].southWall.name = "South Wall " + r + "," + c;
                    roomArray[r, c].southWall.transform.SetParent(roomArray[r, c].transform);
                }
                else
                {
                    roomArray[r, c].southWall = Instantiate(environmentLibrary.wall, new Vector3((r * size) + (size / 2.0f), 0f, c * size), Quaternion.AngleAxis(90.0f, Vector3.up));
                    roomArray[r, c].southWall.name = "South Wall " + r + "," + c;
                    roomArray[r, c].southWall.transform.SetParent(roomArray[r, c].transform);
                }
            }
        }

        PlayerSpawner playerSpawner = FindObjectOfType<PlayerSpawner>();
        // North Elevator Spawn
        int randomNumber = Random.Range(1, labColumns - 1);
        GameObject doorInstance = Instantiate(environmentLibrary.door, roomArray[0, randomNumber].northWall.transform.position, Quaternion.AngleAxis(90.0f, Vector3.up));
        DoorWall doorWall = doorInstance.GetComponent<DoorWall>();
        doorWall.flaggedElevator = true;
        doorInstance.name = roomArray[0, randomNumber].northWall.name;
        doorInstance.transform.SetParent(roomArray[0, randomNumber].transform);
        Destroy(roomArray[0, randomNumber].northWall);
        roomArray[0, randomNumber].northWall = doorInstance;
        roomArray[0, randomNumber].northOpen = true;
        GameObject elevator = Instantiate(environmentLibrary.elevator, roomArray[0, randomNumber].transform.position, Quaternion.identity);
        elevator.name = "North Elevator";
        elevator.transform.SetParent(this.transform);
        playerSpawner.startingPos.Add(new Vector3(roomArray[0, randomNumber].transform.position.x - 15.0f, 0.0f, roomArray[0, randomNumber].transform.position.z));
        

        // South Elevator Spawn
        randomNumber = Random.Range(1, labColumns - 1);
        doorInstance = Instantiate(environmentLibrary.door, roomArray[labRows - 1, randomNumber].southWall.transform.position, Quaternion.AngleAxis(90.0f, Vector3.up));
        doorWall = doorInstance.GetComponent<DoorWall>();
        doorWall.flaggedElevator = true;
        doorInstance.name = roomArray[labRows - 1, randomNumber].southWall.name;
        doorInstance.transform.SetParent(roomArray[labRows - 1, randomNumber].transform);
        Destroy(roomArray[labRows - 1, randomNumber].southWall);
        roomArray[labRows - 1, randomNumber].southWall = doorInstance;
        roomArray[labRows - 1, randomNumber].southOpen = true;
        elevator = Instantiate(environmentLibrary.elevator, roomArray[labRows - 1, randomNumber].transform.position, Quaternion.AngleAxis(180.0f, Vector3.up));
        elevator.name = "South Elevator";
        elevator.transform.SetParent(this.transform);
        playerSpawner.startingPos.Add(new Vector3(roomArray[labRows - 1, randomNumber].transform.position.x + 15.0f, 0.0f, roomArray[labRows - 1, randomNumber].transform.position.z));

        // West Elevator Spawn
        randomNumber = Random.Range(labRows - 2, 2);
        doorInstance = Instantiate(environmentLibrary.door, roomArray[randomNumber, 0].westWall.transform.position, Quaternion.identity);
        doorWall = doorInstance.GetComponent<DoorWall>();
        doorWall.flaggedElevator = true;
        doorInstance.name = roomArray[randomNumber, 0].westWall.name;
        doorInstance.transform.SetParent(roomArray[randomNumber, 0].transform);
        Destroy(roomArray[randomNumber, 0].westWall);
        roomArray[randomNumber, 0].westWall = doorInstance;
        roomArray[randomNumber, 0].westOpen = true;
        elevator = Instantiate(environmentLibrary.elevator, roomArray[randomNumber, 0].transform.position, Quaternion.AngleAxis(270.0f, Vector3.up));
        elevator.name = "West Elevator";
        elevator.transform.SetParent(this.transform);
        playerSpawner.startingPos.Add(new Vector3(roomArray[randomNumber, 0].transform.position.x, 0.0f, roomArray[randomNumber, 0].transform.position.z - 15.0f));

        // East Elevator Spawn
        randomNumber = Random.Range(labRows - 2, 2);
        doorInstance = Instantiate(environmentLibrary.door, roomArray[randomNumber, labColumns - 1].eastWall.transform.position, Quaternion.identity);
        doorWall = doorInstance.GetComponent<DoorWall>();
        doorWall.flaggedElevator = true;
        doorInstance.name = roomArray[randomNumber, labColumns - 1].eastWall.name;
        doorInstance.transform.SetParent(roomArray[randomNumber, labColumns - 1].transform);
        Destroy(roomArray[randomNumber, labColumns - 1].eastWall);
        roomArray[randomNumber, labColumns - 1].eastWall = doorInstance;
        roomArray[randomNumber, labColumns - 1].eastOpen = true;
        elevator = Instantiate(environmentLibrary.elevator, roomArray[randomNumber, labColumns - 1].transform.position, Quaternion.AngleAxis(90.0f, Vector3.up));
        elevator.name = "East Elevator";
        elevator.transform.SetParent(this.transform);
        playerSpawner.startingPos.Add(new Vector3(roomArray[randomNumber, labColumns - 1].transform.position.x, 0.0f, roomArray[randomNumber, labColumns - 1].transform.position.z + 15.0f));
    }

    [PunRPC]
    public void SetSeed(int remoteSeed)
    {
        this.seed = remoteSeed;
        Random.InitState(this.seed);
        InitializeLab();
        HunterKiller hk = new HunterKiller(roomArray, environmentLibrary);
        hk.HuntAndKill();

        // Establish Interior Overrides
        // West Nexus
        int overrideRow = Random.Range(1, (int)labRows / 2);
        int overrideColumn = Random.Range(1, labColumns - 1);
        roomArray[overrideRow, overrideColumn].roomType = 4;
        environmentLibrary.nexus1.transform.position = roomArray[overrideRow, overrideColumn].transform.position;
        environmentLibrary.nexus1.transform.rotation = Quaternion.AngleAxis(90.0f * Random.Range(0, 4), Vector3.up);
        // East Nexus
        overrideRow = Random.Range(((int)labRows / 2) + 1, labRows - 1);
        overrideColumn = Random.Range(1, labColumns - 1);
        roomArray[overrideRow, overrideColumn].roomType = 4;
        environmentLibrary.nexus2.transform.position = roomArray[overrideRow, overrideColumn].transform.position;
        environmentLibrary.nexus2.transform.rotation = Quaternion.AngleAxis(90.0f * Random.Range(0, 4), Vector3.up);
    }
}
