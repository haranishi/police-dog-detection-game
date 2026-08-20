using System.Collections;
using UnityEngine;

namespace PoliceDog.Presentation
{
    public static class PrototypeDogFactory
    {
        public static GameObject Create(Transform parent, Vector3 position)
        {
            var root = new GameObject("Prototype Dog");
            root.transform.SetParent(parent);
            root.transform.position = position;
            var characterController = root.AddComponent<CharacterController>();
            characterController.height = 1.15f;
            characterController.radius = 0.34f;
            characterController.center = new Vector3(0, 0.58f, 0);
            root.AddComponent<PrototypeDogController>();

            var visual = new GameObject("DogVisual").transform;
            visual.SetParent(root.transform, false);
            Paint(Primitive("Body", PrimitiveType.Capsule, visual, new Vector3(0, 0.55f, 0), new Vector3(0.55f, 0.5f, 1.05f)), new Color(0.36f, 0.23f, 0.12f));
            Paint(Primitive("Head", PrimitiveType.Sphere, visual, new Vector3(0, 0.76f, 0.75f), Vector3.one * 0.55f), new Color(0.42f, 0.27f, 0.14f));
            Paint(Primitive("Muzzle", PrimitiveType.Capsule, visual, new Vector3(0, 0.68f, 1.05f), new Vector3(0.25f, 0.22f, 0.38f)), new Color(0.17f, 0.12f, 0.08f));
            Paint(Primitive("Tail", PrimitiveType.Capsule, visual, new Vector3(0, 0.68f, -0.75f), new Vector3(0.18f, 0.45f, 0.18f), new Vector3(55, 0, 0)), new Color(0.36f, 0.23f, 0.12f));
            foreach (var x in new[] { -0.25f, 0.25f })
            {
                foreach (var z in new[] { -0.38f, 0.45f })
                {
                    Paint(Primitive("Leg", PrimitiveType.Capsule, visual, new Vector3(x, 0.25f, z), new Vector3(0.15f, 0.28f, 0.15f)), new Color(0.28f, 0.18f, 0.1f));
                }
            }
            Paint(Primitive("Harness", PrimitiveType.Cube, visual, new Vector3(0, 0.67f, -0.05f), new Vector3(0.6f, 0.12f, 0.45f)), new Color(0.08f, 0.32f, 0.55f));
            return root;
        }

        public static void PlayReward(Transform dog)
        {
            dog.gameObject.AddComponent<RewardWag>();
        }

        private static GameObject Primitive(
            string name,
            PrimitiveType type,
            Transform parent,
            Vector3 position,
            Vector3 scale,
            Vector3? euler = null)
        {
            var item = GameObject.CreatePrimitive(type);
            item.name = name;
            item.transform.SetParent(parent, false);
            item.transform.localPosition = position;
            item.transform.localScale = scale;
            item.transform.localEulerAngles = euler ?? Vector3.zero;
            Object.Destroy(item.GetComponent<Collider>());
            return item;
        }

        private static void Paint(GameObject item, Color color)
        {
            var material = new Material(Shader.Find("Standard"));
            material.color = color;
            item.GetComponent<Renderer>().material = material;
        }

        private sealed class RewardWag : MonoBehaviour
        {
            private float elapsed;

            private void Update()
            {
                elapsed += Time.deltaTime;
                var visual = transform.Find("DogVisual");
                if (visual != null)
                {
                    visual.localRotation = Quaternion.Euler(0, Mathf.Sin(elapsed * 16f) * 14f, 0);
                }
                if (elapsed > 2f) Destroy(this);
            }

            private void OnDestroy()
            {
                var visual = transform.Find("DogVisual");
                if (visual != null) visual.localRotation = Quaternion.identity;
            }
        }
    }
}
