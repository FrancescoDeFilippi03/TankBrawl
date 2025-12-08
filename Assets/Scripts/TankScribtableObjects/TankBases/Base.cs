using UnityEngine;

[CreateAssetMenu(fileName = "TankBase", menuName = "TankElements/TankBase")]
public class TankBase : ScriptableObject
{
    public string baseName;
    public int health;
    public float speed;
    public float armor;
    public Sprite baseSprite;
}
