using UnityEngine;

public class TankController : MonoBehaviour
{
    [SerializeField] float maxSpeed = 5f;
    [SerializeField] float rotationSpeed = 2f;
    [SerializeField] float slowRadius = 2f;
    [SerializeField] float hazardAvoidanceRadius = 5f;
    [SerializeField] public Transform target;
    [SerializeField] public Transform hazard;

    //SFX Sounds
    [SerializeField] AudioClip movingSound;
    [SerializeField] AudioClip hazardSound;
    [SerializeField] AudioSource soundEffectSource;

    private bool isMoving = false; 
    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            soundEffectSource.Stop();
            soundEffectSource.clip = clip;
            soundEffectSource.Play();
        }
    }

    private Vector3 velocity;
    private enum Behavior { None, Seek, Flee, Arrive, Avoid }
    private Behavior currentBehavior = Behavior.None;

    private void Update()
    {
        if (isMoving)
        {
            // Example condition: stop sound and movement when near the target
            if (Vector3.Distance(transform.position, target.position) < 0.5f)
            {
                StopMovement();
            }
        }
        // Continuously update based on the current behavior
        switch (currentBehavior)
        {
            case Behavior.Seek:
                Seek();
                FaceTarget(1);
                break;
            case Behavior.Flee:
                Flee();
                FaceTarget(-1);
                break;
            case Behavior.Arrive:
                Arrive();
                FaceTarget(1);
                break;
            case Behavior.Avoid:
                Avoid();
                FaceTarget(1);
                // Avoidance will require a hazard object, make sure that's handled in GameManager
                break;
        }

    }

    public void StartSeek() {
        PlaySound(movingSound);
        isMoving = true;
        currentBehavior = Behavior.Seek;
    }

    public void StartFlee() {
        PlaySound(movingSound);
        isMoving = true;
        currentBehavior = Behavior.Flee; 
    }

    public void StartArrive()
    {
        PlaySound(movingSound);
        isMoving = true;
        currentBehavior = Behavior.Arrive;
    }

    public void StartAvoid() {
        PlaySound(movingSound);
        isMoving = true;
        currentBehavior = Behavior.Avoid; 
    }

    public void StopMovement()
    {
        isMoving = false;
        soundEffectSource.Stop();
    }
    private void Seek()
    {
        if (target == null) return;
        Vector3 direction = (target.position - transform.position).normalized;
        velocity = direction * maxSpeed;
        transform.position += velocity * Time.deltaTime;

    }

    private void Flee()
    {
        if (target == null) return;
        Vector3 direction = (transform.position - target.position).normalized;
        velocity = direction * maxSpeed;
        transform.position += velocity * Time.deltaTime;
        // Stop at screen edges
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, -8f, 8f),
            Mathf.Clamp(transform.position.y, -4.5f, 4.5f),
            transform.position.z
        );
    }

    private void Arrive()
    {
        if (target == null) return;
        Vector3 direction = target.position - transform.position;
        float distance = direction.magnitude;

        float targetSpeed = (distance < slowRadius) ? maxSpeed * (distance / slowRadius) : maxSpeed;
        velocity = direction.normalized * targetSpeed;
        transform.position += velocity * Time.deltaTime;
    }

    private void Avoid()
    {
        if (target == null || hazard == null) return;
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        // Check the direction and distance to the hazard
        Vector3 directionToHazard = hazard.position - transform.position;
        float distanceToHazard = directionToHazard.magnitude;
        // Avoid the hazard if it's within a certain range
        Vector3 avoidanceForce = Vector3.zero;
        if (distanceToHazard < hazardAvoidanceRadius)
        {
            //PlaySound(hazardSound);
            // Calculate an avoidance direction
            avoidanceForce = Vector3.Cross(directionToHazard, Vector3.forward).normalized;
            avoidanceForce *= Vector3.Dot(avoidanceForce, directionToTarget) < 0 ? -1 : 1;
            // Scale the avoidance force based on proximity (closer = stronger avoidance)
            avoidanceForce *= (hazardAvoidanceRadius - distanceToHazard) / hazardAvoidanceRadius;
        }
        // Combine the avoidance force with the target seeking direction
        Vector3 combinedForce = directionToTarget + avoidanceForce;

        // Normalize the combined force and apply velocity
        velocity = combinedForce.normalized * maxSpeed;
        transform.position += velocity * Time.deltaTime;

        if (velocity.magnitude > 0.01f)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg + 90f;
            float smoothedAngle = Mathf.LerpAngle(transform.eulerAngles.z, angle, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 0, smoothedAngle);
        }
    }


    private void FaceTarget(float facing)
    {
        if (target == null) return;
        Vector3 lookDirection = target.position - transform.position;
        float targetAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg + (90f*facing);
        float smoothedAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, smoothedAngle);
    }


    public void ResetPosition()
    {
        velocity = Vector3.zero;
        transform.position = Vector3.zero;
        target = null;
        currentBehavior = Behavior.None;
    }
}
