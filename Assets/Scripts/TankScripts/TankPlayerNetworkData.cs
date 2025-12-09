using Unity.Netcode;
using UnityEngine;

// 1. CONFIGURAZIONE (Dati pesanti, cambiano quasi mai)
[System.Serializable]
public struct TankConfigData : INetworkSerializable
{
    public TeamColor Team;
    public int BaseId;
    public int TurretId;
    public int WeaponId;
    public int BulletId;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Team);
        serializer.SerializeValue(ref BaseId);
        serializer.SerializeValue(ref TurretId);
        serializer.SerializeValue(ref WeaponId);
        serializer.SerializeValue(ref BulletId);
    }
}

// 2. PUNTEGGIO (Leggero, cambia agli eventi)
[System.Serializable]
public struct TankScoreData : INetworkSerializable
{
    public int Kills;
    public int Deaths;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Kills);
        serializer.SerializeValue(ref Deaths);
    }
}