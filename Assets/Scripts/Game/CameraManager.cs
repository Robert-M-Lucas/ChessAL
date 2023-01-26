using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Game.ThreeD
{
    /// <summary>
    /// Controls the 3D chess camera
    /// </summary>
    public class CameraManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float perspectiveSwitchTime = 1f;
        [HideInInspector, DoNotSerialize] public float maxDist = 15;
        [HideInInspector, DoNotSerialize] public float minDist = 10;
        [SerializeField] private float scrollSensitivity = 1;
        [SerializeField] private float normalFov = 60;
        [SerializeField] private float fov2D = 2;
        [SerializeField] private float orthographicWidth = 11;
        [SerializeField] private Slider mouseSensitivity;
        [SerializeField] private LayerMask square3DMask;

        [Header("References")]
        [SerializeField] private Transform rotationalPivot;
        [SerializeField] private Transform upDownPivot;
        public Transform cameraPosition;
        [SerializeField] private Camera _camera;

        private Vector2 camera_rotation;

        void Start()
        {
            // Set rotation values to current rotation
            camera_rotation.y = rotationalPivot.localRotation.eulerAngles.y;
            camera_rotation.x = upDownPivot.localRotation.eulerAngles.x;
        }

        /// <summary>
        /// Rotates the camera given a Vector 2 mouse movement input
        /// </summary>
        /// <param name="look"></param>
        public void RotateCamera(Vector2 look)
        {
            camera_rotation.x -= look.y;
            camera_rotation.y += look.x;

            camera_rotation.x = Mathf.Clamp(camera_rotation.x, 0, 90);

            rotationalPivot.transform.localRotation = Quaternion.Euler(0, camera_rotation.y, 0);
            upDownPivot.transform.localRotation = Quaternion.Euler(camera_rotation.x, 0, 0);
        }

        /*
        public void Transition(float progress)
        {
            Vector3 movingFrom = transform.parent.position;
            Quaternion rotatingFrom = transform.parent.rotation;

            if (progress < 0.7f)
            {
                float smooth_progress = MathP.CosSmooth(progress * (1 / 0.7f));

                transform.position = Vector3.Lerp(movingFrom, Vector3.up * (orthographicWidth / (2 * Mathf.Tan(0.5f * normalFov * Mathf.Deg2Rad))), smooth_progress);
                transform.rotation = Quaternion.Lerp(rotatingFrom, Quaternion.Euler(90, 0, 180), smooth_progress);
                _camera.fieldOfView = normalFov;
            }
            else
            {
                float smooth_progress = Mathf.Clamp(MathP.CosSmooth((progress - 0.7f) * (1 / 0.3f)), 0.000000001f, 1);

                _camera.fieldOfView = normalFov - (smooth_progress * (normalFov - fov2D));

                transform.rotation = Quaternion.Euler(90, 0, 180);

                transform.position = Vector3.up * (orthographicWidth / (2 * Mathf.Tan(0.5f * _camera.fieldOfView * Mathf.Deg2Rad)));
            }
        }
        */

        /// <summary>
        /// Handles what happens when a mouse raycast hits a square
        /// </summary>
        /// <param name="boardSize"></param>
        /// <returns></returns>
        private V2? HandleHit(int boardSize)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, square3DMask))
            {
                Vector3 position = hit.collider.gameObject.transform.position;
                V2 intPosition = new V2((int)(position.x + (boardSize / 2f)), (int)(position.z + (boardSize / 2f)));
                return intPosition;
            }
            else
                return HandleNoHit();
        }

        /// <summary>
        /// Handles what happens when a mouse raycast doesn't hit anything
        /// </summary>
        /// <returns></returns>
        private V2? HandleNoHit()
        {
            return null;
        }


        /// <summary>
        /// Updates the camera control script when needed
        /// </summary>
        /// <param name="boardSize"></param>
        /// <returns>Square mouse is hovering over</returns>
        public V2? ExternalUpdate(int boardSize)
        {
            cameraPosition.localPosition = new Vector3(0, 0, -Mathf.Clamp((-cameraPosition.localPosition.z) + (-Input.mouseScrollDelta.y * scrollSensitivity), minDist, maxDist));

            float mouse_x = Input.GetAxis("Mouse X");
            float mouse_y = Input.GetAxis("Mouse Y");

            if (I.GetMouseButton(K.SecondaryClick))
            {
                RotateCamera(new Vector2(mouse_x * (mouseSensitivity.value + 0.05f) * 10f, mouse_y * (mouseSensitivity.value + 0.05f) * 10f));
                return HandleNoHit();
            }
            else
            {
                return HandleHit(boardSize);
            }
        }
    }
}