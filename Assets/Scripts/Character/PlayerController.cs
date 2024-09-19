using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [System.Serializable]
    struct CursorMapping
    {
        public CursorType type;
        public Texture2D texture;
        public Vector2 hotspot;
    }

    [SerializeField]
    CursorMapping[] cursorMappings = null;

    private float maxNavMeshProjectionDistance = 10f;
    [SerializeField]
    private float raycastRadius = 1f;

    private Health health;

    private bool isDraginUI = false;

    [SerializeField] int numberOfAbilities = 6;
    private ActionStore actionStore;

    private void Awake()
    {
        health = GetComponent<Health>();
        actionStore = GetComponent<ActionStore>();
    }
    private void Update()
    {
        if (InteractWithUI()) return;

        if (health.IsDead())
        {
            SetCursor(CursorType.None);
            return;
        }

        UseAbilities();

        if (InteractWithComponent()) return;
        if (InteractWithMovement()) return;

        SetCursor(CursorType.None);
    }

    private bool InteractWithComponent()
    {
        RaycastHit[] hits = RaycastAllSorted();

        foreach (RaycastHit hit in hits)
        {
            IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
            foreach (IRaycastable raycastable in raycastables)
            {
                if (raycastable.HandleRaycast(this))
                {
                    SetCursor(raycastable.GetCursorType());
                    return true;
                }
            }
        }
        return false;
    }

    private RaycastHit[] RaycastAllSorted()
    {
        RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);
        float[] distance = new float[hits.Length];
        for (int i = 0; i < hits.Length; i++)
        {
            distance[i] = hits[i].distance;
        }
        Array.Sort(distance, hits); //디폴트 오름차순
        return hits;
    }
    private bool InteractWithUI()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDraginUI = false;
        }
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                isDraginUI = true;
            }
            SetCursor(CursorType.UI);
            return true;
        }

        if (isDraginUI)
        {
            return true;
        }
        return false;
    }

    private bool InteractWithMovement()
    {
        Vector3 target;

        bool hasHit = RaycastNavMesh(out target);
        if (hasHit)
        {

            if (!GetComponent<Mover>().CanMoveTo(target))
            {
                return false;
            }

            if (Input.GetMouseButton(0))
            {
                GetComponent<Mover>().StartMoveAction(target, 1f);
            }
            SetCursor(CursorType.Movement);
            return true;
        }
        return false;
    }

    private bool RaycastNavMesh(out Vector3 target)
    {
        target = new Vector3();

        RaycastHit hit;
        bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
        if (!hasHit) return false;

        NavMeshHit navMeshHit;
        bool hasCastToNavMesh = NavMesh.SamplePosition(hit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);
        
        if (!hasCastToNavMesh) return false;

        target = navMeshHit.position;

        return true;
    }

    public static Ray GetMouseRay()
    {      
        return Camera.main.ScreenPointToRay(Input.mousePosition);
    }

    private void SetCursor(CursorType type)
    {
        CursorMapping mapping = GetCursorMapping(type);
        Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
    }

    private CursorMapping GetCursorMapping(CursorType type)
    {
        foreach (CursorMapping mapping in cursorMappings)
        {
            if (mapping.type == type)
            {
                return mapping;
            }
        }

        return cursorMappings[0];
    }

    private void UseAbilities()
    {
        for (int i = 0; i < numberOfAbilities; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                actionStore.Use(i, gameObject);
            }
        }
    }

}
