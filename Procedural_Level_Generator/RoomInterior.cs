using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInterior : MonoBehaviour
{
    /* This Script serves as a way to store all of the sockets for clutter objects and items.
     * It also has an ID called exitType that is based on how the exits are shaped.
     * This exitType variable and the sockets must be manually assigned.
     */

    public enum ExitType:int{ END_ROOM, L_ROOM, I_ROOM, T_ROOM, X_ROOM }; // Members of this enum are named after the shape of the room itself.
    public ExitType exitType = 0; // Exposes the ExitType enum in the inspector, displays the types by name instead of as int values.
    public Transform[] largeObstacleSockets; // Larger Obstacles that generally are more complex and take up more room. Think collapsed roof, stacks of boxes, etc.
    public Transform[] smallObstacleSockets; // Smaller Obstacles that take up less room. Think standing ferns, random chairs, random boxes, etc.
    public ItemSocket[] itemSockets; // Potential spawn locations for Items. Generally should be more common in certain room types.
}
