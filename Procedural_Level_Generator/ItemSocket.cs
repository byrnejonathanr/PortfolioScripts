using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSocket : MonoBehaviour
{

    [HideInInspector] public EnvironmentLibrary environmentLibrary = null;
    // Start is called before the first frame update
    void Start()
    {
        if (environmentLibrary == null) environmentLibrary = FindObjectOfType<EnvironmentLibrary>();
        //GameObject item = Instantiate(environmentLibrary.RequestItem(), this.transform.position, Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), Vector3.up));
        //item.transform.SetParent(this.transform);
        environmentLibrary.AddItemSocket(this);
    }
}
