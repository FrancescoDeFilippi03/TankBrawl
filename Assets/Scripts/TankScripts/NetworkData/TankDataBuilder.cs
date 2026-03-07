
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

    public TankDataBuilder WithPlayerId(string playerName)
    {
        _data.PlayerName = new Unity.Collections.FixedString64Bytes(playerName);
        return this;
    }

    public TankDataBuilder WithTankId(int tankId)
    {
        _data.TankId = tankId;
        return this;
    }
    public TankConfigData Build() => _data;
}
