using System;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;


[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(TankPlayerData))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class TankStateManager : NetworkBehaviour
{
    //This class will manage only the states of the tank
    private TankBaseState currentState;
    public TankBaseState CurrentState
    {
        get { return currentState; }
        set { currentState = value; }
    }

    private TankStateFactory stateFactory;
    public TankStateFactory StateFactory
    {
        get { return stateFactory; }
    }

    public NetworkVariable<PlayerState> playerState = new NetworkVariable<PlayerState>(PlayerState.Idle, 
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner
    );
    public enum PlayerState { 
        Idle,
        Moving,
        Dead 
    }    


    private TankPlayerData tankPlayerData;
    public TankPlayerData TankPlayerData => tankPlayerData;

    // Input
    private TankInput tankInput;
    public TankInput TankInput => tankInput;


    //movement variables
    private Vector2 movementInput;
    public Vector2 MovementInput => movementInput;

    private Vector2 smoothedMovementInput;
    public Vector2 SmoothedMovementInput
    {
        get => smoothedMovementInput;
        set => smoothedMovementInput = value;
    }

    private Vector2 aimInput;
    public Vector2 AimInput => aimInput;

    [SerializeField] private float movementSmoothing = 5f;
    public float MovementSmoothing => movementSmoothing;

    [SerializeField] private float rotationSmoothing = 7f;
    public float RotationSmoothing => rotationSmoothing;

    [SerializeField] private Sprite crosshairSprite;

    //private LineRenderer rangeIndicator;

    private Rigidbody2D rb;
    public Rigidbody2D Rb => rb;
    
    private Animator tankAnimator;
    public Animator TankAnimator => tankAnimator;

    public override void OnNetworkSpawn()
    {
        playerState.OnValueChanged += OnPlayerStateChanged;

        stateFactory = new TankStateFactory(this);
        tankInput = new TankInput();
        rb = GetComponent<Rigidbody2D>();
        tankPlayerData = GetComponent<TankPlayerData>();
        tankAnimator = GetComponent<Animator>();


        tankPlayerData.Init(TeamManager.Instance.GetTankConfigDataForClient(OwnerClientId));

        if (IsOwner)
        {
            tankInput.Enable();

            tankInput.Tank.Shoot.performed += OnShootPerformed;

            var cameraInScene = FindAnyObjectByType<Unity.Cinemachine.CinemachineCamera>();
            cameraInScene.Target.TrackingTarget = this.transform;


            crosshairSprite = tankPlayerData.TankWeapon.crosshairSprite;
            if (crosshairSprite != null)
            {
                Texture2D cursorTexture = crosshairSprite.texture;
                Vector2 hotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
                Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
            }
            Cursor.visible = true;
        }

        currentState = stateFactory.GetState(playerState.Value);
        currentState.Enter();
    }
    public override void OnNetworkDespawn()
    {
        playerState.OnValueChanged -= OnPlayerStateChanged;

        if (IsOwner)
        {
            tankInput.Tank.Shoot.performed -= OnShootPerformed;
            tankInput.Disable();
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    private void OnPlayerStateChanged(PlayerState previousValue, PlayerState newValue)
    {
        if(IsOwner) return;
        currentState.ChangeState(stateFactory.GetState(newValue));
    }
    private void Update()
    {
        currentState?.Update();
        currentState?.CheckStateChange();

        if (!IsOwner) return;
        movementInput = tankInput.Tank.Movement.ReadValue<Vector2>();
        aimInput = tankInput.Tank.Aim.ReadValue<Vector2>();
        
        HandleTurretRotation();
    }

    private void FixedUpdate()
    {
        currentState?.FixedUpdate();
    }


    
    void HandleTurretRotation()
    {
        Vector2 direction = GetAimDirection();

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        tankPlayerData.WeaponPivotTransform.rotation = Quaternion.Slerp(
            tankPlayerData.WeaponPivotTransform.rotation,
            targetRotation,
            Time.fixedDeltaTime * rotationSmoothing
        );
    }

    private void OnShootPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        Vector2 shootDirection = GetAimDirection();
        tankPlayerData.ShootingSystem.Shoot(shootDirection);
    }

    Vector2 GetAimDirection(){
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(aimInput.x, aimInput.y, 0f));
        Vector2 direction = (worldMousePosition - tankPlayerData.WeaponPivotTransform.position).normalized;
        return direction;
    }

    /*
    void SetupRangeIndicator()
    {
        GameObject rangeObj = new GameObject("RangeIndicator");
        rangeObj.transform.SetParent(transform);
        rangeIndicator = rangeObj.AddComponent<LineRenderer>();
        
        rangeIndicator.positionCount = 2;
        rangeIndicator.loop = false;
        rangeIndicator.useWorldSpace = true;
        rangeIndicator.startWidth = 0.1f;
        rangeIndicator.endWidth = 0.1f;
        rangeIndicator.material = new Material(Shader.Find("Sprites/Default"));
        rangeIndicator.startColor = new Color(1f, 1f, 1f, 0.5f);
        rangeIndicator.endColor = new Color(1f, 1f, 1f, 0.5f);
        
        UpdateRangeIndicator();
    }

    void UpdateRangeIndicator()
    {
        if (rangeIndicator == null) return;
        
        Vector2 direction = GetAimDirection();
        float range = tankPlayerData.TankWeapon.range;
        Vector3 startPos = tankPlayerData.WeaponPivotTransform.position;
        Vector3 endPos = startPos + (Vector3)(direction * range);
        
        rangeIndicator.SetPosition(0, startPos);
        rangeIndicator.SetPosition(1, endPos);
    }

    */
}
