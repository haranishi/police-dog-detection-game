using System;
using UnityEngine;

namespace PoliceDog.Presentation
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PrototypeDogController : MonoBehaviour
    {
        public event Action<bool> FocusChanged;
        public event Action AlertRequested;
        public event Action<bool> NoseHeightChanged;

        [SerializeField] private float walkSpeed = 3.2f;
        [SerializeField] private float runSpeed = 5.4f;
        [SerializeField] private float turnSpeed = 12f;

        private CharacterController controller;
        private Transform visualRoot;
        private bool isSitting;
        private bool wasAlertPressed;
        private bool isFocused;
        private bool noseHigh;
        private float stride;

        public bool IsFocused => isFocused;
        public bool NoseHigh => noseHigh;
        public bool IsSitting => isSitting;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            visualRoot = transform.Find("DogVisual");
        }

        private void Start()
        {
            if (visualRoot == null) visualRoot = transform.Find("DogVisual");
        }

        private void Update()
        {
            var horizontal = Input.GetAxisRaw("Horizontal");
            var vertical = Input.GetAxisRaw("Vertical");
            var movement = new Vector3(horizontal, 0, vertical);
            if (movement.sqrMagnitude > 1) movement.Normalize();

            var focus = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || Input.GetButton("Fire2");
            if (focus != isFocused)
            {
                isFocused = focus;
                FocusChanged?.Invoke(focus);
            }

            if (Input.GetKeyDown(KeyCode.R) || Input.GetButtonDown("Fire3"))
            {
                noseHigh = !noseHigh;
                NoseHeightChanged?.Invoke(noseHigh);
            }

            var alertPressed = Input.GetKey(KeyCode.Space) || Input.GetButton("Fire1");
            if (alertPressed && !wasAlertPressed)
            {
                isSitting = true;
                AlertRequested?.Invoke();
            }
            wasAlertPressed = alertPressed;

            if (movement.sqrMagnitude > 0.01f)
            {
                isSitting = false;
                var targetRotation = Quaternion.LookRotation(movement, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
                var speed = focus ? walkSpeed * 0.55f : Input.GetKey(KeyCode.LeftControl) ? runSpeed : walkSpeed;
                controller.Move(movement * speed * Time.deltaTime + Physics.gravity * Time.deltaTime);
                stride += Time.deltaTime * speed * 4f;
                if (visualRoot != null)
                {
                    visualRoot.localPosition = Vector3.up * (0.02f + Mathf.Abs(Mathf.Sin(stride)) * 0.05f);
                    visualRoot.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(stride) * 2.5f);
                }
            }
            else if (visualRoot != null)
            {
                visualRoot.localPosition = isSitting ? Vector3.down * 0.18f : Vector3.zero;
                visualRoot.localRotation = Quaternion.identity;
            }
        }
    }
}
