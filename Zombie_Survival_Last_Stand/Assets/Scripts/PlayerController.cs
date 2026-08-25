using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;

    public float speed = 12f;
    public float gravity = -9.81f*2;    
    public float jumpHeight = 3f;       


    bool isGrounded;
    bool isMoving;
    public Transform groundCheck;   
    public float groundDistance = 0.4f; 
    public LayerMask groundMask;    


    Vector3 velocity;
    private Vector3 lastPosition = new Vector3(0f,0f,0f);   


    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }


    void Update()
    {
        
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;  
        }



        float x = Input.GetAxis("Horizontal");  
        float z = Input.GetAxis("Vertical");    

        
        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0f;
        right.Normalize();

        
        Vector3 move = right * x + forward * z;   //Eg: (W+A) pressed => z=1,x=-1 So, move = right * -1+ forward * 1    // So move forward&left at same time
        
        characterController.Move(move * speed * Time.deltaTime);


        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); 
        }
        
        velocity.y = velocity.y + gravity * Time.deltaTime;

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