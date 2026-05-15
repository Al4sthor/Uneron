using System.Collections.Specialized;
using System.Threading;
using UnityEngine;

public class Playercamera : MonoBehaviour
{

    Transform KyleRobot;
    //camera
    public Transform cameraAxis;
    public Transform cameraTrack;

    private Transform theCamera; // ← faltaba declarar esto

    private float rotY;
    private float rotX;

    public float camRotSpeed = 200f;
    public float minAngle = -45f;
    public float maxAngle = 45f; // ← faltaba esta variable
    public float cameraSpeed = 200f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        KyleRobot = this.transform;
        theCamera = Camera.main.transform;

    }

    // Update is called once per frame
    void Update()
    {
        Cameralogic();
    }

    public void Cameralogic()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        float thetime = Time.deltaTime;

        rotX += mouseX * thetime * camRotSpeed;
        rotY += mouseY * thetime * camRotSpeed;

        KyleRobot.Rotate(0, mouseX * thetime * camRotSpeed, 0);// para que rote con la camara 

        rotY = Mathf.Clamp(rotY, minAngle, maxAngle);

        Quaternion localRotation = Quaternion.Euler(-rotY, 0, 0);
        cameraAxis.localRotation = localRotation;

        theCamera.position = Vector3.Lerp(theCamera.position, cameraTrack.position, cameraSpeed * thetime);
        theCamera.rotation = Quaternion.Lerp(theCamera.rotation, cameraTrack.rotation, cameraSpeed * thetime);

    }
}