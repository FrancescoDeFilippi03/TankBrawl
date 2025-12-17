using Unity.Netcode;
using UnityEngine;

public class Explosion : NetworkBehaviour
{
    [SerializeField] private float damage = 50f;
    [SerializeField] private AnimationClip explosionAnimation;
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            StartCoroutine(DestroyAfterSeconds(explosionAnimation.length));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if (other.TryGetComponent<IDamageble>(out var damageble))
        {
            damageble.TakeDamage(damage);
        }
    }

    private System.Collections.IEnumerator DestroyAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (IsServer)
            NetworkObject.Despawn();
        else
            Destroy(gameObject);
    }
}
