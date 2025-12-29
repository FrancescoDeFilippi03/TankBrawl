/* using UnityEngine;

public class TankDashingState : TankBaseState
{
    private float dashTimer;
    private float totalDuration;
    private Vector2 currentDashDirection;
    private readonly float dashSteeringSensitivity = 10f;
    private readonly AnimationCurve dashCurve = AnimationCurve.EaseInOut(0.25f, 1f, 1f, 0.25f);

    public TankDashingState(TankStateManager tankStateManager) : base(tankStateManager) { }

    public override void Enter()
    {
        // Debug per confermare l'ingresso
        Debug.Log("<color=green>DASH ENTERED</color>");
        
        // Blocchiamo l'input di attivazione nel controller
        tank.PlayerController.isDashing = true; 

        // Inizializziamo il timer a zero
        dashTimer = 0f;
        totalDuration = tank.PlayerController.Tank.MovementManager.DashDuration;

        // Direzione iniziale
        if (tank.PlayerController.MovementInput.magnitude > 0.1f)
            currentDashDirection = tank.PlayerController.MovementInput.normalized;
        else
            currentDashDirection = tank.transform.up;

        tank.PlayerController.TankAnimator.SetBool("isDashing", true);
    }

    public override void Update()
    {
        dashTimer += Time.deltaTime;
        if (dashTimer >= totalDuration)
        {
            EndDash();
        }
    }

    public override void FixedUpdate()
    {
        float normalizedTime = Mathf.Clamp01(dashTimer / totalDuration);

        if (tank.PlayerController.MovementInput.magnitude > 0.1f)
        {
            currentDashDirection = Vector2.Lerp(
                currentDashDirection, 
                tank.PlayerController.MovementInput.normalized, 
                Time.fixedDeltaTime * dashSteeringSensitivity).normalized;
        }

        float curveMultiplier = dashCurve.Evaluate(normalizedTime);
        Vector2 targetVelocity = curveMultiplier * tank.PlayerController.TankMovementManager.TrackConfig.dashSpeed * currentDashDirection;

        RaycastHit2D hit = Physics2D.Raycast(tank.transform.position, currentDashDirection, 0.8f);
        if (hit.collider != null && !hit.collider.isTrigger && hit.collider.gameObject != tank.gameObject)
        {
            Debug.Log("<color=orange>DASH COLLISION DETECTED </color > with " + hit.collider.name);
            tank.PlayerController.Rb.linearVelocity = Vector2.zero;
            EndDash();
            return;
        }

        tank.PlayerController.Rb.linearVelocity = targetVelocity;
    }

    public override void Exit()
    {
        Debug.Log("<color=red>DASH EXITED</color>");
        tank.PlayerController.TankAnimator.SetBool("isDashing", false);
        
        // Reset della variabile nel controller per permettere un nuovo dash in futuro
        tank.PlayerController.isDashing = false; 
        
        tank.PlayerController.Rb.linearVelocity = Vector2.zero;
        tank.PlayerController.Rb.angularVelocity = 0f;
    }

    void EndDash()
    {
        tank.playerState.Value = TankStateManager.PlayerState.Moving;
        ChangeState(tank.StateFactory.Moving());
    }
} */