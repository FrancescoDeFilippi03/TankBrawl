using UnityEngine;
using UnityEngine.UIElements;

public class TankMainUI : UI
{
    public static TankMainUI Instance { get; private set; }
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

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        ammoBar = root.Q<VisualElement>("AmmoBar");
        healthBar = root.Q<VisualElement>("HealthBar");
        armorBar = root.Q<VisualElement>("ArmorBar");

        ammoLabel = root.Q<Label>("AmmoLabel");
        healthLabel = root.Q<Label>("HealthLabel");
        armorLabel = root.Q<Label>("ArmorLabel");

        Hide();
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
    public void UpdateAmmo(int previousAmmo, int currentAmmo)
    {
        for (int i = 0; i < ammoBar.childCount; i++)
        {
            ammoBar[i].visible = i < currentAmmo;
        }
        ammoLabel.text = $"{currentAmmo}/{subscribedTank.TankConfig.AmmoCapacity}";
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
        subscribedTank.OnHealthChanged += UpdateHealth;
        subscribedTank.OnShieldChanged += UpdateArmor;
        subscribedTank.currentAmmo.OnValueChanged += (current, max) => UpdateAmmo(current, max);

        SetupMainUI(subscribedTank.TankConfig.AmmoCapacity,
                    subscribedTank.TankConfig.maxHealth, 
                    subscribedTank.TankConfig.maxShield);
        
    }

    public void UnsubscribeFromTank()
    {
        if (subscribedTank != null)
        {
            subscribedTank.OnHealthChanged -= UpdateHealth;
            subscribedTank.OnShieldChanged -= UpdateArmor;
            subscribedTank.currentAmmo.OnValueChanged -= (current, max) => UpdateAmmo(current, max);
            
            subscribedTank = null;
        }
    }
}