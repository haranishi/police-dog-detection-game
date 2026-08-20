using System.Collections.Generic;
using PoliceDog.Application;
using PoliceDog.Domain;
using PoliceDog.Presentation;
using UnityEngine;

namespace PoliceDog.Delivery
{
    public sealed class PrototypeBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            // PoliceDog.Application 名前空間が裸の Application を隠すため完全修飾する
            UnityEngine.Application.targetFrameRate = 60;
            BuildLighting();
            BuildLobby();

            var world = new GameObject("Prototype World").transform;
            var dogObject = PrototypeDogFactory.Create(world, new Vector3(0, 0.05f, -6f));
            var dog = dogObject.GetComponent<PrototypeDogController>();
            var emitters = BuildScentSources(world);

            var camera = Camera.main;
            if (camera == null)
            {
                camera = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener)).GetComponent<Camera>();
                camera.tag = "MainCamera";
            }
            camera.backgroundColor = new Color(0.08f, 0.12f, 0.16f);
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.gameObject.AddComponent<ThirdPersonCamera>().Configure(dog.transform);

            var hud = new GameObject("Prototype HUD", typeof(PrototypeHud)).GetComponent<PrototypeHud>();
            var caseController = new GameObject("Case Controller", typeof(PrototypeCaseController)).GetComponent<PrototypeCaseController>();
            caseController.Configure(dog, hud, emitters);
        }

        private static void BuildLighting()
        {
            RenderSettings.ambientLight = new Color(0.45f, 0.48f, 0.52f);
            var light = new GameObject("Main Light", typeof(Light)).GetComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            light.transform.rotation = Quaternion.Euler(50, -35, 0);
        }

        private static void BuildLobby()
        {
            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Arrival Lobby Floor";
            floor.transform.localScale = new Vector3(2.2f, 1, 1.7f);
            Paint(floor, new Color(0.25f, 0.31f, 0.34f));

            for (var index = -2; index <= 2; index++)
            {
                var barrier = GameObject.CreatePrimitive(PrimitiveType.Cube);
                barrier.name = "Queue Barrier";
                barrier.transform.position = new Vector3(index * 2.2f, 0.5f, 1.6f);
                barrier.transform.localScale = new Vector3(0.12f, 1f, 3.4f);
                Paint(barrier, new Color(0.12f, 0.16f, 0.2f));
            }

            for (var index = 0; index < 7; index++)
            {
                var passenger = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                passenger.name = "Neutral Passenger " + (index + 1);
                passenger.transform.position = new Vector3(-6f + index * 2f, 1f, 5.4f + (index % 2) * 1.3f);
                Paint(passenger, Color.HSVToRGB(index / 8f, 0.25f, 0.72f));
            }
        }

        private static List<ScentEmitterView> BuildScentSources(Transform parent)
        {
            var emitters = new List<ScentEmitterView>
            {
                CreateScent(parent, "target-a", "訓練対象臭A", ScentKind.Target, NoseHeight.Ground, new Vector3(5.6f, 0, 5.1f), 1f, 6f, "Target Luggage"),
                CreateScent(parent, "distractor-food", "焼き菓子の香り", ScentKind.Distractor, NoseHeight.Air, new Vector3(-5f, 0, 2.8f), 1f, 5.5f, "Food Bag"),
                CreateScent(parent, "distractor-clean", "洗いたての布の香り", ScentKind.Distractor, NoseHeight.Ground, new Vector3(0.5f, 0, 6.4f), 0.9f, 5f, "Clean Luggage"),
                CreateScent(parent, "residual-a", "古い対象臭の痕跡", ScentKind.Residual, NoseHeight.Ground, new Vector3(4.8f, 0, -0.4f), 0.55f, 5f, "Residual Trail")
            };
            return emitters;
        }

        private static ScentEmitterView CreateScent(
            Transform parent,
            string id,
            string label,
            ScentKind kind,
            NoseHeight height,
            Vector3 position,
            float freshness,
            float range,
            string objectName)
        {
            var source = GameObject.CreatePrimitive(kind == ScentKind.Residual ? PrimitiveType.Cylinder : PrimitiveType.Cube);
            source.name = objectName;
            source.SetActive(false);
            source.transform.SetParent(parent);
            source.transform.position = position + Vector3.up * 0.45f;
            source.transform.localScale = kind == ScentKind.Residual
                ? new Vector3(0.5f, 0.08f, 0.5f)
                : new Vector3(1.1f, 0.9f, 0.75f);
            Paint(source, kind == ScentKind.Target ? new Color(0.2f, 0.36f, 0.48f) : new Color(0.38f, 0.38f, 0.4f));
            var emitter = source.AddComponent<ScentEmitterView>();
            emitter.Configure(id, label, kind, height, freshness, range);
            source.SetActive(true);
            return emitter;
        }

        private static void Paint(GameObject item, Color color)
        {
            var material = new Material(Shader.Find("Standard"));
            material.color = color;
            item.GetComponent<Renderer>().material = material;
        }
    }
}
