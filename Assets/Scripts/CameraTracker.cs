using UnityEngine;
public class CameraTracker : MonoBehaviour
{
    [SerializeField] private GameObject trackObject;
    // Update is called once per frame
    public void Update()
    {
        this.TrackObject();
    }
    /**
        move the camera to centre the trackObject, currently rotates camera according to 0,0  (where the star is)
        ToDo, provide object to rotate from
    */
    private void TrackObject() {
        this.transform.position = new Vector3(trackObject.transform.position.x, trackObject.transform.position.y, this.transform.position.z);
        Vector2 direction = this.transform.position.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        this.transform.eulerAngles = new Vector3(0, 0, angle);
    }
}