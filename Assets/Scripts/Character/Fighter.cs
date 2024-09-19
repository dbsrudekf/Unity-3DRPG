using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Fighter : MonoBehaviour, IAction
{
    [SerializeField]
    private float timeBetweenAttacks = 1f;
    float timeSinceLastAttack = Mathf.Infinity;

    [SerializeField]
    private Transform rightHandTransform = null;
    [SerializeField]
    private Transform leftHandTransform = null;
    [SerializeField]
    private WeaponConfig defaultWeapon = null;
    private WeaponConfig currentWeaponConfig;
    private Weapon currentWeapon;

    [SerializeField]
    private Health target;

    private Equipment equipment;

    private void Awake()
    {
        currentWeaponConfig = defaultWeapon;
        currentWeapon = SetupDefaultWeapon();
        equipment = GetComponent<Equipment>();
        if (equipment)
        {
            equipment.equipmentUpdated += UpdateWeapon;
        }
    }


    private void Update()
    {
        timeSinceLastAttack += Time.deltaTime;

        if (target == null)
        {
            return;
        }

        if (target.IsDead())
        {
            return;
        }

        if (!GetIsInRange(target.transform))
        {
            GetComponent<Mover>().MoveTo(target.transform.position, 1f);
        }
        else
        {
            GetComponent<Mover>().Cancel();
            AttackBehaviour();
        }

    }

    private Weapon SetupDefaultWeapon()
    {

        return AttachWeapon(defaultWeapon);
    }

    private Weapon AttachWeapon(WeaponConfig weapon)
    {
        Animator animator = GetComponent<Animator>();
        return weapon.Spawn(rightHandTransform, leftHandTransform, animator);
    }

    //Animation Event
    void Hit()
    {
        if (target == null) { return; }

        float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

        if (currentWeapon != null)
        {
            currentWeapon.OnHit();
        }

        if (currentWeaponConfig.HasProjectile())
        {
            currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, currentWeaponConfig.GetWeaponDamage());
        }
        else
        {
            target.TakeDamage(gameObject, damage);
        }

    }

    void Shoot()
    {
        Hit();
    }

    private void AttackBehaviour()
    {
        transform.LookAt(target.transform);

        if (timeSinceLastAttack > timeBetweenAttacks)
        {
            TriggerAttack();
            timeSinceLastAttack = 0;

        }

    }

    private void TriggerAttack()
    {
        GetComponent<Animator>().ResetTrigger("stopAttack");
        GetComponent<Animator>().SetTrigger("attack");
    }

    public bool CanAttack(GameObject combatTarget)
    {
        if (combatTarget == null) { return false; }
        if (!GetComponent<Mover>().CanMoveTo(combatTarget.transform.position) && !GetIsInRange(combatTarget.transform)) return false;
        Health targetToTest = combatTarget.GetComponent<Health>();
        return targetToTest != null && !targetToTest.IsDead();
    }

    private bool GetIsInRange(Transform targetTransform)
    {
        return Vector3.Distance(transform.position, target.transform.position) < currentWeaponConfig.GetWeponRange();
    }

    public void Attack(GameObject combattarget)
    {
        GetComponent<ActionScheduler>().StartAction(this);
        target = combattarget.GetComponent<Health>();
    }

    public Health GetTarget()
    {
        return target;
    }

    public Transform GetHandTransform(bool isRightHand)
    {
        if (isRightHand)
        {
            return rightHandTransform;
        }
        else
        {
            return leftHandTransform;
        }
    }

    public void Cancel()
    {
        StopAttack();
        target = null;
    }
    private void StopAttack()
    {
        GetComponent<Animator>().ResetTrigger("attack");
        GetComponent<Animator>().SetTrigger("stopAttack");
    }

    public void EquipWeapon(WeaponConfig weapon)
    {
        currentWeaponConfig = weapon;
        currentWeapon = AttachWeapon(weapon);
    }

    private void UpdateWeapon()
    {
        var weapon = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
        if (weapon == null)
        {
            EquipWeapon(defaultWeapon);
        }
        else
        {
            EquipWeapon(weapon);
        }
    }
}
