using GameDevTV.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, ISaveable
{
    [SerializeField]
    private float regenerationPercentage = 70f;

    [SerializeField]
    private float healthPoints;

    [SerializeField]
    private UnityEvent<float> takeDamage;

    bool wasDeadLastFrame = false;
    [System.Serializable]
    public class TakeDamageEvent : UnityEvent<float>
    {

    }

    [SerializeField]
    private UnityEvent onDie;

    private bool isDead = false;
    float destroyTime = 0;

    private void Awake()
    {
        healthPoints = GetInitialHealth();
    }

    private float GetInitialHealth()
    {
        return GetComponent<BaseStats>().GetStat(Stat.Health);
    }

    private void OnEnable()
    {
        GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
    }

    private void OnDisable()
    {
        GetComponent<BaseStats>().onLevelUp -= RegenerateHealth;
    }

    public void TakeDamage(GameObject instigator, float damage)
    {
        print(gameObject.name + " took damage" + damage);

        healthPoints = Mathf.Max(healthPoints - damage, 0);

        if (IsDead())
        {
            onDie.Invoke();
            Die();
            AwardExperience(instigator);

        }
        else
        {
            takeDamage.Invoke(damage);
        }
        UpdateState();
    }

    public void Heal(float healthToRestore)
    {
        healthPoints = MathF.Min(healthPoints + healthToRestore, GetMaxHealPoints());
        UpdateState();
    }

    public float GetHealthPoints()
    {
        return healthPoints;
    }

    public float GetMaxHealPoints()
    {
        return GetComponent<BaseStats>().GetStat(Stat.Health);
    }

    public float GetPercentage()
    {
        return GetFraction() * 100;
    }

    public float GetFraction()
    {
        return healthPoints / GetComponent<BaseStats>().GetStat(Stat.Health);
    }

    private void UpdateState()
    {
        Animator animator = GetComponent<Animator>();
        if (!wasDeadLastFrame && IsDead())
        {
            animator.SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        if (wasDeadLastFrame && !IsDead())
        {
            animator.Rebind();
        }

        wasDeadLastFrame = IsDead();
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        GetComponent<Animator>().SetTrigger("die");
        GetComponent<ActionScheduler>().CancelCurrentAction();
        GetComponent<CapsuleCollider>().enabled = false;
      
    }

    private void AwardExperience(GameObject instigator)
    {
        Experience experience = instigator.GetComponent<Experience>();
        if (experience == null) return;

        experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
    }

    private void RegenerateHealth()
    {
        float regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * (regenerationPercentage / 100);
        healthPoints = MathF.Max(healthPoints, regenHealthPoints);
    }

    public bool IsDead()
    {
        return healthPoints <= 0;
    }

    public object CaptureState()
    {
        return healthPoints;
    }

    public void RestoreState(object state)
    {
        healthPoints = (float)state;
        Debug.Log("Restore" + healthPoints);
        UpdateState();
    }
}
