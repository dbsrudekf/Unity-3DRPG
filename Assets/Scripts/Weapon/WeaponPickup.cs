using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour, IRaycastable
{
    [SerializeField]
    private WeaponConfig weapon = null;
    [SerializeField]
    private float healthToRestore = 0;
    [SerializeField]
    private float respawnTime = 5;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PickUp(other.gameObject);
        }
    }

    private void PickUp(GameObject subject)
    {
        if (weapon != null)
        {
            Debug.Log("weaponPickup");
            subject.GetComponent<Fighter>().EquipWeapon(weapon);
        }
        if (healthToRestore > 0)
        {
            subject.GetComponent<Health>().Heal(healthToRestore);
        }
        StartCoroutine(HideForSeconds(respawnTime));
    }

    private IEnumerator HideForSeconds(float seconds)
    {
        ShowPickup(false);
        yield return new WaitForSeconds(seconds);
        ShowPickup(true);
    }

    private void ShowPickup(bool shouldShow)
    {
        GetComponent<Collider>().enabled = shouldShow;

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(shouldShow);
        }
    }

    public bool HandleRaycast(PlayerController callingController)
    {
        if (Input.GetMouseButtonDown(0))
        {
            PickUp(callingController.gameObject);
        }
        return true;
    }

    public CursorType GetCursorType()
    {
        return CursorType.PickUp;
    }
}
