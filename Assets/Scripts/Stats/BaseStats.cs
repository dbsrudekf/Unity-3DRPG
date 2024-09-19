
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStats : MonoBehaviour
{
    [Range(1, 99)]
    [SerializeField]
    private int startinglevel = 1;
    [SerializeField]
    private CharacterClass characterClass;
    [SerializeField]
    private Progression progression = null;
    [SerializeField]
    private GameObject levelUpParticleEffect = null;
    [SerializeField]
    private bool shouldUseModifiers = false;

    public event Action onLevelUp;

    private int currentLevel = 1;

    Experience experience;

    private void Awake()
    {
        experience = GetComponent<Experience>();
        currentLevel = CalculateLevel();
    }

    private void OnEnable()
    {
        if (experience != null)
        {
            experience.onExperienceGained += UpdateLevel;
        }
    }

    private void OnDisable()
    {
        if (experience != null)
        {
            experience.onExperienceGained -= UpdateLevel;
        }
    }

    private void UpdateLevel()
    {
        int newLevel = CalculateLevel();
        if (newLevel > currentLevel)
        {
            currentLevel = newLevel;
            LevelUpEffect();
            onLevelUp();
        }
    }

    private void LevelUpEffect()
    {
        Instantiate(levelUpParticleEffect, transform);
    }

    public float GetStat(Stat stat)
    {
        return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat) / 100);
    }

    private float GetBaseStat(Stat stat)
    {
        return progression.GetStat(stat, characterClass, GetLevel());
    }

    private float GetAdditiveModifier(Stat stat)
    {
        if (!shouldUseModifiers) return 0;
        
        float total = 0;
        foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
        {
            foreach (float modifier in provider.GetAdditiveModifiers(stat))
            {
                total += modifier;
            }
        }
        return total;
    }

    private float GetPercentageModifier(Stat stat)
    {
        if (!shouldUseModifiers) return 0;

        float total = 0;
        foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
        {          
            foreach (float modifier in provider.GetPercentageModifiers(stat))
            {
                total += modifier;
            }
        }
        return total;
    }

    public int GetLevel()
    {       
        return currentLevel = CalculateLevel();
    }

    private int CalculateLevel()
    {
        Experience experience = GetComponent<Experience>();

        if (experience == null) return startinglevel;

        float currentXP = experience.GetPoints();

        int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
        for (int levels = 1; levels <= penultimateLevel; levels++)
        {
            float XPToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, levels);
            if (XPToLevelUp > currentXP)
            {
                //Debug.Log(levels);
                return levels;
            }
        }
        return penultimateLevel + 1;
    }
}
