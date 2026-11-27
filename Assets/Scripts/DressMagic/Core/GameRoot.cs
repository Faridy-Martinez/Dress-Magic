using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif
using DressMagic.Art;
using DressMagic.Data;
using DressMagic.Screens;
using DressMagic.UI;

namespace DressMagic.Core
{
    public enum ScreenId
    {
        Menu,
        Settings,
        Scenarios,
        DressUp,
        Collectibles,
        Polaroid
    }

    public abstract class GameScreen : MonoBehaviour
    {
        public GameRoot Game;
        public RectTransform Root;

        public abstract void Build();

        public virtual void OnHide() { }
    }

    /// <summary>Núcleo del juego: estado, guardado, navegación entre pantallas y avisos.</summary>
    public sealed class GameRoot : MonoBehaviour
    {
        public static GameRoot Instance { get; private set; }

        public SaveData Save { get; private set; }
        public AudioService Audio { get; private set; }

        public readonly Dictionary<Slot, ItemDef> Equipped = new Dictionary<Slot, ItemDef>();
        public readonly Dictionary<string, int> ItemColors = new Dictionary<string, int>();

        public event Action<ItemDef> MagicUnlocked;

        Canvas canvas;
        CanvasScaler scaler;
        RectTransform screenHost;
        RectTransform overlayHost;
        Image brightnessLayer;
        Image fadeLayer;

        GameScreen current;
        ScreenId currentId;
        bool transitioning;

        public ScreenId CurrentScreen => currentId;
        public ScreenId ReturnTarget = ScreenId.Menu;

        // ------------------------------------------------------------- ciclo

        void Awake()
        {
            Instance = this;
            Save = SaveSystem.Load();
            BuildCanvas();

            Audio = gameObject.AddComponent<AudioService>();
            Audio.Init(Save);

            LoadOutfit();
            ApplyUiScale();
            ApplyBrightness();
        }

        void Start()
        {
            Go(ScreenId.Menu, false);
        }

