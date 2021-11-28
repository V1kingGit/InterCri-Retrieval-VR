using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement singleton;

    [SerializeField] private CharacterController charController;

    [SerializeField] private float gravity = -10f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float speed = 12f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;


    private Vector3 velocity;
    private bool isGrounded;

    // Update is called once per frame

    private void Awake()
    {
        singleton = this;
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        bool jumpPressed = Input.GetButtonDown("Jump");

        // Fall
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Movement
        Vector3 movement = transform.right * x + transform.forward * z;
        charController.Move(movement * speed * Time.deltaTime);

        // Jump
        if(jumpPressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        velocity.y += gravity * Time.deltaTime;
        charController.Move(velocity * Time.deltaTime);
    }
}
