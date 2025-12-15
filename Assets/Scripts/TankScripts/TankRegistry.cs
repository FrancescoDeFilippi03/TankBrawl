using UnityEngine;
using System.Collections.Generic;

public class TankRegistry : MonoBehaviour
{
    public static TankRegistry Instance { get; private set; }

    [Header("Griglie di Conversione (ID -> Oggetto)")]
    // L'ordine nella lista determina l'ID: Elemento 0 = ID 0, Elemento 1 = ID 1...
    public List<Base> AllBases;
    public List<Weapon> AllWeapons;
    public List<BulletConfig> AllBullets;

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
    
    public Base GetBase(int id) => 
        (id >= 0 && id < AllBases.Count) ? AllBases[id] : AllBases[0];
    public Weapon GetWeapon(int id) => 
        (id >= 0 && id < AllWeapons.Count) ? AllWeapons[id] : AllWeapons[0];

    public BulletConfig GetBullet(int id) => 
        (id >= 0 && id < AllBullets.Count) ? AllBullets[id] : AllBullets[0];
}