using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SteeringAgent : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 3f;
    public float maxForce = 10f; // Limit how "fast" we can change direction (turning radius)
    
    [Header("Arrive")]
    public float slowingRadius = 5f;
    
    
    [Header("Cohesion")]
    public float cohesionRadius = 3f;
    
    [Header("Separation")]
    public float separationRadius = 5f;
    public float separationStrength = 5f;
    
    [Header("Weights")]
    public float arriveWeight = 1f;
    public float separationWeight = 1f;
    
    [Header("Debug")]
    public bool drawDebug = true;
    private Vector3 velocity = Vector3.zero;
    
    // Optional target for Seek / Arrive
    public Transform target;
    
    // Static list so agents can find each other
    public static List<SteeringAgent> allAgents = new
        List<SteeringAgent>();
    private void OnEnable()
    {
        allAgents.Add(this);
    }
    private void OnDisable()
    {
        allAgents.Remove(this);
    }
    
    void Update()
    {
        var vector3 = transform.position;
        vector3.y = 1;
        transform.position = vector3;
        Vector3 totalSteering = Vector3.zero;
        if (target != null)
        {
            totalSteering += Arrive(target.position, slowingRadius) * arriveWeight;
        }
        
        if (allAgents.Count > 1)
        {
            Vector3 separationForce = Separation(separationRadius, separationStrength) * separationWeight;
            totalSteering += separationForce;
            totalSteering += Cohesion(cohesionRadius);
        }
        
        totalSteering = Vector3.ClampMagnitude(totalSteering, maxForce);    
        velocity += totalSteering * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        transform.position += velocity * Time.deltaTime;
        if (velocity.sqrMagnitude > 0.0001f)
        {
            transform.forward = velocity.normalized;
        }
    }
    // -- BEHAVIOUR STUBS --
    public Vector3 Seek(Vector3 targetPosition)
    {
        Vector3 toTarget = targetPosition - transform.position;
        // If we are already there, stop steering
        if (toTarget.sqrMagnitude < 0.0001f)
            return Vector3.zero;
        // Desired Velocity: Full speed towards target
        Vector3 desired = toTarget.normalized * maxSpeed;
        // Reynolds' Steering Formula
        return desired - velocity;
    }
    
    public Vector3 Arrive(Vector3 targetPos, float slowRadius) {
        
        Vector3 toTarget = targetPos - transform.position;
        float distance = toTarget.magnitude;

        if (distance < 0.0001f)
        {
            return Vector3.zero;
        }
            
        float desiredSpeed = maxSpeed;
        // Ramp down speed if within radius
        if (distance < slowingRadius)
        {
            desiredSpeed = maxSpeed * (distance / slowRadius);
        }
        Vector3 desired = toTarget.normalized * desiredSpeed;
        return desired - velocity;
    }
    
    public Vector3 Separation(float radius, float strength) {
        Vector3 force = Vector3.zero;
        int neighborCount = 0;
        foreach (SteeringAgent agent in allAgents)
        {
            if (agent == this)
            {
                continue;
            }
            
            Vector3 toMe = transform.position - agent.transform.position;
            float dist = toMe.magnitude;
            if (dist > 0f && dist < separationRadius)
            {
                force += toMe.normalized / dist;
                neighborCount++;
            }
        }
        if (neighborCount > 0)
        {
            force /= neighborCount; // Average direction
            
            force = force.normalized * maxSpeed;
            force = force - velocity;
            force *= separationStrength;
        }

        return force;
    }

    private Vector3 Cohesion(float radius)
    {
        Vector3 force = Vector3.zero;
        int neighborCount = 0;
        foreach (SteeringAgent agent in allAgents)
        {
            if (agent == this)
            {
                continue;
            }

            Vector3 towards = agent.transform.position - transform.position;
            float dist = towards.magnitude;
            if (dist > 0f && dist < radius)
            {
                force += towards;
                neighborCount++;
            }
        }
        force /= neighborCount;
        force = force.normalized * maxSpeed;
        force = force - velocity;

        return force;
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawDebug) return;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + velocity);
    }
}