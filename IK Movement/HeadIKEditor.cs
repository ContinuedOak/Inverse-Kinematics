using StarterAssets;
using UnityEngine;

public class HeadIKEditor : MonoBehaviour
{
    // Attach this script to the player object

    [Header("Head IK Editor")]
    [Header("by ContinuedOak")]

    [Header("Main Config")]
    [Tooltip("Whether the IK can work")]
    public bool isIKOnHead;
    [Tooltip("If the useRay is true, then it will read the end point of the raycast from 'PlayerRaycast', if false, this can be used to set objects with scripts")]
    public bool useRay;
    [Tooltip("This is the Script Raycast, assign the object that has the raycast you want (usually your camera) for this to work")]
    public PlayerRaycast ray;
    [Tooltip("An array of Transforms the head should look at in sequence")]
    public Transform[] objectLookAtPositions;
    [Tooltip("The exact position the head should look at")]
    public Vector3 headLookAtPosition;
    [Space(1)]

    [Header("Walk Config")]
    [Tooltip("Default weight for the head IK")]
    [Range(0f, 1f)] public float headWeight = 1.0f;
    [Tooltip("Default weight for the body")]
    [Range(0f, 1f)] public float bodyWeight = 0.3f;
    [Tooltip("Weight for head when looking behind")]
    [Range(0f, 1f)] public float headWeightWhenLookingBehind = 0.7f;
    [Tooltip("Weight for body when looking behind")]
    [Range(0f, 1f)] public float bodyWeightWhenLookingBehind = 0.5f;
    [Tooltip("Weight for head when looking behind and down")]
    [Range(0f, 1f)] public float headWeightWhenLookingBehindDown = 0.6f;
    [Tooltip("Weight for body when looking behind and down")]
    [Range(0f, 1f)] public float bodyWeightWhenLookingBehindDown = 0.4f;
    [Tooltip("Weight for head when looking behind and up")]
    [Range(0f, 1f)] public float headWeightWhenLookingBehindUp = 0.8f;
    [Tooltip("Weight for body when looking behind and up")]
    [Range(0f, 1f)] public float bodyWeightWhenLookingBehindUp = 0.6f;
    [Space(1)]

    [Header("Sprint Config")]
    [Tooltip("Default weight for the head IK during sprint")]
    [Range(0f, 1f)] public float sprintHeadWeight = 0.8f;
    [Tooltip("Default weight for the body during sprint")]
    [Range(0f, 1f)] public float sprintBodyWeight = 0.3f;
    [Tooltip("Weight for head when sprinting and looking behind")]
    [Range(0f, 1f)] public float sprintHeadWeightWhenLookingBehind = 0.6f;
    [Tooltip("Weight for body when sprinting and looking behind")]
    [Range(0f, 1f)] public float sprintBodyWeightWhenLookingBehind = 0.4f;

    [Header("Smoothing Config")]
    [Tooltip("Speed of the transition")]
    public float transitionSpeed = 5.0f;
    [Tooltip("The minimum speed needed to switch")]
    [Range(0f, 1f)] public float speedRequired = 0.2f;

    #region Private's
    // Privates Must Be Assigned Upon Start
    private StarterAssetsInputs _input;
    private ThirdPersonController _player;
    private Animator anim;
    private float currentHeadWeight;
    private float currentBodyWeight;
    private Transform playerTransform;
    private GameObject _mainCamera;
    private Transform cameraTransform;
    #endregion

