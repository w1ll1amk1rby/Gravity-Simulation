using System.Collections.Generic;
using UnityEngine;
public class GravitationalBodyManager : MonoBehaviour
{
    private List<GravitationalBody> bodies = new List<GravitationalBody>();
    [SerializeField] private GravitationalBody star;
    public void FixedUpdate()
    {
        this.HandleGravityTick();
    }
    public void AddGravitationalBody(GravitationalBody body)
    {
        if (!this.bodies.Contains(body))
        {
            this.bodies.Add(body);
            if (this.star != null && body != this.star)
            {
                this.OrbitGravitationalObject(this.star, body);
            }
        }
    }
    public void RemoveGravitationalBody(GravitationalBody body)
    {
        if (this.bodies.Contains(body))
        {
            this.bodies.Remove(body);
        }
    }
    /**
        apply gravity between all orbital objects for a tick
    */
    private void HandleGravityTick()
    {
        for (int i = 0; i < this.bodies.Count - 1; i++)
        {
            for (int j = i + 1; j < this.bodies.Count; j++)
            {
                this.ApplyGravityForceBetweenBodies(this.bodies[i], this.bodies[j]);
            }
        }
    }
    /**
        apply gravitational forces between two objects (per tick)
    */
    private void ApplyGravityForceBetweenBodies(GravitationalBody firstBody, GravitationalBody secondBody)
    {
        float distance = Vector2.Distance(firstBody.transform.position, secondBody.transform.position);
        if(distance <= 0) {
            return;
        }
        float mass1 = firstBody.GetRigidbody().mass;
        float mass2 = secondBody.GetRigidbody().mass;
        float force = (PhysicsConstants.GravitationalConstant * mass1 * mass2) / (distance * distance);
        Vector2 forceVector = (secondBody.transform.position - firstBody.transform.position).normalized * force;
        firstBody.GetRigidbody().AddForce(forceVector);
        secondBody.GetRigidbody().AddForce(-1 * forceVector);
    }
    /**
        places orbitBody in orbit of toOrbitBody by changing its velocity.
    */
    private void OrbitGravitationalObject(GravitationalBody orbitBody, GravitationalBody toOrbitBody)
    {
        // v = sqrt(GM/r);
        float distance = Vector2.Distance(orbitBody.transform.position, toOrbitBody.transform.position);
        float velocity = Mathf.Sqrt((PhysicsConstants.GravitationalConstant * orbitBody.GetRigidbody().mass) / distance);
        Vector2 dirToOrbitBody = (orbitBody.transform.position - toOrbitBody.transform.position).normalized;
        Vector2 velocityVector = Vector2.Perpendicular((dirToOrbitBody * velocity) + orbitBody.GetRigidbody().velocity);
        toOrbitBody.GetRigidbody().velocity = velocityVector;
    }
}