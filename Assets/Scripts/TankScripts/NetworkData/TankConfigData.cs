using System;
using Unity.Collections;
using Unity.Netcode;

// CONFIGURAZIONE (Dati pesanti, cambiano quasi mai)
[Serializable]
public struct TankConfigData : INetworkSerializable,IEquatable<TankConfigData>
{
    public FixedString64Bytes PlayerId;
    public ulong ClientId;
    public TeamColor Team;
    public int HullId;
    public int WeaponId;
    public int TrackId;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Team);
        serializer.SerializeValue(ref HullId);
        serializer.SerializeValue(ref WeaponId);
        serializer.SerializeValue(ref TrackId);
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref PlayerId);
    }

    public readonly bool Equals(TankConfigData other)
    {
        return  Team == other.Team &&
                HullId == other.HullId &&
                WeaponId == other.WeaponId &&
                TrackId == other.TrackId &&
                ClientId == other.ClientId &&
                PlayerId == other.PlayerId;
    }

    
}