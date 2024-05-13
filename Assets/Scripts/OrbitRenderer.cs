using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class OrbitRenderer : MonoBehaviour
{
    private static readonly float TwoPi = 2 * Mathf.PI;
    [SerializeField] private GravitationalBody orbitingBody;
    [SerializeField] private GravitationalBody centreBody;
    private LineRenderer lineRenderer;
    private int numSegments = 100; // number of segments to use for drawing the ellipse
    public void Awake()
    {
        this.lineRenderer = GetComponent<LineRenderer>();
    }
    public void FixedUpdate()
    {
        this.Draw();
    }
    /**
    *   draw the orbital ellipse around the centre object for the orbiting object    
    */
    private void Draw() {
        OrbitSnapshot orbit = new OrbitSnapshot(orbitingBody, centreBody);
        if(!orbit.IsOrbitClosed()) {
            this.lineRenderer.positionCount = 0;
            lineRenderer.SetPositions(new Vector3[0]);
            return;
        } else {
            this.lineRenderer.positionCount = this.numSegments + 1;
        }
        float semiMajorAxis = orbit.GetSemiMajorAxis();
        float semiMinorAxis = orbit.GetSemiMinorAxis();
        float angleToPeriapsis = orbit.GetAnglePeriapsis();
        float cosAngleToPeriapsis = Mathf.Cos(angleToPeriapsis);
        float sinAngleToPeriapsis = Mathf.Sin(angleToPeriapsis);
        Vector2 ellipsePosition = orbit.GetEllipsePosition();
        Vector3[] positions = new Vector3[this.numSegments + 1];
        for (int i = 0; i <= this.numSegments; i++)
        {
            float angle = angleToPeriapsis + (OrbitRenderer.TwoPi * i) / this.numSegments;
            // calculate x,y location before rotation and presume centre 0,0
            float x = semiMajorAxis * Mathf.Cos(angle);
            float y = semiMinorAxis * Mathf.Sin(angle);
            // rotate the ellipse position to match 
            float xRotated = (x * cosAngleToPeriapsis) - (y * sinAngleToPeriapsis);
            float yRotated = (x * sinAngleToPeriapsis) + (y * cosAngleToPeriapsis);
            // move the point to be where the ellipse is stationed
            positions[i] = new Vector3(xRotated + ellipsePosition.x, yRotated + ellipsePosition.y, 0);
        }
        lineRenderer.SetPositions(positions);
    }
}