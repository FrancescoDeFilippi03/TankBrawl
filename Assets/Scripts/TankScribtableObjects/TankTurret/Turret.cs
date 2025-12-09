using UnityEngine;

[CreateAssetMenu(fileName = "Turret", menuName = "TankElements/Turret")]
public class Turret : ScriptableObject
{
    public string turretName;
    public float rangeMultiplier;
    public float fireRateMultiplier;
    public float damageMultiplier;
    public float ammoMultiplier;
    public Sprite turretSpriteRed;
    public Sprite turretSpriteBlue;
}
