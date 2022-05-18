using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorWall : MonoBehaviour
{

    private EnvironmentLibrary environmentLibrary;
    public bool flaggedElevator;

    // Start is called before the first frame update
    void Start()
    {
        if (!flaggedElevator)
        {
            environmentLibrary = FindObjectOfType<EnvironmentLibrary>();
            environmentLibrary.DoorSpawner(this.transform);
        }
    }
}
