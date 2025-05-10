using System.Collections;
using UnityEngine;

public class Usable : MonoBehaviour
{

    [SerializeField] private Transform _platform;
    [SerializeField] private GameObject _useButtonSprite;
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

        if (transform.CompareTag("PickUpTNT"))
        { 
            
        }


    }
    public void triggerPlatformMovement()
    {
        FloatingPlatforms fp = _platform.GetComponent<FloatingPlatforms>();
        fp.SetMovementDir("Horizontal");
        fp.SetDuration(4f);
        fp.SetHorizontalChange(-16f);
        //Debug.Log(fp.GetArgs());

        if (leverAnimator != null)
        {
            leverAnimator.Play("ToggleRight");
        }


        StartCoroutine(fp.Movement());
        //StartCoroutine(ResetAnimation());
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
