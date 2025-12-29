using UnityEngine;

public class TankShootingManager : MonoBehaviour
{
    
    [SerializeField] private Transform weaponPivotTransform;
    public Transform WeaponPivotTransform => weaponPivotTransform;

    private Vector2 aimDirection;

    [SerializeField] private float rotationSmoothing = 7f;


    [SerializeField] ShootingSystem shootingSystem;
    public ShootingSystem ShootingSystem => shootingSystem;

    public void InitializeShooting(ShootingType shootingType, float fireRate,float range, float damage,GameObject bulletPrefab ,int ammoCapacity)
    {
        shootingSystem.InitializeWeapon( shootingType, fireRate, range, damage, bulletPrefab, ammoCapacity);
    }


    public void Shoot(bool isHoldingTrigger)
    {
        shootingSystem.TryShoot(aimDirection, isHoldingTrigger);
    }

    Vector2 GetAimDirection(Vector2 aimInput){
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(aimInput.x, aimInput.y, 0f));
        Vector2 direction = (worldMousePosition - weaponPivotTransform.position).normalized;
        return direction;
    }

    public void HandleTurretRotation(Vector2 aimInput)
    {
        
        aimDirection = GetAimDirection(aimInput);

        float targetAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        weaponPivotTransform.rotation = Quaternion.Slerp(
            weaponPivotTransform.rotation,
            targetRotation,
            Time.fixedDeltaTime * rotationSmoothing
        );
    }

    public void CursorInitialization(Sprite crosshairSprite)
    {
        if (crosshairSprite != null)
        {
            Texture2D cursorTexture = crosshairSprite.texture;
            Vector2 hotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
            Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
        }
        Cursor.visible = true;
    }

    public void CursorReset()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}

