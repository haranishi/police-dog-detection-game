using UnityEngine;

namespace PoliceDog.Presentation
{
    public sealed class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0, 3.2f, -6.2f);
        [SerializeField] private float smooth = 7f;

        public void Configure(Transform newTarget)
        {
            target = newTarget;
        }

        private void LateUpdate()
        {
            if (target == null) return;
            var desired = target.position + target.rotation * offset;
            transform.position = Vector3.Lerp(transform.position, desired, 1 - Mathf.Exp(-smooth * Time.deltaTime));
            transform.LookAt(target.position + Vector3.up * 0.65f);
        }
    }
}
