using System.Collections;
using UnityEngine;

public class Usable : MonoBehaviour
{

    [SerializeField] private Transform _platform;
    [SerializeField] private GameObject _useButtonSprite;
    [SerializeField] private float _platformMoveDuration;
    [SerializeField] private float _PlatformHorizontalChange;
    [SerializeField] private float _PlatformVerticalChange;
    [SerializeField] private GameObject _rocksParrent;
    [SerializeField] private GameObject _TNTUI;
    [SerializeField] private AudioClip explosionIgnition;
    [SerializeField] private AudioClip explosion;
    private Transform[] _rocks;
    private Animator leverAnimator;
    private void Awake()
    {
        _useButtonSprite.SetActive(false);
        if (transform.CompareTag("Lever"))
        {
            leverAnimator = GetComponent<Animator>();

            if (leverAnimator != null)
            {
                leverAnimator.Play("Idle");
            }
        }


    }

    private void Start()
    {
        if (_rocksParrent != null) 
        { 
            _rocks = _rocksParrent.GetComponentsInChildren<Transform>();
        }
    }

    public void TriggerPlatformMovement()
    {
        FloatingPlatforms fp = _platform.GetComponent<FloatingPlatforms>();
        if (_PlatformHorizontalChange != 0)
        {
            fp.SetMovementDir("Horizontal");
            fp.SetDuration(_platformMoveDuration); //4
            fp.SetHorizontalChange(_PlatformHorizontalChange); //-16
        }
        else if (_PlatformVerticalChange != 0)
        {
            Debug.Log("SetVerticalChange");
            fp.SetMovementDir("vertical");
            fp.SetDuration(_platformMoveDuration); //4
            fp.SetHorizontalChange(_PlatformVerticalChange); //-16
        }
        //Debug.Log(fp.GetArgs());

        if (leverAnimator != null)
        {
            leverAnimator.Play("ToggleRight");
        }


        StartCoroutine(fp.Movement());
        //StartCoroutine(ResetAnimation());
    }

    public void TriggerTNTPlace()
    {
        StartCoroutine(HandleTNT());
    }

    private IEnumerator HandleTNT()
    {
        SFXManager.Instance.PlaySFXClip(explosionIgnition,transform,.8f);
        yield return new WaitForSeconds(explosionIgnition.length);
        SFXManager.Instance.PlaySFXClip(explosion, transform, .8f);

        for (int i = 1; i < _rocks.Length; i++)
        {
            _rocks[i].gameObject.layer = 8; // nastavi layer da collision ne affecta playerja
            var rb = _rocks[i].GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.AddForce(GetRandomVector3() * 2, ForceMode2D.Impulse);
        }

    }

    public void PickUpTNT(CharacterController cc)
    {
        cc.hasDynamite = true;
        _TNTUI.SetActive(true);
        gameObject.SetActive(false);
    }

    private Vector3 GetRandomVector3(float min = -10f, float max = 10f)
    {
        float x = Random.Range(min, max);
        float y = Random.Range(min, max);

        return new Vector3(x, y);
    }

    //Togle use btn visiblity
    private void OnTriggerEnter2D(Collider2D collision)
    {
        _useButtonSprite.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _useButtonSprite.SetActive(false);
    }

    private IEnumerator ResetAnimation() 
    {
        yield return new WaitForSeconds(_platform.GetComponent<FloatingPlatforms>().GetDuration());

        if (leverAnimator != null)
        {
            leverAnimator.Play("ToggleLeft");
        }
    }

}
