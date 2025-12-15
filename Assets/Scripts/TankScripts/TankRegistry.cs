using UnityEngine;
using System.Collections.Generic;

public class TankRegistry : MonoBehaviour
{
    public static TankRegistry Instance { get; private set; }

    [Header("Griglie di Conversione (ID -> Oggetto)")]
    // L'ordine nella lista determina l'ID: Elemento 0 = ID 0, Elemento 1 = ID 1...
    public List<BaseConfig> AllBases;
    public List<WeaponConfig> AllWeapons;
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
    
    public BaseConfig GetBase(int id) => 
        (id >= 0 && id < AllBases.Count) ? AllBases[id] : AllBases[0];
    public WeaponConfig GetWeapon(int id) => 
        (id >= 0 && id < AllWeapons.Count) ? AllWeapons[id] : AllWeapons[0];
}