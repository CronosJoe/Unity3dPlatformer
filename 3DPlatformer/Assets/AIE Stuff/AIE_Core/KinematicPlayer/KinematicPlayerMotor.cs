using UnityEngine;
using TMPro;

/// <summary>
/// Basic kinematic player motor demonstrating how to implement a motor for a KinematicBody
/// </summary>
public class KinematicPlayerMotor : MonoBehaviour, IKinematicMotor
{
    [Header("Body")]
    public KinematicBody body;
    public Transform dummyReference;
    public PlayerController playerRef;
    public TMP_Text timerDisplay;
    [Header("Common Movement Settings")]
    public float moveSpeed = 8.0f;
    public float jumpHeight = 2.0f;

    [Header("Ground Movement")]
    public float maxGroundAngle = 75f;
    public float groundAccel = 200.0f;
    public float groundFriction = 12.0f;
    public LayerMask groundLayers;
    public float maxGroundAdhesionDistance = 0.1f;

    public bool Grounded { get; private set; }
    private bool wasGrounded;
    private bool isDashing { get; set; }
    public bool canDash = true;

    [Header("Air Movement")]
    public float airAccel = 50.0f;
    public float airFriction = 3.0f;

    private bool jumpedThisFrame;

    // Input handling
    private Vector3 moveWish;
    private bool jumpWish;

    //timer variables
    [SerializeField]float currentTime = 0;
    [Range(0, 360)]
    [SerializeField] int bombTime = 120; // in seconds
    [Range(0, 2)]
    public float dashTime = 0.3f; //also in seconds
    float maxDashTime = 0.1f;//set on start to dashTime
    [Range(0,2)]
    public float dashDelay = 0.5f;
    float dashDelayMax = 0.0f;
    [Range(0, 20)]
    [SerializeField]int dashDistance = 5;
    Vector3 startingPosition = Vector3.zero;




    //Timer method
    private void UpdateTimer() 
    {
        if (!playerRef.pause)
        {
            currentTime += Time.deltaTime;
            if (isDashing)
            {
                dashTime -= Time.deltaTime;
                if (dashTime <= 0)
                {
                    dashTime = maxDashTime;
                    isDashing = false;
                }
            }
            else if (!canDash)
            {
                dashDelay -= Time.deltaTime;
                if (dashDelay <= 0)
                {
                    dashDelay = dashDelayMax;
                    canDash = true;
                }
            }
            if (bombTime <= currentTime)
            {
                playerRef.EndingTheGame(true); //this will automatically pull to end screen, might want animation/sound first
                timerDisplay.text = "0 seconds left";
            }
            else 
            {
                timerDisplay.text = (int)(bombTime - currentTime) + " seconds left";
            }

        }
    }



    //
    // Motor API
    //

    public void MoveInput(Vector3 move)
    {
        moveWish = move;
    }

    public void JumpInput()
    {
        jumpWish = true;
    }
    public void DashInput() 
    {
        if (canDash) 
        {
            isDashing = true;
        }
    }
    //
    // Motor Utilities
    //
    
    public Vector3 ClipVelocity(Vector3 inputVelocity, Vector3 normal)
    {
        return Vector3.ProjectOnPlane(inputVelocity, normal);
    }

    //
    // IKinematicMotor implementation
    //

    public Vector3 UpdateVelocity(Vector3 oldVelocity)
    {
        if (!isDashing)
        {
            Vector3 velocity = oldVelocity;

            //
            // integrate player forces
            //

            if (jumpWish)
            {
                jumpWish = false;

                if (wasGrounded)
                {
                    jumpedThisFrame = true;

                    velocity.y += Mathf.Sqrt(-2.0f * body.EffectiveGravity.y * jumpHeight);
                }
            }

            bool isGrounded = !jumpedThisFrame && wasGrounded;

            float effectiveAccel = (isGrounded ? groundAccel : airAccel);
            float effectiveFriction = (isGrounded ? groundFriction : airFriction);

            // apply friction
            float keepY = velocity.y;
            velocity.y = 0.0f; // don't consider vertical movement in friction calculation
            float prevSpeed = velocity.magnitude;
            if (prevSpeed != 0)
            {
                float frictionAccel = prevSpeed * effectiveFriction * Time.deltaTime;
                velocity *= Mathf.Max(prevSpeed - frictionAccel, 0) / prevSpeed;
            }

            velocity.y = keepY;

            // apply movement
            moveWish = Vector3.ClampMagnitude(moveWish, 1);
            float velocityProj = Vector3.Dot(velocity, moveWish);
            float accelMag = effectiveAccel * Time.deltaTime;

            // clamp projection onto movement vector
            if (velocityProj + accelMag > moveSpeed)
            {
                accelMag = moveSpeed - velocityProj;
            }

            return velocity + (moveWish * accelMag);
        }
        else
        {
            //direction will be v3.forward
            canDash = false;
            return dummyReference.forward * (dashDistance);
        }
    }

    public void OnMoveHit(ref Vector3 curPosition, ref Vector3 curVelocity, Collider other, Vector3 direction, float pen)
    {
        Vector3 clipped = ClipVelocity(curVelocity, direction);
        
        // floor
        if (groundLayers.Test(other.gameObject.layer) &&  // require ground layer
            direction.y > 0 &&                                      // direction check
            Vector3.Angle(direction, Vector3.up) < maxGroundAngle)  // angle check
        {
            // only change Y-position if bumping into the floor
            curPosition.y += direction.y * (pen);
            curVelocity.y = clipped.y;
            
            Grounded = true;
        }
        // other
        else
        {
            curPosition += direction * (pen);
            curVelocity = clipped;
        }
    }

    public void OnPreMove()
    {
        // reset frame data
        jumpedThisFrame = false;
        Grounded = false;
    }
    public void OnFinishMove(ref Vector3 curPosition, ref Vector3 curVelocity)
    {
        // Ground Adhesion

        // early exit if we're already grounded or jumping
        if (Grounded || jumpedThisFrame || !wasGrounded) return;
        
        var groundCandidates = body.Cast(curPosition, Vector3.down, maxGroundAdhesionDistance, groundLayers);
        Vector3 snapPosition = curPosition;
        foreach (var candidate in groundCandidates)
        {
            // ignore colliders that we start inside of - it's either us or something bad happened
            if(candidate.point == Vector3.zero) { continue; }

            // NOTE: This code assumes that the ground will always be below us
            snapPosition.y = candidate.point.y - body.FootOffset.y - body.contactOffset;
            
            // Snap to the ground - perform any necessary collision and sliding logic
            body.DeferredCollideAndSlide(ref snapPosition, ref curVelocity, candidate.collider);
            //Debug.Assert(snapPosition.y >= candidate.point.y, "Snapping put us underneath the ground?!");
            break;
        }

        curPosition = snapPosition;
    }

    public void OnPostMove()
    {
        // record grounded status for next frame
        wasGrounded = Grounded;

        if (transform.position.y <= -40.0f) 
        {
            body.InternalVelocity = Vector3.zero;
            transform.position = startingPosition;
        }
    }
    
    //
    // Unity Messages
    //

    private void Start()
    {
        startingPosition = transform.position;

        dashDelayMax = dashDelay;
        maxDashTime = dashTime;


        body.motor = this;
        OnValidate();
    }
    private void Update()
    {
        UpdateTimer();
    }

    private void OnValidate()
    {
        if(body == null ||body.BodyCollider == null) { return; }
    }
}
