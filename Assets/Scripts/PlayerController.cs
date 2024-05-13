using UnityEngine;
[RequireComponent(typeof(Thruster))]
public class PlayerController : MonoBehaviour
{
    private bool upPressed;
    private bool downPressed;
    private bool leftPressed;
    private bool rightPressed;
    private Thruster thruster;
    [SerializeField] private GameObject perspective;
    public void Awake()
    {
        this.upPressed = false;
        this.downPressed = false;
        this.leftPressed = false;
        this.rightPressed = false;
        this.thruster = this.GetComponent<Thruster>();
    }
    public void Update()
    {
        this.CheckInputs();
    }
    public void FixedUpdate()
    {
        this.HandleInputs();
    }
    public void CheckInputs() {
        if (Input.GetKey(KeyCode.W))
        {
            this.upPressed = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            this.downPressed = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            this.leftPressed = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            this.rightPressed = true;
        }
    }
    public void HandleInputs() {
        float radian = perspective.transform.eulerAngles.z*Mathf.Deg2Rad;
        Vector2 camDirection = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)).normalized;
        Vector2 moveDirection = Vector2.zero;
        if (this.upPressed)
        {
            moveDirection = moveDirection + Vector2.Perpendicular(camDirection);
            this.upPressed = false;
        }
        else if (this.downPressed)
        {
            moveDirection = moveDirection + -1*Vector2.Perpendicular(camDirection);
            this.downPressed = false;
        }
        if (this.leftPressed)
        {
            moveDirection = moveDirection + -1*camDirection;
            this.leftPressed = false;
        }
        else if (this.rightPressed)
        {
            moveDirection = moveDirection + camDirection;
            this.rightPressed = false;
        }
        this.thruster.Thrust(moveDirection);
        if (moveDirection.magnitude != 0)
        {
            this.thruster.Thrust(moveDirection);
        }
    }
}