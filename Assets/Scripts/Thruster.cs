using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
/**
    apply forces too a rigid body in a given direction
*/
public class Thruster : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    [SerializeField] private float thrusterForce;
    public void Awake()
    {
        this.rigidBody = this.GetComponent<Rigidbody2D>();
    }
    public void Thrust(Vector2 direction) {
        if (direction.magnitude != 0)
        {
            this.rigidBody.AddForce(direction.normalized*thrusterForce);
        }
    }
}
