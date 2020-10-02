using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SimpleCamera : MonoBehaviour
{
    Rigidbody rb;
    DoorComponent doorComponent;

    public float speed = 1;
    public float lookSpeed = 1;

    public float cameraAngleMin = 0;
    public float cameraAngleMax = 180;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        doorComponent = FindObjectOfType<DoorComponent>();
    }

    // This is responsible for any jitter you see, I just didn't wanna put too much time into movement
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + transform.forward * speed * Time.deltaTime * Input.GetAxis("Vertical"));
    }

    // Rotates the camera aronud the vertical as you move your mouse
    // Rotates the player around the vertical as you press A and D (cause this has pseudo tank controls lol)
    private void Update()
    {
        transform.Rotate(Vector3.up * Input.GetAxis("Horizontal") * lookSpeed);

        var eulerAngles = transform.rotation.eulerAngles;
        eulerAngles.y = Mathf.Clamp(eulerAngles.y, cameraAngleMin, cameraAngleMax);
        transform.rotation = Quaternion.Euler(eulerAngles);
    }
}
