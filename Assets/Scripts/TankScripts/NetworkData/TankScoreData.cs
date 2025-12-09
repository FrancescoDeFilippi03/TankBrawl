using Unity.Netcode;

// PUNTEGGIO (Leggero, cambia agli eventi)
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