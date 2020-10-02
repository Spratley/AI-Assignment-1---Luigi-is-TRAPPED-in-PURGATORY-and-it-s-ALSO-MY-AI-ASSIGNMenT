using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TeleportToCounterpart : MonoBehaviour
{
    public GameObject counterpart; // The object we want to seamlessly teleport to

    private DoorComponent doorToReset; // This is bad and hardcoded, but in this case there is only one actual door and so we will just store the reference to it

    private void Start()
    {
        // This is done programatically cause I keep forgetting to make them triggers
        GetComponent<Collider>().isTrigger = true;

        doorToReset = FindObjectOfType<DoorComponent>(); // Again, really bad but it's okay for this
    }

    private void OnTriggerEnter(Collider other)
    {
        doorToReset.ResetDoor(); // Reset the door so the transition isn't as jarring
        Teleport(other.gameObject);
    }

    // Finds the relative position and direction of the player, then teleports them to the same relative coordinates of the counterpart object
    public void Teleport(GameObject obj)
    {
        // Position and direction of the object relative to this
        Vector3 localObjPos = transform.InverseTransformPoint(obj.transform.position);
        Vector3 localObjDir = transform.InverseTransformDirection(obj.transform.forward);

        // Transform the object so local properties are still valid
        obj.transform.position = counterpart.transform.TransformPoint(localObjPos);
        obj.transform.forward = counterpart.transform.TransformDirection(localObjDir);
    }
}
