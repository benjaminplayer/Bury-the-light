using UnityEngine;

public class TouchingDirections : MonoBehaviour
{
    public ContactFilter2D castFilter;
    public float groundDistance = 0.05f;
    public float wallDistance = 0.2f;
    public float ceilDistance = 0.05f;
    CapsuleCollider2D touchingCol;
    Animator animator;

    RaycastHit2D[] groundHits = new RaycastHit2D[5];
    RaycastHit2D[] wallHits = new RaycastHit2D[5];
    RaycastHit2D[] ceilHits = new RaycastHit2D[5];

    [SerializeField]
    private bool _isGrounded;

    public bool IsGrounded { get { return _isGrounded; } private set 
        {
            _isGrounded = value;
            animator.SetBool("isGrounded", value);
        } 
    }

    [SerializeField]
    private bool _isOnWall;

    public bool IsOnWall
    {
        get { return _isOnWall; }
        private set
        {
            _isOnWall = value;
            animator.SetBool("isOnWall", value);
        }
    }

    [SerializeField]
    private bool _isOnCeil;

    private Vector2 wallCheckDir => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    public bool IsOnCeil
    {
        get { return _isOnCeil; }
        private set
        {
            _isOnCeil = value;
            animator.SetBool("isOnCeil", value);
        }
    }

    private void Awake()
    {
        touchingCol = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        IsGrounded = touchingCol.Cast(Vector2.down, castFilter, groundHits, groundDistance) > 0;
        IsOnWall = touchingCol.Cast(wallCheckDir, castFilter, wallHits, wallDistance) > 0;
        IsOnCeil = touchingCol.Cast(Vector2.up, castFilter, ceilHits, ceilDistance) > 0;
    }

}
