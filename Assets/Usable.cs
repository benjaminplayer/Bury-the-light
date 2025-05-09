using UnityEngine;

public class Usable : MonoBehaviour
{

    [SerializeField] private Transform _platform;
    [SerializeField] private float _moveFor;
    [SerializeField] private float _duration;

    public void triggerPlatformMovement()
    {
        FloatingPlatforms fp = _platform.GetComponent<FloatingPlatforms>();
        fp.SetMovementDir("Horizontal");
        StartCoroutine(fp.Movement(_platform, _platform.position + new Vector3(0, _moveFor), _duration));
    }

}
