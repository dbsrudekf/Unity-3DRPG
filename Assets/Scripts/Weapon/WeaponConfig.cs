using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
public class WeaponConfig : EquipableItem, IModifierProvider
{
    [SerializeField]
    private float weaponRange = 2f;
    [SerializeField]
    private float weaponDamage = 5f;
    [SerializeField]
    private float percentageBonus = 0f;
    [SerializeField]
    private bool isRightHanded = true;



    [SerializeField]
    private AnimatorOverrideController animatorOverride = null;
    [SerializeField]
    private Weapon equippedPrefab = null;
    [SerializeField]
    private Projectile projectile = null;

    const string weaponName = "Weapon";

    public float GetWeponRange()
    {
        return weaponRange;
    }

    public float GetWeaponDamage()
    {
        return weaponDamage;
    }

    public float GetPercentageBonus()
    {
        return percentageBonus;
    }

    public Weapon Spawn(Transform righthand, Transform leftHand, Animator animator)
    {
        DestroyOldWeapon(righthand, leftHand);

        Weapon weapon = null;
        
        if (equippedPrefab != null)
        {
            Transform handTransform = GetTransform(righthand, leftHand);
            weapon = Instantiate(equippedPrefab, handTransform);
            weapon.gameObject.name = weaponName;
        }
        var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController; //animator.runtimeAnimatorController가 AnimatorOverrideController 타입인지 확인 아니면 null값 리턴

        if (animatorOverride != null)
        {
            animator.runtimeAnimatorController = animatorOverride;
        }
        else if (overrideController != null)
        {
            animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
        }

        return weapon;
    }

    private void DestroyOldWeapon(Transform righthand, Transform leftHand)
    {
        Transform oldWeapon = righthand.Find(weaponName);

        if (oldWeapon == null)
        {
            oldWeapon = leftHand.Find(weaponName);
        }
        if (oldWeapon == null) return;

        oldWeapon.name = "DESTROYING";
        Destroy(oldWeapon.gameObject);
    }

    private Transform GetTransform(Transform righthand, Transform leftHand)
    {
        Transform handTransform;
        if (isRightHanded) handTransform = righthand;
        else handTransform = leftHand;
        return handTransform;
    }

    public bool HasProjectile()
    {
        return projectile != null;
    }

    public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
    {
        Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
        projectileInstance.SetTarget(target, instigator, calculatedDamage);
    }

    public IEnumerable<float> GetAdditiveModifiers(Stat stat)
    {
        if (stat == Stat.Damage)
        {
            yield return weaponDamage;
        }
    }

    public IEnumerable<float> GetPercentageModifiers(Stat stat)
    {
        if (stat == Stat.Damage)
        {
            yield return percentageBonus;
        }
    }
}
