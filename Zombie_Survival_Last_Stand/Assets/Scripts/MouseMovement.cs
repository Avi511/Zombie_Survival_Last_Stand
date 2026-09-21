using UnityEngine;
using UnityEngine.EventSystems;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensitivity = 500f; //To Controls how fast the camera looks around.
    float xRotation = 0f;
    float yRotation = 0f;
    public float topClamp = -90f;
    public float bottomClamp = 90f;

    void Start()
    {
        //Locking cursor to the middle of the screen & making it invisible
        Cursor.lockState = CursorLockMode.Locked;   
    }

    void Update()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }


        //Getting Inputs from mouse and stores it in a variable
        float mouseX = Input.GetAxis("Mouse X")*mouseSensitivity*Time.deltaTime;    //Edit->Project Settings->Input Manager->Axes(Mouse X,Mouse Y, ...) 
        float mouseY = Input.GetAxis("Mouse Y")*mouseSensitivity*Time.deltaTime;


        //Rotation around x axis(Look Up & Down)---This means spinning around an imaginary line (x axis), not moving along it.
        xRotation = xRotation - mouseY; //When mouse down the body to rotate up.
        xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);   //Clamp the rotation


        //Rotation around y axis(Look Right & Left)
        yRotation = yRotation + mouseX;//When mouse right the body to rotate right.


        //Apply rotation to the transform
        transform.localRotation = Quaternion.Euler(xRotation,yRotation,0f); 
    }

}