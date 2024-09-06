using UnityEngine;

public class HeadIKSetObjectCol : MonoBehaviour
{
    public Transform objectLookAtPoition; // The object to add to the array

    // Private reference to HeadIKEditor
    private HeadIKEditor _headIKEditor;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger has a HeadIKEditor component
        if (other.TryGetComponent(out _headIKEditor))
        {
            // Check if the objectLookAtPoition is not already in the array
            if (_headIKEditor.objectLookAtPositions != null)
            {
                // Create a list from the array for easier manipulation
                var objectList = new System.Collections.Generic.List<Transform>(_headIKEditor.objectLookAtPositions);

                // Add the new object if it is not already in the list
                if (!objectList.Contains(objectLookAtPoition))
                {
                    objectList.Add(objectLookAtPoition);
                    _headIKEditor.objectLookAtPositions = objectList.ToArray(); // Convert back to array
                    _headIKEditor.useRay = false;
                    Debug.Log("Head IK is now looking at: " + objectLookAtPoition.name);
                }
            }
            else
            {
                Debug.LogWarning("objectLookAtPositions array is null.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object exiting the trigger has a HeadIKEditor component
        if (other.TryGetComponent(out _headIKEditor))
        {
            if (_headIKEditor.objectLookAtPositions != null)
            {
                // Create a list from the array for easier manipulation
                var objectList = new System.Collections.Generic.List<Transform>(_headIKEditor.objectLookAtPositions);

                // Remove the object if it exists in the list
                if (objectList.Contains(objectLookAtPoition))
                {
                    objectList.Remove(objectLookAtPoition);
                    _headIKEditor.objectLookAtPositions = objectList.ToArray(); // Convert back to array
                    _headIKEditor.useRay = true;
                    Debug.Log("Head IK has stopped looking at: " + objectLookAtPoition.name);
                }
            }
            else
            {
                Debug.LogWarning("objectLookAtPositions array is null.");
            }
        }
    }
}
