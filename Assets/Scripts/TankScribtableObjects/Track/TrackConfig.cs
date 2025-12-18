using UnityEngine;

[CreateAssetMenu(fileName = "Track", menuName = "TankElements/Track")]
public class TrackConfig : ScriptableObject
{
    public float trackMaxSpeed;
    public float trackAcceleration;
    public float rotationSpeed;
    public float frictionValue;
}
