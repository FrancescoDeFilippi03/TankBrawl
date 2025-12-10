using Unity.Netcode;

// CONFIGURAZIONE (Dati pesanti, cambiano quasi mai)
[System.Serializable]
public struct TankConfigData : INetworkSerializable
{
    public TeamColor Team;
    public int BaseId;
    public int TurretId;
    public int WeaponId;
    public int BulletId;
    public bool IsReady;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Team);
        serializer.SerializeValue(ref BaseId);
        serializer.SerializeValue(ref TurretId);
        serializer.SerializeValue(ref WeaponId);
        serializer.SerializeValue(ref BulletId);
        serializer.SerializeValue(ref IsReady);
    }
}
