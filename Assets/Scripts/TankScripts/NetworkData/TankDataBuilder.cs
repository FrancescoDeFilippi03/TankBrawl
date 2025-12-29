
//BUILDER: per dati da salvare nel NetworkVariable TankConfigData
public class TankDataBuilder
{
    private TankConfigData _data;

    public TankDataBuilder()
    {
        _data = new TankConfigData();
    }

    public TankDataBuilder WithTeam(TeamColor team)
    {
        _data.Team = team;
        return this;
    }

    public TankDataBuilder WithClientId(ulong clientId)
    {
        _data.ClientId = clientId;
        return this;
    }

    public TankDataBuilder WithPlayerId(string playerId)
    {
        _data.PlayerId = new Unity.Collections.FixedString64Bytes(playerId);
        return this;
    }

    public TankDataBuilder WithTankId(int tankId)
    {
        _data.TankId = tankId;
        return this;
    }



    public TankConfigData Build() => _data;
}
