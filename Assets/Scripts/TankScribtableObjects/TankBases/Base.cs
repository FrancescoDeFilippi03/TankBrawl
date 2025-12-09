using UnityEngine;

[CreateAssetMenu(fileName = "TankBase", menuName = "TankElements/TankBase")]
public class Base : ScriptableObject
{
    public string baseName;
    public int health;
    public float speed;
    public float armor;
    public Sprite baseSpriteRed;
    public Sprite baseSpriteBlue;
}
