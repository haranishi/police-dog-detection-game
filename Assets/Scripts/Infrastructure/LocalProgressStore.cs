using System;
using System.IO;
using UnityEngine;

namespace PoliceDog.Infrastructure
{
    [Serializable]
    public sealed class ProgressData
    {
        public int schemaVersion = 1;
        public int trust;
        public int completions;
    }

    public sealed class LocalProgressStore
    {
        private readonly string path;

        public LocalProgressStore()
        {
            // PoliceDog.Application 名前空間が裸の Application を隠すため完全修飾する
            path = Path.Combine(UnityEngine.Application.persistentDataPath, "progress.json");
        }

        public ProgressData Load()
        {
            if (!File.Exists(path)) return new ProgressData();
            try
            {
                var data = JsonUtility.FromJson<ProgressData>(File.ReadAllText(path));
                return data != null && data.schemaVersion == 1 ? data : new ProgressData();
            }
            catch
            {
                return new ProgressData();
            }
        }

        public void Save(ProgressData data)
        {
            var temporaryPath = path + ".tmp";
            File.WriteAllText(temporaryPath, JsonUtility.ToJson(data, true));
            if (File.Exists(path)) File.Delete(path);
            File.Move(temporaryPath, path);
        }
    }
}
