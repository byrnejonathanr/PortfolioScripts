using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnvironmentLibrary : MonoBehaviourPun
{

    // Basic resources used for the overall lab layout.
    [Header("Basic Resources")]
    public GameObject wall;
    public GameObject door;
    public GameObject elevator;

    // Objects that can be spawned randomly within any interior that allows for them, determined by the presence of socket objects within the RoomInterior.
    [Header("Clutter & Items")]
    [SerializeField] private GameObject[] largeObstacles;
    [SerializeField] private GameObject[] smallObstacles;
    [SerializeField] private GameObject[] items;

    // How likely an object of each type is to spawn for that generation.
    [Header("Spawn Frequencies")]
    [SerializeField] private float largeObstacleFrequency = 20.0f;
    [SerializeField] private float smallObstacleFrequency = 50.0f;
    [SerializeField] private float itemFrequency = 20.0f;


    // Libraries for all the handcrafted RoomInterior objects to be selected from at random.
    [Header("Interior Layouts")]
    [SerializeField] private RoomInterior[] hallway;
    [SerializeField] private RoomInterior[] office;
    [SerializeField] private RoomInterior[] research;
    [SerializeField] private RoomInterior[] security;

    [SerializeField] private List<ItemSocket> itemSockets;
    [SerializeField] private GameObject[] essentialItems;

    // References to the Nexus interiors already present in the scene, to be assigned to two random rooms before interior generation.
    public RoomInterior nexus1;
    public RoomInterior nexus2;

    // Returns an interior 
    public RoomInterior RequestInterior(int roomType, int exitType)
    {
        RoomInterior[] selectedType = null;
        RoomInterior selectedRoom = null;
        int randomRoomInArray = 0;
        switch (roomType)
        {
            case 0:
                selectedType = hallway;
                break;
            case 1:
                selectedType = office;
                break;
            case 2:
                selectedType = research;
                break;
            case 3:
                selectedType = security;
                break;
        }
        while (selectedRoom == null)
        {
            randomRoomInArray = Random.Range(0, selectedType.Length);
            if (exitType == (int)selectedType[randomRoomInArray].exitType)
            {
                selectedRoom = selectedType[randomRoomInArray];
            }
        }
        return selectedRoom;
    }

    public void DoorSpawner(Transform spawnLocation)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject spawnedDoor = PhotonNetwork.Instantiate("Door", spawnLocation.position, spawnLocation.rotation);
        }
    }

    public void RequestLargeObstacle(Transform target)
    {
        if (Random.Range(0, 100) < largeObstacleFrequency)
        {
            GameObject spawnedObject = Instantiate(largeObstacles[Random.Range(0, largeObstacles.Length)], target.position, Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), Vector3.up));
            spawnedObject.transform.SetParent(target.parent);
        }
        //Destroy(target.gameObject);
    }

    public void RequestSmallObstacle(Transform target)
    {
        if (Random.Range(0, 100) < smallObstacleFrequency)
        {
            GameObject spawnedObject = Instantiate(smallObstacles[Random.Range(0, smallObstacles.Length)], target.position, Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), Vector3.up));
            spawnedObject.transform.SetParent(target.parent);
        }
        //Destroy(target.gameObject);
    }
    public void AddItemSocket(ItemSocket itemSocket)
    {
        itemSockets.Add(itemSocket);
        if (CheckItemSocketsLoaded())
        {
            PopulateItems();
        }
    }

    private bool CheckItemSocketsLoaded()
    {
        return FindObjectsOfType<ItemSocket>().Length == itemSockets.Count;
    }

    public void PopulateItems()
    {
        foreach (GameObject go in essentialItems)
        {

            if (itemSockets.Count <= 0)
            {
                Debug.LogError("Insufficient number of item sockets to house essential items");
                return;
            }
            int randomSocket = Random.Range(0, itemSockets.Count);
            Transform location = itemSockets[randomSocket].transform;
            if (PhotonNetwork.IsMasterClient)
            { 
                GameObject spawned = PhotonNetwork.Instantiate(go.name, location.position, Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), Vector3.up)); 
                spawned.transform.SetParent(location.parent);
                //Debug.Log(location.transform.position.ToString());
            }
            Destroy(itemSockets[randomSocket].gameObject);
            itemSockets.RemoveAt(randomSocket);
        }

        foreach (ItemSocket itemSocket in itemSockets)
        {
            RequestItem(itemSocket.transform);
        }
    }

    public void RequestItem(Transform target)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (Random.Range(0, 100) < itemFrequency)
            {
                GameObject spawned = PhotonNetwork.Instantiate(items[Random.Range(0, items.Length)].name, target.position, Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), Vector3.up)); //omits last item in RequestItem, KEYCARD NEEDS TO BE THE LAST ITEM IN THE ITEMS ARRAY
                spawned.transform.SetParent(target.parent);
            }
            //Destroy(target.gameObject);
        }
    }
}
