using Unity.Netcode;
using UnityEngine;

public class TankPlayerData : NetworkBehaviour
{
    /* //Tank Elements
    private WeaponConfig tankWeapon;
    public WeaponConfig TankWeapon => tankWeapon;
    private HullConfig  tankBase;
    public HullConfig TankBase => tankBase;

    private TrackConfig tankTrack;
    public TrackConfig TankTrack => tankTrack;

    public void InitTankElements(TankConfigData configData)
    {
        tankBase   = TankRegistry.Instance.GetBase(configData.BaseId);
        tankWeapon = TankRegistry.Instance.GetWeapon(configData.WeaponId);
        tankTrack  = TankRegistry.Instance.GetTrack(configData.TrackId);
    } */


    [SerializeField] private TankHullList tankHullList;
    [SerializeField] private TankWeaponList tankWeaponList;
    [SerializeField] private TankTrackList tankTrackListLeft;
    [SerializeField] private TankTrackList tankTrackListRight;

    [SerializeField] NetworkVariable<int> hullId = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    [SerializeField] NetworkVariable<int> weaponId = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    [SerializeField] NetworkVariable<int> trackId = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    [SerializeField]private TankPlayerController playerController;

    public override void OnNetworkSpawn()
    {
        hullId.OnValueChanged += OnHullIdChanged;
        weaponId.OnValueChanged += OnWeaponIdChanged;
        trackId.OnValueChanged += OnTrackIdChanged;
    }

    public void InitializeTankPlayerData()
    {
        

        TankConfigData configData = TeamManager.Instance.GetTankConfigDataForClient(OwnerClientId);

        if (!IsOwner) return;
        hullId.Value = configData.HullId;
        weaponId.Value = configData.WeaponId;
        trackId.Value = configData.TrackId;

        Debug.Log($"Initialized TankPlayerData for Client {OwnerClientId} with HullID: {hullId.Value}, WeaponID: {weaponId.Value}, TrackID: {trackId.Value}");
    }   

    public override void OnNetworkDespawn()
    {
        hullId.OnValueChanged -= OnHullIdChanged;
        weaponId.OnValueChanged -= OnWeaponIdChanged;
        trackId.OnValueChanged -= OnTrackIdChanged;
    }

    private void OnHullIdChanged(int previousValue, int newValue)
    {
        tankHullList.SetHullActive(newValue);

        HullConfig activeHull = tankHullList.GetHullConfigById(newValue);

        playerController.TankHealthManager.InitializeHealth(
            activeHull.health,
            activeHull.armor
        );

        playerController.TankMovementManager.InitializeMovement(
            activeHull,
            tankTrackListLeft.GetTrackConfigById(trackId.Value)
        );

        Debug.Log($"Hull changed to ID: {newValue}");
    }

    private void OnWeaponIdChanged(int previousValue, int newValue)
    {
        tankWeaponList.SetWeaponActive(newValue);


        WeaponConfig activeWeapon = tankWeaponList.GetWeaponConfigById(newValue);

        playerController.TankShootingManager.InitializeShooting(activeWeapon, tankWeaponList.GetActiveWeaponFirePoints());
        playerController.TankShootingManager.CursorInitialization(activeWeapon.crosshairSprite);

        Debug.Log($"Weapon changed to ID: {newValue}");
    }

    private void OnTrackIdChanged(int previousValue, int newValue)
    {
        tankTrackListLeft.SetTrackActive(newValue);
        tankTrackListRight.SetTrackActive(newValue);

        TrackConfig activeTrack = tankTrackListLeft.GetTrackConfigById(newValue);

        playerController.TankMovementManager.InitializeMovement(
            tankHullList.GetHullConfigById(hullId.Value),
            activeTrack
        );

        Debug.Log($"Track changed to ID: {newValue}");
    }

    public void SetHull(int id)
    {
        if (!NetworkObject.IsSpawned) {
            // Se non siamo in gioco, aggiorna solo la grafica per l'editor
            tankHullList.SetHullActive(id);
            return;
        }

        if (IsOwner) hullId.Value = id;
    }

    public void SetWeapon(int id)
    {
        if (!NetworkObject.IsSpawned) {
            tankWeaponList.SetWeaponActive(id);
            return;
        }

        if (IsOwner) weaponId.Value = id;
    }

    public void SetTrack(int id)
    {
        if (!NetworkObject.IsSpawned) {
            tankTrackListLeft.SetTrackActive(id);
            tankTrackListRight.SetTrackActive(id);
            return;
        }

        if (IsOwner) trackId.Value = id;
    }

}
