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
    public int TankId;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Team);
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref PlayerId);
        serializer.SerializeValue(ref TankId);
    }

    public readonly bool Equals(TankConfigData other)
    {
        return  Team == other.Team &&
                ClientId == other.ClientId &&
                TankId == other.TankId &&
                PlayerId == other.PlayerId;
    }

    
}