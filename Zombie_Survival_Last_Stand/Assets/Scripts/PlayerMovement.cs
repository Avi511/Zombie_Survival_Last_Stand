using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController;

    public float speed = 12f;
    public float runMultiplier = 1.5f;
    public float gravity = -9.81f*2;    //Simply makes the player fall faster(Gravity always pulls downward)
    public float jumpHeight = 3f;       //How high the player jumps


    bool isGrounded;
    bool isMoving;
    public Transform groundCheck;   //Assign a new gameObject on bottom of player
    public float groundDistance = 0.4f; //Radius of the checking sphere (Sphere is between the groundCheck & groundMask)
    public LayerMask groundMask;    //Assign the Floor/Ground layer to it


    Vector3 velocity;
    private Vector3 lastPosition = new Vector3(0f,0f,0f);   //Stores where the player was in the previous frame


    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }


    void Update()
    {
        //Checking the Player is Grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); //It returns the bool value


        //Resetting the default velocity (Up & Down) Bcz suppose you're falling,Velocity might become -20 like that.
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;   //When you touch the ground,gravity still wants to push downward. If we leave it like that,the player keeps trying to go underground.
                                //So we reset it. (-2) instead of 0 -> Because Character Controller works more reliably with a small downward value.
        }




        //Getting the inputs
        float x = Input.GetAxis("Horizontal");  // A = -1/D = +1
        float z = Input.GetAxis("Vertical");    // W = +1/S = -1


        
        // Get only the horizontal forward direction.These are direction vectors, not positions...They always represent the direction your GameObject is facing
        //Unity calculates forward, right, up from the Rotation. So you change Rotation, and Unity automatically changes forward, right, and up
        //Forward (North / +Z) => (0, 0, 1)             //Right (East / +X) => (1, 0, 0)           //45° Left (Northwest) => (-0.707, 0, 0.707)      
        //transform.forward → the direction the player is looking
        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        // Get only the horizontal right direction
        //Forward (North / +Z) => (1, 0, 0)             //Right (East / +X) => (0, 0, -1)           //45° Left (Northwest) => (0.707, 0, 0.707)
        //transform.right → the direction to the player's right side.  Eg :When looking 45° Left (Northwest) => transform.forward = (-0.707(-x), 0, 0.707(+z)) , So for this same looking face -> transform.right = (0.707(+x),0,0.707(+z)) Bcz it represents right side direction from that looking face.
        Vector3 right = transform.right;
        right.y = 0f;
        right.Normalize();


        //Check if Shift is being held while moving forward
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && z > 0;
        //Set current movement speed
        float currentSpeed = speed;

        if (isRunning)
        {
            currentSpeed = speed * runMultiplier;
        }

        //Creating the moving Vector
        Vector3 move = right * x + forward * z;   //Eg: (W+A) pressed => z=1,x=-1 So, move = right * -1+ forward * 1    // So move forward&left at same time
        //Actually moving the player
        characterController.Move(move * currentSpeed * Time.deltaTime);



        //Check if the player can jump
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            //Going up //Now only stored that speed in the velocity variable.
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); //positive, because jumping starts by moving upward.
        }
        //Falling down
        velocity.y = velocity.y + gravity * Time.deltaTime;
        //Executing the Jump -> Now Unity uses the current value of velocity.y to actually move the player upward.
        characterController.Move(velocity * Time.deltaTime);



        if(lastPosition != gameObject.transform.position && isGrounded == true)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        lastPosition = gameObject.transform.position;

    }

}