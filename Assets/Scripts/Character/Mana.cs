using GameDevTV.Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mana : MonoBehaviour, ISaveable
{
    float mana;

    private void Awake()
    {
        mana = GetMaxMana();
    }

    private void Update()
    {
        if (mana < GetMaxMana())
        {
            mana += GetRegenRate() * Time.deltaTime;
            if (mana > GetMaxMana())
            {
                mana = GetMaxMana();
            }
        }
    }

    public float GetMana()
    {
        return mana;
    }

    public float GetMaxMana()
    {
        return GetComponent<BaseStats>().GetStat(Stat.Mana);
    }

    public float GetRegenRate()
    {
        return GetComponent<BaseStats>().GetStat(Stat.ManaRegenRate);
    }

    public bool UseMana(float manaToUse)
    {
        if (manaToUse > mana)
        {
            return false;
        }
        mana -= manaToUse;
        return true;
    }

    public object CaptureState()
    {
        return mana;
    }

    public void RestoreState(object state)
    {
        mana = (float)state;
    }
}
