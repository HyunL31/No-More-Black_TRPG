using Spine.Unity;
using UnityEngine;

public enum PlayerDirection
{
    Front,
    Side,
    Back
}

public class PlayerController : MonoBehaviour
{
    [Header("Spine")]
    [SerializeField] private SkeletonAnimation Skeleton_Front;
    [SerializeField] private SkeletonAnimation Skeleton_Side;
    [SerializeField] private SkeletonAnimation Skeleton_Back;

    [Header("Movement")]
    [SerializeField] private Rigidbody Rigidbody;
    [SerializeField] private LayerMask Layer_ground;
    [SerializeField] private Transform Transform_GroundCheck;

    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _runSpeed = 5f;
    [SerializeField] private float _jumpForce = 2f;

    [SerializeField] private float _groundCheckRadius = 0.1f;

    private PlayerDirection _currentDirection = PlayerDirection.Front;

    private Vector2 _moveInput;

    private bool _isGround = true;
    private bool _isJumping = false;

    private int _maxJumpCount = 2;
    private int _currentJumpCount = 0;

    private string _currentAnimation = "";
    private bool _currentLoop = true;

    private void Start()
    {
        SetDirection(_currentDirection);
        PlayAnimation("basic");
    }

    private void Update()
    {
        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _moveInput.y = Input.GetAxisRaw("Vertical");
        _moveInput = _moveInput.normalized;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        UpdateDirection();
    }

    private void FixedUpdate()
    {
        Move();
        CheckGround();
    }

    private void Move()
    {
        Vector3 movement = new Vector3(_moveInput.x, 0f, _moveInput.y);
        Vector3 velocity = Rigidbody.linearVelocity;

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? _runSpeed : _speed;

        velocity.x = movement.x * currentSpeed;
        velocity.z = movement.z * currentSpeed;

        Rigidbody.linearVelocity = velocity;

        if (!_isGround || _isJumping)
        {
            return;
        }

        if (_moveInput.sqrMagnitude > 0f)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                PlayAnimation("run");
            }
            else
            {
                PlayAnimation("walk");
            }
        }
        else
        {
            PlayAnimation("basic");
        }
    }

    private void Jump()
    {
        if (_currentJumpCount >= _maxJumpCount)
        {
            return;
        }

        _currentJumpCount++;

        _isGround = false;
        _isJumping = true;

        Vector3 velocity = Rigidbody.linearVelocity;
        velocity.y = 0f;
        Rigidbody.linearVelocity = velocity;

        Rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);

        string jumpAnimation = _currentJumpCount == 1 ? "jump" : "jump2";

        _currentAnimation = "";
        PlayAnimation(jumpAnimation, false);
    }

    private bool IsJumpAnimationFinished()
    {
        var entry = GetCurrentSkeleton().AnimationState.GetCurrent(0);
        return entry == null || entry.IsComplete;
    }

    private void CheckGround()
    {
        bool grounded = Physics.CheckSphere(Transform_GroundCheck.position, _groundCheckRadius, Layer_ground, QueryTriggerInteraction.Ignore);

        if (!_isJumping)
        {
            _isGround = grounded;
            return;
        }

        if (grounded && IsJumpAnimationFinished())
        {
            _currentJumpCount = 0;
            _isJumping = false;
            _isGround = true;
        }
    }

    private void UpdateDirection()
    {
        if (_moveInput.y > 0)
        {
            SetDirection(PlayerDirection.Back);
        }
        else if (_moveInput.y < 0)
        {
            SetDirection(PlayerDirection.Front);
        }
        else if (_moveInput.x > 0)
        {
            SetDirection(PlayerDirection.Side, true);
        }
        else if (_moveInput.x < 0)
        {
            SetDirection(PlayerDirection.Side, false);
        }
    }

    private void SetDirection(PlayerDirection direction, bool isRight = true)
    {
        bool directionChanged = _currentDirection != direction;

        _currentDirection = direction;

        Skeleton_Front.gameObject.SetActive(direction == PlayerDirection.Front);
        Skeleton_Side.gameObject.SetActive(direction == PlayerDirection.Side);
        Skeleton_Back.gameObject.SetActive(direction == PlayerDirection.Back);

        if (direction == PlayerDirection.Side)
        {
            Vector3 scale = Skeleton_Side.transform.localScale;

            scale.x = Mathf.Abs(scale.x) * (isRight ? 1f : -1f);

            Skeleton_Side.transform.localScale = scale;
        }

        if (directionChanged && !string.IsNullOrEmpty(_currentAnimation))
        {
            GetCurrentSkeleton().AnimationState.SetAnimation(0, _currentAnimation, _currentLoop);
        }
    }

    private SkeletonAnimation GetCurrentSkeleton()
    {
        switch (_currentDirection)
        {
            case PlayerDirection.Front:
                return Skeleton_Front;

            case PlayerDirection.Side:
                return Skeleton_Side;

            case PlayerDirection.Back:
                return Skeleton_Back;

            default:
                return Skeleton_Front;
        }
    }

    private void PlayAnimation(string anim, bool isLoop = true)
    {
        if (_currentAnimation == anim)
        {
            return;
        }

        _currentAnimation = anim;
        _currentLoop = isLoop;

        SkeletonAnimation skeleton = GetCurrentSkeleton();

        skeleton.AnimationState.SetAnimation(0, anim, isLoop);
    }

    private void OnDrawGizmos()
    {
        if (Transform_GroundCheck == null)
        {
            return;
        }

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(Transform_GroundCheck.position, _groundCheckRadius);
    }
}