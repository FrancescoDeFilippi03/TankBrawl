using System;
using Unity.Collections;
using Unity.Netcode;

// CONFIGURAZIONE (Dati pesanti, cambiano quasi mai)
[System.Serializable]
public struct TankConfigData : INetworkSerializable,IEquatable<TankConfigData>
{
    public FixedString64Bytes PlayerId;
    public ulong ClientId;
    public TeamColor Team;
    public int BaseId;
    public int WeaponId;
    public int BulletId;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Team);
        serializer.SerializeValue(ref BaseId);
        serializer.SerializeValue(ref WeaponId);
        serializer.SerializeValue(ref BulletId);
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref PlayerId);
    }

    public readonly bool Equals(TankConfigData other)
    {
        return Team == other.Team &&
               BaseId == other.BaseId &&
               WeaponId == other.WeaponId &&
               BulletId == other.BulletId &&
               ClientId == other.ClientId &&
               PlayerId == other.PlayerId;
    }

    
}