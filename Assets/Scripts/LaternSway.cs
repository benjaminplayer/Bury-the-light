using UnityEngine;

public class LaternSway : MonoBehaviour
{
    public float swayAngle = 15f;        // Max swing angle
    public float swaySpeed = 5f;         // Speed of rotation
    private Quaternion originalRotation;
    private Quaternion targetRotation;
    private bool shouldSway = false;

    void Start()
    {
        originalRotation = transform.rotation;
        targetRotation = originalRotation;
    }

    void Update()
    {
        // Smoothly rotate toward the target
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * swaySpeed);

        // If not swaying anymore and close enough to original rotation, snap back
        if (!shouldSway && Quaternion.Angle(transform.rotation, originalRotation) < 0.1f)
        {
            transform.rotation = originalRotation;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            float direction = (other.transform.position.x < transform.position.x) ? 1f : -1f;
            targetRotation = Quaternion.Euler(0f, 0f, direction * swayAngle);
            shouldSway = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            targetRotation = originalRotation;
            shouldSway = false;
        }
    }
}
