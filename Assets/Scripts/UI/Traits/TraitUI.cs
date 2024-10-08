using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TraitUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI unassignedPointsText;
    [SerializeField] Button commitButton;

    TraitStore playerTraitStore = null;

    private void Start()
    {
        playerTraitStore = GameObject.FindGameObjectWithTag("Player").GetComponent<TraitStore>();
        commitButton.onClick.AddListener(playerTraitStore.Commit);
    }

    private void Update()
    {
        unassignedPointsText.text = playerTraitStore.GetUnassignedPoints().ToString();
    }
}
