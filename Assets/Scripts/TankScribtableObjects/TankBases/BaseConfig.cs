using UnityEngine;
using UnityEngine.U2D.Animation;

[CreateAssetMenu(fileName = "TankBase", menuName = "TankElements/TankBase")]
public class BaseConfig : ScriptableObject
{
    //statistics
    public string baseName;
    public int health;
    public float speed;
    public float armor;



    public Sprite baseSpriteRed;
    public Sprite baseSpriteBlue;
    public SpriteLibraryAsset trackSpriteLibraryAsset;
    public GameObject baseVisualPrefab;
}