        void BuildCanvas()
        {
            GameObject canvasGo = new GameObject("DressMagic Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGo.transform.SetParent(transform, false);
            canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = false;

            scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1600f, 900f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            RectTransform root = Ui.Node(canvas.transform, "Root");
            Ui.Stretch(root);

            Image backdrop = Ui.Img(root, "Backdrop", MakeSoftBackdrop(), Color.white);
            Ui.Stretch(backdrop.rectTransform);

            screenHost = Ui.Node(root, "ScreenHost");
            Ui.Stretch(screenHost);

            overlayHost = Ui.Node(root, "OverlayHost");
            Ui.Stretch(overlayHost);

            brightnessLayer = Ui.Img(root, "Brightness", null, new Color(0f, 0f, 0f, 0f));
            Ui.Stretch(brightnessLayer.rectTransform);

            fadeLayer = Ui.Img(root, "Fade", null, new Color(1f, 1f, 1f, 0f));
            Ui.Stretch(fadeLayer.rectTransform);

            EnsureEventSystem();
        }

        void EnsureEventSystem()
        {
            if (EventSystem.current != null) return;
            if (FindAnyObjectByType<EventSystem>() != null) return;

            GameObject es = new GameObject("EventSystem", typeof(EventSystem));
#if ENABLE_INPUT_SYSTEM
            InputSystemUIInputModule module = es.AddComponent<InputSystemUIInputModule>();
            try { module.AssignDefaultActions(); } catch { }
#else
            es.AddComponent<StandaloneInputModule>();
#endif
        }

        static Sprite MakeSoftBackdrop()
        {
            Canvas2D c = new Canvas2D(8, 256, 1);
            c.MultiGradient(Pal.Hex("F7EEFF"), Pal.Hex("E4D7F8"), Pal.Hex("D2C2F0"), 0.55f);
            return c.ToSprite(false);
        }

        // --------------------------------------------------------- navegación

        public void Go(ScreenId id, bool animate = true)
        {
            if (transitioning) return;
            StartCoroutine(Transition(id, animate));
        }

        IEnumerator Transition(ScreenId id, bool animate)
        {
            transitioning = true;
            bool fx = Save.animations;

            if (animate && fx && fadeLayer != null)
            {
                float t = 0f;
                while (t < 0.16f)
                {
                    t += Time.unscaledDeltaTime;
                    fadeLayer.color = new Color(1f, 1f, 1f, Mathf.Clamp01(t / 0.16f));
                    yield return null;
                }
                fadeLayer.color = new Color(1f, 1f, 1f, 1f);
            }

            if (current != null)
            {
                current.OnHide();
                Destroy(current.gameObject);
                current = null;
            }

            yield return null;

            currentId = id;
            current = Spawn(id);
            if (current != null) current.Build();

            if (animate && fx && fadeLayer != null)
            {
                float t = 0f;
                while (t < 0.24f)
                {
                    t += Time.unscaledDeltaTime;
                    fadeLayer.color = new Color(1f, 1f, 1f, 1f - Mathf.Clamp01(t / 0.24f));
                    yield return null;
                }
            }
            if (fadeLayer != null) fadeLayer.color = new Color(1f, 1f, 1f, 0f);

            transitioning = false;
        }

        GameScreen Spawn(ScreenId id)
        {
            GameObject go = new GameObject(id.ToString(), typeof(RectTransform));
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.SetParent(screenHost, false);
            Ui.Stretch(rt);

            GameScreen screen;
            switch (id)
            {
                case ScreenId.Menu: screen = go.AddComponent<MenuScreen>(); break;
                case ScreenId.Settings: screen = go.AddComponent<SettingsScreen>(); break;
                case ScreenId.Scenarios: screen = go.AddComponent<ScenarioScreen>(); break;
                case ScreenId.DressUp: screen = go.AddComponent<DressUpScreen>(); break;
                case ScreenId.Collectibles: screen = go.AddComponent<CollectiblesScreen>(); break;
                default: screen = go.AddComponent<PolaroidScreen>(); break;
            }

            screen.Game = this;
            screen.Root = rt;
            return screen;
        }

        // ------------------------------------------------------------ ajustes

        public void ApplyUiScale()
        {
            if (scaler == null) return;
            switch (Mathf.Clamp(Save.uiScale, 0, 2))
            {
                case 0: scaler.referenceResolution = new Vector2(1840f, 1035f); break;
                case 2: scaler.referenceResolution = new Vector2(1440f, 810f); break;
                default: scaler.referenceResolution = new Vector2(1600f, 900f); break;
            }
        }

        public void ApplyBrightness()
        {
            if (brightnessLayer == null) return;
            Save.brightness = Mathf.Clamp(Save.brightness, 0.35f, 1f);
            float dark = Mathf.Clamp01(1f - Save.brightness) * 0.88f;
            brightnessLayer.color = new Color(0f, 0f, 0f, dark);
        }

        public void SaveSettings()
        {
            SaveSystem.Save(Save);
        }

        public void ResetEverything()
        {
            Equipped.Clear();
            ItemColors.Clear();
            SpriteBank.UnpinAll();
            Save = new SaveData();
            SaveSystem.Save(Save);
            ApplyUiScale();
            ApplyBrightness();
            if (Audio != null) Audio.Apply(Save);
            Go(ScreenId.Menu, false);
        }

        public void Persist()
        {
            SaveOutfit();
            SaveSystem.Save(Save);
        }

        // ---------------------------------------------------------- armario

        public bool IsUnlocked(ItemDef def)
        {
            return def != null && (!def.magic || Save.unlocked.Contains(def.id));
        }

        public List<ItemDef> AvailableIn(string category)
        {
            List<ItemDef> all = ItemDatabase.InCategory(category);
            List<ItemDef> list = new List<ItemDef>();
            foreach (ItemDef d in all)
            {
                if (d.magic && !Save.unlocked.Contains(d.id)) continue;
                list.Add(d);
            }
            return list;
        }

        public int ColorOf(string itemId)
        {
            int idx;
            if (ItemColors.TryGetValue(itemId, out idx)) return idx;
            ItemDef def = ItemDatabase.Get(itemId);
            return def != null ? def.defaultColor : 0;
        }

        public void SetColor(string itemId, int colorIndex)
        {
            if (string.IsNullOrEmpty(itemId)) return;
            ItemColors[itemId] = Mathf.Clamp(colorIndex, 0, ItemDatabase.ColorValues.Length - 1);
            Persist();
        }

        public ItemDef EquippedIn(Slot slot)
        {
            ItemDef d;
            return Equipped.TryGetValue(slot, out d) ? d : null;
        }

        public bool ToggleItem(string id)
        {
            ItemDef def = ItemDatabase.Get(id);
            if (def == null) return false;
            if (def.magic && !Save.unlocked.Contains(id)) return false;

            ItemDef currentEquip = EquippedIn(def.slot);
            if (currentEquip != null && currentEquip.id == id)
            {
                Equipped.Remove(def.slot);
            }
            else
            {
                Equipped[def.slot] = def;
                if (!ItemColors.ContainsKey(id)) ItemColors[id] = def.defaultColor;
            }

            PinOutfit();
            if (Audio != null) Audio.Click();
            Persist();
            CheckMagic();
            return true;
        }

        public void ClearSlot(Slot slot)
        {
            if (!Equipped.ContainsKey(slot)) return;
            Equipped.Remove(slot);
            PinOutfit();
            Persist();
        }

        public void ClearOutfit()
        {
            Equipped.Clear();
            PinOutfit();
            Persist();
        }

        public HashSet<string> EquippedIds()
        {
            HashSet<string> set = new HashSet<string>();
            foreach (ItemDef d in Equipped.Values) set.Add(d.id);
            return set;
        }

        void LoadOutfit()
        {
            Equipped.Clear();
            ItemColors.Clear();

            foreach (string entry in Save.outfit)
            {
                string[] split = entry.Split('|');
                if (split.Length < 2) continue;
                int slotIndex;
                if (!int.TryParse(split[0], out slotIndex)) continue;
                ItemDef def = ItemDatabase.Get(split[1]);
                if (def == null) continue;
                if (def.magic && !Save.unlocked.Contains(def.id)) continue;
                Equipped[(Slot)slotIndex] = def;
            }

            foreach (string entry in Save.outfitColors)
            {
                string[] split = entry.Split('|');
                if (split.Length < 2) continue;
                int color;
                if (!int.TryParse(split[1], out color)) continue;
                ItemColors[split[0]] = color;
            }

            PinOutfit();
        }

        public void PinOutfit()
        {
            SpriteBank.UnpinAll();
            foreach (ItemDef d in Equipped.Values) SpriteBank.Pin(d.id);
        }

        public void SaveOutfit()
        {
            Save.outfit.Clear();
            foreach (KeyValuePair<Slot, ItemDef> kv in Equipped)
                Save.outfit.Add((int)kv.Key + "|" + kv.Value.id);

            Save.outfitColors.Clear();
            foreach (KeyValuePair<string, int> kv in ItemColors)
                Save.outfitColors.Add(kv.Key + "|" + kv.Value);
        }

        // ------------------------------------------------------- magia

        void CheckMagic()
        {
            MagicCombo combo = MagicCombos.Match(EquippedIds());
            if (combo == null) return;
            if (Save.discoveredCombos.Contains(combo.id)) return;

            Save.discoveredCombos.Add(combo.id);
            bool isNew = !Save.unlocked.Contains(combo.reward);
            if (isNew) Save.unlocked.Add(combo.reward);
            SaveSystem.Save(Save);

            ItemDef reward = ItemDatabase.Get(combo.reward);
            if (reward == null) return;

            if (Audio != null) Audio.Unlock();
            if (MagicUnlocked != null) MagicUnlocked(reward);
            ShowUnlock(reward, isNew);
        }

        // --------------------------------------------------------- avisos

        public void ShowUnlock(ItemDef item, bool isNew)
        {
            RectTransform scrim = Ui.Node(overlayHost, "UnlockScrim");
            Ui.Stretch(scrim);
            Image scrimImg = scrim.gameObject.AddComponent<Image>();
            scrimImg.color = new Color(0.28f, 0.13f, 0.26f, 0.55f);
            scrimImg.raycastTarget = true;
            FadeIn fader = scrim.gameObject.AddComponent<FadeIn>();
            fader.duration = 0.2f;
            fader.rise = 0f;

            RectTransform card = Ui.Node(scrim, "Card");
            Ui.Pin(card, 0.5f, 0.5f, 700f, 560f);
            Image cardBg = card.gameObject.AddComponent<Image>();
            cardBg.sprite = Ui.Round(40, Pal.Cream, Pal.A(Pal.Hex("8A5A8C"), 0.45f), 6);
            cardBg.type = Image.Type.Sliced;
            cardBg.color = Color.white;
            cardBg.raycastTarget = true;
            PopIn pop = card.gameObject.AddComponent<PopIn>();
            pop.duration = 0.35f;

            SparkleField sparkles = SparkleField.Create(card, 26, Pal.A(Pal.Butter, 0.95f), 1.1f);

            Text title = Ui.LabelWithShadow(card, "¡Has desbloqueado\nun nuevo objeto!", 44, Pal.Hex("C4437A"), Pal.A(Pal.White, 0.9f));
            Ui.PinAt(title.rectTransform, 0.5f, 1f, 0f, -84f, 620f, 130f);

            RectTransform disc = Ui.Node(card, "Disc");
            Ui.PinAt(disc, 0.5f, 0.5f, 0f, 26f, 230f, 230f);
            Image glow = Ui.Img(disc, "Glow", SpriteBank.Glow(), Pal.A(Pal.Butter, 0.55f));
            Ui.Stretch(glow.rectTransform);
            Image discImg = Ui.Img(disc, "DiscBg", SpriteBank.Ellipse("unlockdisc", 256, 256, Pal.White, Pal.A(Pal.Hex("E8A92E"), 0.85f), 6), Color.white);
            Ui.Stretch(discImg.rectTransform);
            Image icon = Ui.Img(disc, "Icon", SpriteBank.Icon(item.id, ColorOf(item.id)), Color.white);
            Ui.Stretch(icon.rectTransform, 22f, 22f, 22f, 22f);
            icon.preserveAspect = true;
            Pulse pulse = disc.gameObject.AddComponent<Pulse>();
            pulse.amount = 0.04f;

            Text name = Ui.LabelWithShadow(card, item.name, 34, Pal.Hex("6A3B63"), Pal.A(Pal.White, 0.8f));
            Ui.PinAt(name.rectTransform, 0.5f, 0f, 0f, 214f, 620f, 50f);

            Text sub = Ui.Label(card, isNew ? "Ya está en tu vestuario Especial" : "Vuelve a tus combinaciones mágicas", 22, Pal.TextSoft);
            Ui.PinAt(sub.rectTransform, 0.5f, 0f, 0f, 174f, 620f, 36f);

            Button ok = Ui.Pill(card, "¡Genial!", 220f, 78f, Pal.Pink, Pal.White, null, 32, 30f);
            Ui.PinAt(ok.GetComponent<RectTransform>(), 0.5f, 0f, -130f, 84f, 220f, 78f);
            ok.onClick.AddListener(() =>
            {
                if (Audio != null) Audio.Click();
                Destroy(scrim.gameObject);
            });

            Button see = Ui.Pill(card, "Ver colección", 260f, 78f, Pal.Mint, Pal.Hex("2E6B5C"), null, 30, 30f);
            Ui.PinAt(see.GetComponent<RectTransform>(), 0.5f, 0f, 150f, 84f, 260f, 78f);
            see.onClick.AddListener(() =>
            {
                if (Audio != null) Audio.Click();
                Destroy(scrim.gameObject);
                ReturnTarget = currentId;
                Go(ScreenId.Collectibles);
            });

            if (sparkles != null && !Save.sparkles) sparkles.SetActive(false);
        }

        public void ShowToast(string message, float seconds = 2f)
        {
            RectTransform holder = Ui.Node(overlayHost, "Toast");
            Ui.PinAt(holder, 0.5f, 0f, 0f, 130f, 700f, 84f);
            Image bg = holder.gameObject.AddComponent<Image>();
            bg.sprite = Ui.Round(30, Pal.A(Pal.Hex("4A2B48"), 0.9f), Color.clear);
            bg.type = Image.Type.Sliced;
            bg.raycastTarget = false;

            Text label = Ui.Label(holder, message, 26, Pal.White);
            Ui.Stretch(label.rectTransform, 26f, 0f, 26f, 0f);

            FadeIn fader = holder.gameObject.AddComponent<FadeIn>();
            fader.duration = 0.2f;
            fader.rise = 0f;
            StartCoroutine(ToastLife(holder, seconds));
        }

        IEnumerator ToastLife(RectTransform holder, float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
            if (holder == null) yield break;
            CanvasGroup g = Ui.Group(holder.gameObject);
            float t = 0f;
            while (t < 0.3f && holder != null)
            {
                t += Time.unscaledDeltaTime;
                g.alpha = 1f - Mathf.Clamp01(t / 0.3f);
                yield return null;
            }
            if (holder != null) Destroy(holder.gameObject);
        }

        // ------------------------------------------------------ utilidades

        public static string SpanishSlotName(Slot slot)
        {
            switch (slot)
            {
                case Slot.Back: return "Capa";
                case Slot.Legs: return "Pantalón";
                case Slot.Skirt: return "Falda";
                case Slot.Dress: return "Vestido";
                case Slot.Top: return "Blusa";
                case Slot.Feet: return "Calzado";
                case Slot.Neck: return "Broche";
                case Slot.Head: return "Cabeza";
                case Slot.Bag: return "Bolso";
                default: return "Varita";
            }
        }
    }

    /// <summary>Aparición con rebote.</summary>
    public sealed class PopIn : MonoBehaviour
    {
        public float duration = 0.35f;
        RectTransform rt;
        float t;

        void Awake()
        {
            rt = (RectTransform)transform;
        }

        void OnEnable()
        {
            t = 0f;
            if (rt != null) rt.localScale = Vector3.one * 0.6f;
        }

        void Update()
        {
            if (rt == null) return;
            if (t >= duration) return;
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / duration);
            float s = Mathf.Lerp(0.6f, 1f, 1f - Mathf.Pow(1f - k, 3f)) + Mathf.Sin(k * Mathf.PI) * 0.12f;
            rt.localScale = new Vector3(s, s, 1f);
        }
    }
}
