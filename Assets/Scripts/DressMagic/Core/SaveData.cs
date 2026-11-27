using System;
using System.Collections.Generic;
using UnityEngine;

namespace DressMagic.Core
{
    [Serializable]
    public class SaveData
    {
        public int version = 1;

        public List<string> unlocked = new List<string>();
        public List<string> outfit = new List<string>();
        public List<string> outfitColors = new List<string>();
        public List<string> discoveredCombos = new List<string>();

        public int doll = 0;
        public string scenario = "dulces";

        public float music = 0.55f;
        public float sfx = 0.80f;
        public float brightness = 1.0f;
        public int uiScale = 1;
        public bool sparkles = true;
        public bool animations = true;
        public bool seenIntro = false;
        public int photoCount = 0;

        public int totalUnlocked => unlocked.Count;
    }

    public static class SaveSystem
    {
        const string Key = "DressMagic.Save.v1";

        public static SaveData Load()
        {
            try
            {
                string json = PlayerPrefs.GetString(Key, string.Empty);
                if (string.IsNullOrEmpty(json)) return new SaveData();
                SaveData d = JsonUtility.FromJson<SaveData>(json);
                if (d == null) return new SaveData();
                if (d.unlocked == null) d.unlocked = new List<string>();
                if (d.outfit == null) d.outfit = new List<string>();
                if (d.outfitColors == null) d.outfitColors = new List<string>();
                if (d.discoveredCombos == null) d.discoveredCombos = new List<string>();
                return d;
            }
            catch (Exception e)
            {
                Debug.LogWarning("[DressMagic] No se pudo leer el guardado: " + e.Message);
                return new SaveData();
            }
        }

        public static void Save(SaveData d)
        {
            if (d == null) return;
            try
            {
                PlayerPrefs.SetString(Key, JsonUtility.ToJson(d));
                PlayerPrefs.Save();
            }
            catch (Exception e)
            {
                Debug.LogWarning("[DressMagic] No se pudo guardar: " + e.Message);
            }
        }

        public static void Wipe()
        {
            PlayerPrefs.DeleteKey(Key);
            PlayerPrefs.Save();
        }
    }
}
