using System.Collections.Generic;
using UnityEngine;

public class TankHullList : MonoBehaviour
{
    public List<GameObject> hulls = new List<GameObject>();
    public List<HullConfig> hullConfigs = new List<HullConfig>();
    public void SetHullActive(int hullId)
    {
        for (int i = 0; i < hulls.Count; i++)
        {
            hulls[i].SetActive(i == hullId);
        }
    }

    public GameObject GetActiveHull()
    {
        foreach (var hull in hulls)
        {
            if (hull.activeSelf)
                return hull;
        }
        return null;
    }

    public HullConfig GetActiveHullConfig()
    {
        for (int i = 0; i < hulls.Count; i++)
        {
            if (hulls[i].activeSelf)
                return hullConfigs[i];
        }
        return null;
    }

    public HullConfig GetHullConfigById(int hullId)
    {
        if (hullId >= 0 && hullId < hullConfigs.Count)
        {
            return hullConfigs[hullId];
        }
        return null;
    }

}
