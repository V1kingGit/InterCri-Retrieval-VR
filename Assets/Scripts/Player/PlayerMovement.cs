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

    [System.NonSerialized] public float movementX;
    [System.NonSerialized] public float movementZ;

    private Vector3 velocity;
    private bool isGrounded;

    private void Awake()
    {
        singleton = this;
    }

    void Update()
    {
        bool jumpPressed = Input.GetButtonDown("Jump");

        // Fall
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Movement
        Vector3 movement = transform.right * PlayerInputs.movement.x + transform.forward * PlayerInputs.movement.y; // Y is really Z, but with Vector2
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
