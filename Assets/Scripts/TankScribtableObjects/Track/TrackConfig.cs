using UnityEngine;

[CreateAssetMenu(fileName = "Track", menuName = "TankElements/Track")]
public class TrackConfig : ScriptableObject
{
    public float trackMaxSpeed;
    public float trackAcceleration;
    public float maxMassHeld;
    public float dashSpeed;
    public float dashDuration;
}
