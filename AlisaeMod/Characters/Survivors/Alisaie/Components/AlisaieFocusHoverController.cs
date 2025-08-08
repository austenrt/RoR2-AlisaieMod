using UnityEngine;
using RoR2;

namespace AlisaieMod.Characters.Survivors.Alisaie.Components
{
    /**
     * Class allows Alisaie's spellcasting focus to follow her hand during gameplay without animating it manually.
     */
    public class AlisaieFocusHoverController : MonoBehaviour
    {
        public string handBoneName = "j_te_l"; // Name of the hand bone
        public string focusName = "Focus";  // Name of the focus in childLocator

        public Vector3 offset = new Vector3(0f, 0.35f, 0f);

        [Header("Position Smoothing")]
        public float smoothTime = 0.15f;
        private Vector3 velocity = Vector3.zero;

        [Header("Yaw Rotation Smoothing")]
        public float yawFollowSpeed = 5f;

        private Transform target;          // The hand bone
        private Transform focusObject;
        private Transform rotationSource;  // The body for yaw alignment

        void Start()
        {
            ChildLocator childLocator = GetComponent<ChildLocator>();

            target = childLocator.FindChild(handBoneName);
            focusObject = childLocator.FindChild(focusName);
            rotationSource = transform;

        }

        void LateUpdate()
        {

            // Smooth position follow
            Vector3 targetPosition = target.position + offset;
            focusObject.position = Vector3.SmoothDamp(focusObject.position, targetPosition, ref velocity, smoothTime);

            // Smooth Y rotation follow
            float currentYaw = focusObject.eulerAngles.y;
            float targetYaw = rotationSource.eulerAngles.y;
            float smoothYaw = Mathf.LerpAngle(currentYaw, targetYaw, Time.deltaTime * yawFollowSpeed);

            // Apply only Y-axis rotation to keep it upright
            focusObject.rotation = Quaternion.Euler(0f, smoothYaw, 0f);
        }
    }
}
