
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

    public TankDataBuilder WithLoadout()
    {
        PlayerLoadoutData playerLoadoutData = LoadoutSystem.LoadLoadout();

        _data.BaseId = playerLoadoutData.BaseId;
        _data.TurretId = playerLoadoutData.TurretId;
        _data.WeaponId = playerLoadoutData.WeaponId;
        _data.BulletId = playerLoadoutData.BulletId;
        return this;
    }

    public TankConfigData Build() => _data;
}
