using UnityEngine;
using UnityEngine.UIElements;

public class TankMainUI : UI
{
    [SerializeField] private UIDocument uiDocument;
    
    [SerializeField] private VisualTreeAsset bulletTemplate; 
    
    private Tank subscribedTank;

    // Bars
    private VisualElement ammoBar;
    private VisualElement healthBar;
    private VisualElement armorBar;

    //labels
    private Label ammoLabel;
    private Label healthLabel;
    private Label armorLabel;

    protected override void OnEnable()
    {
        base.OnEnable();

        ammoBar = root.Q<VisualElement>("AmmoBar");
        healthBar = root.Q<VisualElement>("HealthBar");
        armorBar = root.Q<VisualElement>("ArmorBar");

        ammoLabel = root.Q<Label>("AmmoLabel");
        healthLabel = root.Q<Label>("HealthLabel");
        armorLabel = root.Q<Label>("ArmorLabel");


    }

    public void SetupMainUI(int maxAmmo, float maxHealth, float maxArmor)
    {
        SetupAmmo(maxAmmo);
        SetupHealth(maxHealth);
        SetupArmor(maxArmor);
    }

    void SetupAmmo(int maxAmmo)
    {
        ammoBar.Clear();
        
        for (int i = 0; i < maxAmmo; i++)
        {
            bulletTemplate.CloneTree(ammoBar);
        }
        ammoLabel.text = $"{maxAmmo}/{maxAmmo}";
    }

    void SetupHealth(float maxHealth)
    {
        healthLabel.text = $"{maxHealth}";
    }

    void SetupArmor(float maxArmor)
    {
        armorLabel.text = $"{maxArmor}";
    }
    public void UpdateAmmo(int currentCount, int maxCount)
    {
        for (int i = 0; i < ammoBar.childCount; i++)
        {
            ammoBar[i].visible = i < currentCount;
        }
        ammoLabel.text = $"{currentCount}/{maxCount}";
    }

    public void UpdateHealth(float previousHealth, float currentHealth)
    {
        healthLabel.text = $"{currentHealth:F0}";
        healthBar.style.width = Length.Percent(currentHealth / subscribedTank.TankConfig.maxHealth * 100);
    }

    public void UpdateArmor(float previousShield, float currentShield)
    {
        armorLabel.text = $"{currentShield:F0}";
        armorBar.style.maxHeight = Length.Percent(currentShield / subscribedTank.TankConfig.maxShield * 100);
    }

    public void SubscribeToTank(Tank tank)
    {
        if (subscribedTank != null)
        {
            UnsubscribeFromTank();
        }

        subscribedTank = tank;
        
        // Subscribe to tank events
        tank.OnHealthChanged += UpdateHealth;
        tank.OnShieldChanged += UpdateArmor;
        
        if (tank.ShootingSystem != null)
        {
            tank.ShootingSystem.OnAmmoChanged += UpdateAmmo;
        }

        SetupMainUI(tank.TankConfig.weaponData.ammoCapacity,
                    tank.TankConfig.maxHealth, 
                    tank.TankConfig.maxShield);
        
    }


    private void UnsubscribeFromTank()
    {
        if (subscribedTank != null)
        {
            subscribedTank.OnHealthChanged -= UpdateHealth;
            subscribedTank.OnShieldChanged -= UpdateArmor;
            
            if (subscribedTank.ShootingSystem != null)
            {
                subscribedTank.ShootingSystem.OnAmmoChanged -= UpdateAmmo;
            }
            
            subscribedTank = null;
        }
    }

    void OnDestroy()
    {
        UnsubscribeFromTank();
    }
}