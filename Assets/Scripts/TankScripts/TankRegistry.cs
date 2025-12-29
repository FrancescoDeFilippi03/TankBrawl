using UnityEngine;
using System.Collections.Generic;

public class TankRegistry : MonoBehaviour
{
    public static TankRegistry Instance { get; private set; }
    private void Awake()
    {
        // Singleton Pattern
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); 
            return;
        }
        Instance = this;
    }
    
}