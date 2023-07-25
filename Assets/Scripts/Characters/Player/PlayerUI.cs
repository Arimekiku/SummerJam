using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Image[] healthContainers;
    [Space, SerializeField] private Transform ammoContainer;
    [SerializeField] private Transform pocketAmmoContainer;
    [SerializeField] private Image weaponImage;

    private Image magazineToSpawn;

    private readonly List<Image> pocketAmmoSprites = new List<Image>();

    private void Awake()
    {
        magazineToSpawn = Resources.Load("Prefabs/Weapon/PlayerWeapon/AmmoUI", typeof(Image)) as Image;
    }

    public void UpdateHealth(int amount)
    {
        int i = 0;
        
        foreach (Image healthContainer in healthContainers)
            healthContainer.enabled = ++i < amount;
    }

    public void ClearWeapon()
    {
        weaponImage.sprite = null;
        pocketAmmoSprites.ForEach(s => Destroy(s.gameObject));
        pocketAmmoSprites.Clear();
    }

    public void UpdateWeapon(PlayerRangedWeapon newWeapon, Dictionary<Type, int> ammoPocket)
    {
        weaponImage.sprite = newWeapon.WeaponSprite;
        
        newWeapon.currentAmmoUI = new AmmunitionUI[newWeapon.MaxAmmo];

        for (int i = 0; i < newWeapon.MaxAmmo; i++)
        {
            AmmunitionUI uiToSpawn = newWeapon.currentAmmoUI[i] = Instantiate(newWeapon.AmmoUI, ammoContainer);
                    
            if (i < newWeapon.CurrentAmmo)
                uiToSpawn.Reload();
            else
                uiToSpawn.Deplete();
        }

        if (ammoPocket.TryGetValue(newWeapon.GetType(), out int ammoCount))
        {
            int magazinesToSpawn = ammoCount % newWeapon.MaxAmmo;
            
            for (int i = 0; i < magazinesToSpawn; i++)
            {
                Image imageToSpawn = Instantiate(magazineToSpawn, pocketAmmoContainer);
                pocketAmmoSprites.Add(imageToSpawn);

                imageToSpawn.transform.localPosition = new Vector2(0, 30 * i);
            }    
        }
    }

    public void DepleteAmmo()
    {
        if (pocketAmmoSprites.Count != 0)
        {
            Image imageToDestroy = pocketAmmoSprites.Last();
            pocketAmmoSprites.Remove(imageToDestroy);
            Destroy(imageToDestroy.gameObject);
        }
    }

    public void RestoreAmmo()
    {
        Image imageToSpawn = Instantiate(magazineToSpawn, pocketAmmoContainer);

        if (pocketAmmoSprites.Count != 0)
            imageToSpawn.transform.localPosition = pocketAmmoSprites.Last().transform.localPosition + Vector3.up * 30f;
        else
            imageToSpawn.transform.localPosition = Vector3.zero;
        
        pocketAmmoSprites.Add(imageToSpawn);
    }
}
