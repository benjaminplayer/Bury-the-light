using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Camera cam;
    public Transform followTarget;


    //Start pos za parallax objekt
    Vector2 startingPosition;

    //Z value parallax objekta
    float startingZ;

    Vector2 camMoveSinceStart => (Vector2)cam.transform.position - startingPosition;

    float zDistanceFromTarget => transform.position.z - followTarget.transform.position.z;

    float clippingPlane => (cam.transform.position.z + (zDistanceFromTarget > 0 ? cam.farClipPlane : cam.nearClipPlane)); //if parralax je infront -> far else near

    float parallaxFactor => Mathf.Abs(zDistanceFromTarget * clippingPlane); // " => " to pomeni da se izvede usak frame

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startingPosition = transform.position;
        startingZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newPositon = startingPosition + camMoveSinceStart / parallaxFactor;

        transform.position = new Vector3(newPositon.x, newPositon.y, startingZ);
    }
}
