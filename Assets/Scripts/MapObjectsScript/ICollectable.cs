using UnityEngine;
using Unity.Netcode;
public abstract class ICollectable : NetworkBehaviour
{
    [SerializeField] protected float timeToSpawn = 10f;
    protected float spawnTimer = 0f;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected Collider2D collectibleCollider;    
    protected NetworkVariable<bool> isSpawned = new NetworkVariable<bool>(false, 
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    public override void OnNetworkSpawn()
    {
        isSpawned.OnValueChanged += OnSpawnedStateChanged;
        SetCollectibleActive(isSpawned.Value);
        
        // Ritarda la rotazione per assicurarsi che venga applicata dopo la sincronizzazione di rete
        Invoke(nameof(ApplyClientSideRotation), 0.1f);
    }

    public override void OnNetworkDespawn()
    {
        isSpawned.OnValueChanged -= OnSpawnedStateChanged;
    }

    private void OnSpawnedStateChanged(bool previousValue, bool newValue)
    {
        SetCollectibleActive(newValue);
    }

    private void ApplyClientSideRotation()
    {
        TeamColor localplayerTeam = TeamManager.Instance.GetTankConfigDataForClient(NetworkManager.Singleton.LocalClientId).Team;
        Debug.Log($"Local player team: {localplayerTeam} clientId: {NetworkManager.Singleton.LocalClientId}");
        if (localplayerTeam == TeamColor.Red)
        {
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }
    }


    public abstract void OnCollect(TankPlayerController collector);

    protected void SetCollectibleActive(bool active)
    {
        if (spriteRenderer != null) spriteRenderer.enabled = active;
        if (collectibleCollider != null) collectibleCollider.enabled = active;
    }

    protected void Update()
    {
        if (IsServer && !isSpawned.Value)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= timeToSpawn)
            {
                spawnTimer = 0f;
                isSpawned.Value = true;
            }
        }
    }
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;

        if (collision.TryGetComponent<TankPlayerController>(out var tankController))
        {
            OnCollect(tankController);
        }
    }

}