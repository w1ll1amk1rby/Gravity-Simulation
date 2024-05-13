using UnityEngine;
/** 
    this class creates a snapshot of an objects orbit around another object.
    presumed the centre object is not acted apon and no other objects act on the orbitting object.
    ToDo currently doesn't handle objects that are on an escape velocity, data that is returned in this case will be incorrect such as the the periapsis being 0.
*/
public class OrbitSnapshot
{
    private float gravPotenEnergy;
    private float kineticEnergy;
    private float totalEnergy;
    private float angularMomentum;
    private float eccentricity;
    private bool closedOrbit;
    private float semiMajorAxis;
    private float semiMinorAxis;
    private float periapsisDistance;
    private float apoapsisDistance;
    private float anglePeriapsis;
    private float angleOrbit;
    private Vector2 periapsisPosition;
    private Vector2 apoapsisPosition;
    private Vector2 ellipsePosition;

    public OrbitSnapshot(GravitationalBody orbitBody, GravitationalBody centreBody)
    {
        Rigidbody2D centreRigid = centreBody.GetRigidbody();
        Rigidbody2D orbitRigid = orbitBody.GetRigidbody();
        float centreMass = centreRigid.mass;
        float orbitMass = orbitRigid.mass;
        Vector2 orbitPosition = orbitBody.transform.position;
        Vector2 centrePosition = centreBody.transform.position;
        Vector2 relativePosition = orbitPosition - centrePosition;
        float distance = Vector2.Distance(orbitPosition, centrePosition);
        Vector2 velocityVector = orbitRigid.velocity - centreRigid.velocity;
        this.CalculateOrbitDetails(orbitMass, centreMass, distance, relativePosition, velocityVector);
        this.CalculateEllipseDetails(orbitMass, centreMass, distance, relativePosition, centrePosition, velocityVector);
    }
    public Vector2 GetEllipsePosition()
    {
        return this.ellipsePosition;
    }
    public float GetSemiMajorAxis()
    {
        return this.semiMajorAxis;
    }
    public float GetSemiMinorAxis()
    {
        return this.semiMinorAxis;
    }
    public float GetEccentricity()
    {
        return this.eccentricity;
    }
    public float GetAnglePeriapsis()
    {
        return this.anglePeriapsis;
    }
    public float GetAngleOrbit()
    {
        return this.angleOrbit;
    }
    public bool IsOrbitClosed()
    {
        return this.closedOrbit;
    }
    /**
    *   calculate the details of the orbit using the starting positions and velcocities 
    */
    private void CalculateOrbitDetails(float orbitMass, float centreMass, float distance, Vector2 relativePosition, Vector2 velocityVector)
    {
        float velocity = velocityVector.magnitude;
        this.kineticEnergy = 0.5f * orbitMass * Mathf.Pow(velocity, 2);
        this.gravPotenEnergy = PhysicsConstants.GravitationalConstant * ((centreMass * orbitMass) / distance);
        this.totalEnergy = this.kineticEnergy - this.gravPotenEnergy;
        this.angularMomentum = orbitMass * velocity * distance * Mathf.Sin(this.CalculateVelocityPositionAngle(relativePosition, velocityVector));
        this.eccentricity = this.CalculateEccentricity(centreMass, orbitMass);
        this.closedOrbit = this.totalEnergy <= 0;
    }
    /**
    *    calculates the angle between the velocityVector and positionVector.
    */
    private float CalculateVelocityPositionAngle(Vector2 relativePosition, Vector2 velocityVector)
    {
        float radialVelocity = Vector2.Dot(velocityVector, relativePosition);
        float crossProduct = (velocityVector.x * relativePosition.y) - (velocityVector.y * relativePosition.x);
        return Mathf.Atan2(crossProduct, radialVelocity);
    }
    /**
    *   calculates the eccentricity of the orbit
    */
    private float CalculateEccentricity(float centreMass, float orbitMass)
    {
        float eccentricityRatio = (
            2 *
            Mathf.Pow(this.angularMomentum, 2) *
            this.totalEnergy
        ) / (
            Mathf.Pow(PhysicsConstants.GravitationalConstant, 2) *
            Mathf.Pow(orbitMass, 3) *
            Mathf.Pow(centreMass, 2)
        );
        if (eccentricityRatio < -1)
        {
            eccentricityRatio = -1;
        }
        return Mathf.Sqrt(1 + eccentricityRatio);
    }
    private void CalculateEllipseDetails(float orbitMass, float centreMass, float distance, Vector2 relativePosition, Vector2 centrePosition, Vector2 velocityVector)
    {
        if (this.closedOrbit)
        {
            this.semiMajorAxis = -(PhysicsConstants.GravitationalConstant * centreMass * orbitMass) / (2 * this.totalEnergy);
            this.semiMinorAxis = this.semiMajorAxis * Mathf.Sqrt(1 - Mathf.Pow(this.eccentricity, 2));
        }
        else
        {
            this.semiMajorAxis = 0;
            this.semiMinorAxis = 0;
        }
        this.periapsisDistance = this.semiMajorAxis * (1 - this.eccentricity);
        this.apoapsisDistance = this.semiMajorAxis * (1 + this.eccentricity);
        this.angleOrbit = Mathf.Atan2(relativePosition.y, relativePosition.x);
        this.anglePeriapsis = this.angleOrbit + this.CalculateAngleToPeriapsis(centreMass, orbitMass, distance, velocityVector, relativePosition);
        Vector2 periapsisDirection = new Vector2(Mathf.Cos(this.anglePeriapsis), Mathf.Sin(this.anglePeriapsis)).normalized;
        this.periapsisPosition = periapsisDirection * this.periapsisDistance + centrePosition;
        this.apoapsisPosition = -1 * periapsisDirection * this.apoapsisDistance + centrePosition;
        this.ellipsePosition = (this.periapsisPosition + this.apoapsisPosition) / 2;
    }
    private float CalculateAngleToPeriapsis(float centreMass, float orbitMass, float distance, Vector2 velocityVector, Vector2 relativePosition)
    {
        float angularMomentumSquared = Mathf.Pow(this.angularMomentum, 2);
        float gravitationalForceDenominator = PhysicsConstants.GravitationalConstant * Mathf.Pow(orbitMass, 2) * centreMass * distance;
        float cosine = ((angularMomentumSquared / gravitationalForceDenominator) - 1f) / this.eccentricity;
        cosine = Mathf.Clamp(cosine, -1f, 1f);
        float angleToPeriapsis = Mathf.Acos(cosine);
        float relativePositionDotVelocity = Vector2.Dot(relativePosition, velocityVector);
        if (relativePositionDotVelocity < 0f)
        {
            angleToPeriapsis = (2f * Mathf.PI) - angleToPeriapsis;
        }
        return angleToPeriapsis;
    }
}