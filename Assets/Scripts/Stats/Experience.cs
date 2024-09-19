using GameDevTV.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experience : MonoBehaviour, ISaveable
{
    [SerializeField]
    private float experiencePoints = 0;

    public event Action onExperienceGained;

    private void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            GainExperience(Time.deltaTime * 1000);
        }
    }
    public float GetPoints()
    {
        return experiencePoints;
    }

    public void GainExperience(float experience)
    {
        experiencePoints += experience;
        onExperienceGained();
    }

    public object CaptureState()
    {
        return experiencePoints;
    }

    public void RestoreState(object state)
    {
        experiencePoints = (float)state;
    }
}
