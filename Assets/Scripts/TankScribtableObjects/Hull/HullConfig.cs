using UnityEngine;
using UnityEngine.U2D.Animation;

[CreateAssetMenu(fileName = "TankBase", menuName = "TankElements/Hull")]
public class HullConfig : ScriptableObject
{
    //statistics
    public string baseName;
    public int health;
    public float speed;
    public float armor;
    public float mass;

}