    #region Unity Functions
    private void Awake()
    {
        // get a reference to our main camera
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            cameraTransform = _mainCamera.transform;
        }
    }

    void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
        _player = GetComponent<ThirdPersonController>();
        anim = GetComponent<Animator>();

        currentHeadWeight = headWeight;
        currentBodyWeight = bodyWeight;

        playerTransform = this.transform;
    }

    void Update()
    {
        if (useRay)
            SetPlayerLookAt();
        else
        {
            if (objectLookAtPositions != null && objectLookAtPositions.Length > 0)
            {
                Transform closestObject = GetClosestObject();
                if (closestObject != null)
                {
                    UseObjectLookAt(closestObject);  // Set head to look at the closest object
                }
                else
                {
                    Debug.LogWarning("No valid closest object found.");
                }
            }
        }
    }
    #endregion

    #region OnAnimatorIK
    void OnAnimatorIK(int layerIndex)
    {
        if (anim && isIKOnHead)
        {
            float horizontalAngle = Vector3.Angle(playerTransform.forward, cameraTransform.forward);
            float verticalAngle = Vector3.Angle(Vector3.up, cameraTransform.forward);
            bool isLookingBehind = horizontalAngle > 90f;
            bool isLookingDown = verticalAngle > 135f; // Looking down if the angle is greater than 135 degrees
            bool isLookingUp = verticalAngle < 45f; // Looking up if the angle is less than 45 degrees

            if (_input.sprint)
                HeadLookAtRaycastSprint(isLookingBehind);
            else
                HeadLookAtRaycastWalk(isLookingBehind, isLookingDown, isLookingUp);

            // Smooth the transition
            anim.SetLookAtWeight(currentHeadWeight, currentBodyWeight);
            anim.SetLookAtPosition(headLookAtPosition);
        }
        else if (anim)
        {
            anim.SetLookAtWeight(0);
        }
    }
    #endregion
    #region HeadLookAtRaycastWalk

    private void HeadLookAtRaycastWalk(bool isLookingBehind, bool isLookingDown, bool isLookingUp)
    {
        if (_player.CurrentSpeed() >= speedRequired)
        {
            float targetHeadWeight = headWeight;
            float targetBodyWeight = bodyWeight;

            if (isLookingBehind && isLookingDown)
            {
                targetHeadWeight = headWeightWhenLookingBehindDown;
                targetBodyWeight = bodyWeightWhenLookingBehindDown;
            }
            else if (isLookingBehind && isLookingUp)
            {
                targetHeadWeight = headWeightWhenLookingBehindUp;
                targetBodyWeight = bodyWeightWhenLookingBehindUp;
            }
            else if (isLookingBehind)
            {
                targetHeadWeight = headWeightWhenLookingBehind;
                targetBodyWeight = bodyWeightWhenLookingBehind;
            }

            // Smooth the transition
            currentHeadWeight = Mathf.Lerp(currentHeadWeight, targetHeadWeight, Time.deltaTime * transitionSpeed);
            currentBodyWeight = Mathf.Lerp(currentBodyWeight, targetBodyWeight, Time.deltaTime * transitionSpeed);
        }
    }
    #endregion
    #region HeadLookAtRaycastSprint
    private void HeadLookAtRaycastSprint(bool isLookingBehind)
    {
        float targetHeadWeight = sprintHeadWeight;
        float targetBodyWeight = sprintBodyWeight;

        if (isLookingBehind)
        {
            targetHeadWeight = sprintHeadWeightWhenLookingBehind;
            targetBodyWeight = sprintBodyWeightWhenLookingBehind;
        }

        // Smooth the transition
        currentHeadWeight = Mathf.Lerp(currentHeadWeight, targetHeadWeight, Time.deltaTime * transitionSpeed);
        currentBodyWeight = Mathf.Lerp(currentBodyWeight, targetBodyWeight, Time.deltaTime * transitionSpeed);
    }
    #endregion

    private Transform GetClosestObject()
    {
        Transform closestObject = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform obj in objectLookAtPositions)
        {
            if (obj != null)
            {
                float distance = Vector3.Distance(playerTransform.position, obj.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestObject = obj;
                }
            }
        }

        return closestObject;
    }

    void UseObjectLookAt(Transform closestObject)
    {
        if (closestObject != null)
        {
            headLookAtPosition = closestObject.position;
        }
        else
        {
            Debug.LogWarning("Invalid closest object.");
        }
    }

    void SetPlayerLookAt()
    {
        if (ray != null)
            headLookAtPosition = ray.GetRaycastEndPoint();
        else
            Debug.LogError("ray is currently not assigned");
    }

    #region Public Functions
    public void IKStateSwitch()
    {
        isIKOnHead = !isIKOnHead;
    }
    #endregion
}