using System;
using Unity.Collections;
using Unity.Netcode;

[Serializable]
public struct SessionPlayerData : INetworkSerializable,IEquatable<SessionPlayerData>
{
    public FixedString64Bytes PlayerName;
    public ulong ClientId;
    public TeamColor Team;
    public int TankId;

    public SessionPlayerData(ulong clientId,string playerName, int tankId,TeamColor assignedTeam)
    {
        ClientId = clientId;
        PlayerName = playerName;
        TankId = tankId;
        Team = assignedTeam;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Team);
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref TankId);
    }

    public readonly bool Equals(SessionPlayerData other)
    {
        return  Team == other.Team &&
                ClientId == other.ClientId &&
                TankId == other.TankId &&
                PlayerName == other.PlayerName;
    }    
}