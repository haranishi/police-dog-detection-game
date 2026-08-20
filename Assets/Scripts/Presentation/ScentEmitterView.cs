using PoliceDog.Domain;
using UnityEngine;

namespace PoliceDog.Presentation
{
    public sealed class ScentEmitterView : MonoBehaviour
    {
        [SerializeField] private string scentId = "target-1";
        [SerializeField] private string label = "訓練対象臭";
        [SerializeField] private ScentKind kind = ScentKind.Target;
        [SerializeField] private NoseHeight noseHeight = NoseHeight.Ground;
        [Range(0, 1)] [SerializeField] private float freshness = 1;
        [Min(0.1f)] [SerializeField] private float range = 6;

        private Transform scentVisual;

        public ScentKind Kind => kind;
        public string Label => label;

        public void Configure(
            string id,
            string displayLabel,
            ScentKind scentKind,
            NoseHeight height,
            float sourceFreshness,
            float sourceRange)
        {
            scentId = id;
            label = displayLabel;
            kind = scentKind;
            noseHeight = height;
            freshness = sourceFreshness;
            range = sourceRange;
        }

        public ScentSource ToDomain()
        {
            var position = transform.position;
            return new ScentSource(
                scentId,
                label,
                kind,
                new Point3(position.x, position.y, position.z),
                noseHeight,
                freshness,
                range);
        }

        public void SetFocused(bool focused, float intensity)
        {
            if (scentVisual == null) return;
            scentVisual.gameObject.SetActive(focused && intensity > 0.02f);
            if (!scentVisual.gameObject.activeSelf) return;

            var pulse = kind == ScentKind.Target
                ? 1 + Mathf.Sin(Time.time * 5f) * 0.08f
                : kind == ScentKind.Distractor
                    ? 1 + Mathf.Sin(Time.time * 2f) * 0.18f
                    : Mathf.PingPong(Time.time * 1.4f, 1) > 0.35f ? 1 : 0.15f;
            scentVisual.localScale = BaseScale(kind) * Mathf.Lerp(0.45f, 1.2f, intensity) * pulse;
        }

        private void Awake()
        {
            BuildVisualGrammar();
            SetFocused(false, 0);
        }

        private void BuildVisualGrammar()
        {
            var root = new GameObject("ScentGrammar").transform;
            root.SetParent(transform, false);
            root.localPosition = Vector3.up * 0.8f;
            scentVisual = root;

            switch (kind)
            {
                case ScentKind.Target:
                    for (var index = 0; index < 4; index++)
                    {
                        var segment = Primitive("Bundle " + index, PrimitiveType.Cube, root);
                        segment.transform.localPosition = Vector3.up * index * 0.42f;
                        segment.transform.localScale = new Vector3(0.12f, 0.32f, 0.12f);
                        Paint(segment, new Color(0.2f, 0.85f, 1f, 0.82f));
                    }
                    break;
                case ScentKind.Distractor:
                    for (var index = 0; index < 5; index++)
                    {
                        var bubble = Primitive("Bubble " + index, PrimitiveType.Sphere, root);
                        var angle = index / 5f * Mathf.PI * 2;
                        bubble.transform.localPosition = new Vector3(Mathf.Cos(angle), index * 0.12f, Mathf.Sin(angle)) * 0.48f;
                        bubble.transform.localScale = Vector3.one * 0.3f;
                        Paint(bubble, new Color(1f, 0.75f, 0.2f, 0.68f));
                    }
                    break;
                default:
                    for (var index = 0; index < 4; index++)
                    {
                        var dash = Primitive("Broken trail " + index, PrimitiveType.Cube, root);
                        dash.transform.localPosition = new Vector3(0, 0.06f, index * 0.45f);
                        dash.transform.localScale = new Vector3(0.16f, 0.06f, 0.24f);
                        Paint(dash, new Color(0.78f, 0.48f, 1f, 0.65f));
                    }
                    break;
            }
        }

        private static Vector3 BaseScale(ScentKind scentKind)
        {
            return scentKind == ScentKind.Target
                ? Vector3.one * 0.9f
                : scentKind == ScentKind.Distractor
                    ? Vector3.one * 1.15f
                    : Vector3.one * 0.85f;
        }

        private static GameObject Primitive(string name, PrimitiveType type, Transform parent)
        {
            var item = GameObject.CreatePrimitive(type);
            item.name = name;
            item.transform.SetParent(parent, false);
            Object.Destroy(item.GetComponent<Collider>());
            return item;
        }

        private static void Paint(GameObject item, Color color)
        {
            var material = new Material(Shader.Find("Sprites/Default"));
            material.color = color;
            item.GetComponent<Renderer>().material = material;
        }
    }
}
