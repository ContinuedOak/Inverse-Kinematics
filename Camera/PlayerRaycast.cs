using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRaycast : MonoBehaviour
{
    // Attach this script to the camera object

    [Header("Player Raycast Editor")]
    [Header("by ContinuedOak")]

    [Header("Main Config")]
    [Tooltip("Displays Current Objects Name (Used for debugging)")]
    public string returnCurrentObject;
    [Tooltip("The distant the raycast goes")]
    public int raycastDistance = 100;
    [Tooltip("Set the raycast color")]
    public Color raycastColor = Color.red;
    
    #region Private's
    private GameObject currentObjectBeingLookedAt;
    private Vector3 raycastEndPoint; // Store the end point of the ray
    #endregion

    #region Unity
    void Update()
    {
        DetectObjectLookingAt();

        // Check if the object being looked at is not null before accessing its name
        if (currentObjectBeingLookedAt != null)
        {
            returnCurrentObject = currentObjectBeingLookedAt.name;
        }
        else
        {
            returnCurrentObject = "No object being looked at";
        }
    }
    #endregion

    #region DetectObjectLookingAt
    void DetectObjectLookingAt()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            currentObjectBeingLookedAt = hit.collider.gameObject;
            raycastEndPoint = ray.GetPoint(raycastDistance); // Store the end point of the ray
        }
        else
        {
            currentObjectBeingLookedAt = null;
            raycastEndPoint = ray.GetPoint(raycastDistance); // Still store the end point, even if no hit
        }

        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, raycastColor);
    }
    #endregion

    #region Public voids
    // Optional: Get the object being looked at by other scripts
    public GameObject GetCurrentObjectBeingLookedAt()
    {
        return currentObjectBeingLookedAt;
    }

    // New method to get the raycast end point
    public Vector3 GetRaycastEndPoint()
    {
        return raycastEndPoint;
    }
    #endregion
}
