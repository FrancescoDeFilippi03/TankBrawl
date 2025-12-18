using System.Collections.Generic;
using UnityEngine;

public class TankTrackList : MonoBehaviour
{
    public List<GameObject> tracks = new List<GameObject>();
    public List<TrackConfig> trackConfigs = new List<TrackConfig>();
    public void SetTrackActive(int trackId)
    {
        for (int i = 0; i < tracks.Count; i++)
        {
            tracks[i].SetActive(i == trackId);
        }
    }

    public GameObject GetActiveTrack()
    {
        foreach (var track in tracks)
        {
            if (track.activeSelf)
                return track;
        }
        return null;
    }
    public TrackConfig GetActiveTrackConfig()
    {
        for (int i = 0; i < tracks.Count; i++)
        {
            if (tracks[i].activeSelf)
                return trackConfigs[i];
        }
        return null;
    }

    

    public TrackConfig GetTrackConfigById(int trackId)
    {
        if (trackId >= 0 && trackId < trackConfigs.Count)
        {
            return trackConfigs[trackId];
        }
        return null;
    }

}
