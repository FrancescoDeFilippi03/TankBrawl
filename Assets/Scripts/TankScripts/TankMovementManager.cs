using UnityEngine;

public class TankMovementManager : MonoBehaviour
{
    [SerializeField] private float movementSmoothing = 5f;
    public float MovementSmoothing => movementSmoothing;

    [SerializeField] private float rotationSmoothing = 7f;
    public float RotationSmoothing => rotationSmoothing;

    private Vector2 smoothedMovementInput;
    public Vector2 SmoothedMovementInput
    {
        get => smoothedMovementInput;
        set => smoothedMovementInput = value;
    }
    private Vector2 currentVelocity = Vector2.zero;


    private HullConfig hullConfig;
    public HullConfig HullConfig => hullConfig;
    
    private TrackConfig trackConfig;    
    public TrackConfig TrackConfig => trackConfig;

    public void InitializeMovement(HullConfig hullConfig, TrackConfig trackConfig)
    {
        this.hullConfig = hullConfig;
        this.trackConfig = trackConfig;
    }
    
    public void MoveTank(Vector2 MovementInput,Rigidbody2D rb)
    {
        SmoothedMovementInput = Vector2.SmoothDamp(
            SmoothedMovementInput,
            MovementInput,
            ref currentVelocity,
            1f / movementSmoothing
        );

        if (SmoothedMovementInput.magnitude > 0.01f)
        {
            Vector2 movement = hullConfig.speed * Time.fixedDeltaTime * SmoothedMovementInput;
            
            rb.MovePosition(rb.position + movement);
        }

        if (MovementInput.magnitude > 0.1f)
        {
            HandleRotation(MovementInput, rb);
        }
    }

    public void HandleRotation(Vector2 inputDirection , Rigidbody2D rb)
    {
        float targetAngle = Mathf.Atan2(inputDirection.y, inputDirection.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

        Quaternion currentRotation = Quaternion.Euler(0, 0, rb.rotation);

        Quaternion newRotation = Quaternion.Slerp(
            currentRotation,
            targetRotation,
            RotationSmoothing * Time.fixedDeltaTime
        );

        rb.MoveRotation(newRotation.eulerAngles.z);
    }

    
    public void Dash( Vector2 movementInput , Rigidbody2D rb)
    {
        Vector2 dashVelocity = Time.fixedDeltaTime * trackConfig.dashSpeed * movementInput.normalized;
        rb.MovePosition(rb.position + dashVelocity);
    }
}
