using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class GravitationalBody : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    [SerializeField] private GravitationalBodyManager gravManager;
    public void Awake()
    {
        this.rigidBody = this.GetComponent<Rigidbody2D>();
    }
    public void Start()
    {
        this.gravManager.AddGravitationalBody(this);
    }
    public void OnDestroy()
    {
        this.gravManager.RemoveGravitationalBody(this);
    }
    public Rigidbody2D GetRigidbody()
    {
        return this.rigidBody;
    }
}