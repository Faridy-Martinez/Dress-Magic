using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DressMagic.Art;

namespace DressMagic.UI
{
    /// <summary>Fábrica de interfaz con el estilo pastel del juego.</summary>
    public static class Ui
    {
        static Font font;

        public static Font Font
        {
            get
            {
                if (font != null) return font;
                font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                return font;
            }
        }

        public static readonly Color Ink = Pal.Ink;
        public static readonly Color TextMain = Pal.Hex("5A3155");
        public static readonly Color TextSoft = Pal.Hex("8A6A8C");

        // --------------------------------------------------------------- nodes

        public static RectTransform Node(Transform parent, string name)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            rt.localScale = Vector3.one;
            return rt;
        }

        public static void Stretch(RectTransform rt, float l = 0f, float b = 0f, float r = 0f, float t = 0f)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(l, b);
            rt.offsetMax = new Vector2(-r, -t);
        }

        public static void Pin(RectTransform rt, float fx, float fy, float w, float h)
        {
            rt.anchorMin = new Vector2(fx, fy);
            rt.anchorMax = new Vector2(fx, fy);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(w, h);
        }

        public static void PinAt(RectTransform rt, float fx, float fy, float ox, float oy, float w, float h)
        {
            rt.anchorMin = new Vector2(fx, fy);
            rt.anchorMax = new Vector2(fx, fy);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(ox, oy);
            rt.sizeDelta = new Vector2(w, h);
        }

        public static Image Img(Transform parent, string name, Sprite sprite, Color color, bool raycast = false)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            rt.localScale = Vector3.one;
            Image img = go.GetComponent<Image>();
            img.sprite = sprite;
            img.color = color;
            img.raycastTarget = raycast;
            if (sprite != null && sprite.border != Vector4.zero) img.type = Image.Type.Sliced;
            return img;
        }

        public static Text Label(Transform parent, string text, int size, Color color,
            TextAnchor anchor = TextAnchor.MiddleCenter, bool bold = false)
        {
            GameObject go = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            rt.localScale = Vector3.one;
            Text t = go.GetComponent<Text>();
            t.font = Font;
            t.text = text;
            t.fontSize = size;
            t.color = color;
            t.alignment = anchor;
            t.fontStyle = bold ? FontStyle.Bold : FontStyle.Normal;
            t.horizontalOverflow = HorizontalWrapMode.Wrap;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            t.supportRichText = true;
            t.raycastTarget = false;
            return t;
        }

        public static Text LabelWithShadow(Transform parent, string text, int size, Color color,
            Color shadow, TextAnchor anchor = TextAnchor.MiddleCenter, bool bold = true)
        {
            Text t = Label(parent, text, size, color, anchor, bold);
            Shadow sh = t.gameObject.AddComponent<Shadow>();
            sh.effectColor = shadow;
            sh.effectDistance = new Vector2(0f, -3f);
            return t;
        }

        // ------------------------------------------------------------- sprites

        public static Sprite Round(int radius, Color fill, Color border, int borderWidth = 0)
        {
            string key = "r" + radius + "_" + ColorUtility.ToHtmlStringRGBA(fill) + "_" +
                         (borderWidth > 0 ? ColorUtility.ToHtmlStringRGBA(border) + borderWidth : "n");
            return SpriteBank.Nine(key, radius, fill, border, borderWidth);
        }

        // ------------------------------------------------------------- buttons

        public static Button Pill(Transform parent, string label, float w, float h,
            Color fill, Color labelColor, Action onClick, int fontSize = 28, float radius = 26f,
            Sprite icon = null)
        {
            RectTransform root = Node(parent, "Btn");
            root.sizeDelta = new Vector2(w, h);
            Button btn = root.gameObject.AddComponent<Button>();
            btn.transition = Selectable.Transition.ColorTint;

            Image shadow = Img(root, "Shadow", Round((int)radius, Pal.A(Pal.Hex("6B3A66"), 0.28f), Color.clear), Color.white);
            Stretch(shadow.rectTransform);
            shadow.rectTransform.offsetMin = new Vector2(0f, -7f);
            shadow.rectTransform.offsetMax = new Vector2(0f, -7f);

            Sprite bodySprite = Round((int)radius, fill, Pal.A(Pal.Hex("6B3A66"), 0.42f), 4);
            Image body = Img(root, "Body", bodySprite, Color.white, true);
            Stretch(body.rectTransform);
            btn.targetGraphic = body;

            ColorBlock cb = btn.colors;
            cb.normalColor = Color.white;
            cb.highlightedColor = new Color(1.06f, 1.06f, 1.06f, 1f);
            cb.pressedColor = new Color(0.88f, 0.88f, 0.88f, 1f);
            cb.selectedColor = Color.white;
            cb.disabledColor = new Color(0.7f, 0.7f, 0.7f, 0.6f);
            cb.fadeDuration = 0.08f;
            btn.colors = cb;

            Text text = LabelWithShadow(root, label, fontSize, labelColor, Pal.A(Pal.Hex("6B3A66"), 0.35f));
            Stretch(text.rectTransform, 16f, 0f, 16f, 0f);
            if (icon != null)
            {
                Image ic = Img(root, "Icon", icon, Color.white);
                PinAt(ic.rectTransform, 0f, 0.5f, h * 0.62f, 0f, h * 0.62f, h * 0.62f);
                ic.preserveAspect = true;
            }

            if (onClick != null) btn.onClick.AddListener(() => onClick());

            ClickPunch punch = root.gameObject.AddComponent<ClickPunch>();
            punch.target = root;
            return btn;
        }

        public static Button IconButton(Transform parent, Sprite icon, float size, Color fill, Action onClick)
        {
            RectTransform root = Node(parent, "IconBtn");
            root.sizeDelta = new Vector2(size, size);
            Button btn = root.gameObject.AddComponent<Button>();
            btn.transition = Selectable.Transition.ColorTint;

            Image shadow = Img(root, "Shadow", Round((int)(size * 0.32f), Pal.A(Pal.Hex("6B3A66"), 0.25f), Color.clear), Color.white);
            Stretch(shadow.rectTransform);
            shadow.rectTransform.offsetMin = new Vector2(0f, -5f);
            shadow.rectTransform.offsetMax = new Vector2(0f, -5f);

            Image body = Img(root, "Body", Round((int)(size * 0.32f), fill, Pal.A(Pal.Hex("6B3A66"), 0.4f), 4), Color.white, true);
            Stretch(body.rectTransform);
            btn.targetGraphic = body;

            Image ic = Img(root, "Icon", icon, Color.white);
            Stretch(ic.rectTransform, size * 0.22f, size * 0.22f, size * 0.22f, size * 0.22f);
            ic.preserveAspect = true;

            if (onClick != null) btn.onClick.AddListener(() => onClick());
            ClickPunch punch = root.gameObject.AddComponent<ClickPunch>();
            punch.target = root;
            return btn;
        }

        public static Button SpriteButton(Transform parent, Sprite sprite, float w, float h,
            Action onClick, Color bg, bool showBg = true)
        {
            RectTransform root = Node(parent, "SpriteBtn");
            root.sizeDelta = new Vector2(w, h);
            Button btn = root.gameObject.AddComponent<Button>();
            btn.transition = Selectable.Transition.ColorTint;

            if (showBg)
            {
                Image body = Img(root, "Body", Round(22, bg, Pal.A(Pal.Hex("6B3A66"), 0.34f), 3), Color.white, true);
                Stretch(body.rectTransform);
                btn.targetGraphic = body;
            }

            Image pic = Img(root, "Pic", sprite, Color.white, true);
            Stretch(pic.rectTransform, 6f, 6f, 6f, 6f);
            pic.preserveAspect = true;
            if (!showBg) btn.targetGraphic = pic;

            ColorBlock cb = btn.colors;
            cb.normalColor = Color.white;
            cb.highlightedColor = new Color(1.07f, 1.07f, 1.07f, 1f);
            cb.pressedColor = new Color(0.86f, 0.86f, 0.86f, 1f);
            cb.selectedColor = Color.white;
            cb.fadeDuration = 0.08f;
            btn.colors = cb;

            if (onClick != null) btn.onClick.AddListener(() => onClick());
            ClickPunch punch = root.gameObject.AddComponent<ClickPunch>();
            punch.target = root;
            return btn;
        }

        // ------------------------------------------------------------- sliders

        public static Slider Slider(Transform parent, float w, float h, float value,
            Color fillColor, Color trackColor, Action<float> onChange)
        {
            RectTransform root = Node(parent, "Slider");
            root.sizeDelta = new Vector2(w, h);
            UnityEngine.UI.Slider slider = root.gameObject.AddComponent<UnityEngine.UI.Slider>();
            slider.direction = UnityEngine.UI.Slider.Direction.LeftToRight;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.wholeNumbers = false;

            Image track = Img(root, "Track", Round((int)(h * 0.5f), trackColor, Pal.A(Pal.Hex("6B3A66"), 0.25f), 3), Color.white);
            Stretch(track.rectTransform, 0f, h * 0.28f, 0f, h * 0.28f);

            RectTransform fillArea = Node(root, "Fill Area");
            Stretch(fillArea, h * 0.5f, h * 0.28f, h * 0.5f, h * 0.28f);

            Image fill = Img(fillArea, "Fill", Round((int)(h * 0.5f), fillColor, Color.clear), Color.white);
            fill.rectTransform.anchorMin = Vector2.zero;
            fill.rectTransform.anchorMax = Vector2.one;
            fill.rectTransform.offsetMin = Vector2.zero;
            fill.rectTransform.offsetMax = Vector2.zero;
            slider.fillRect = fill.rectTransform;

            RectTransform handleArea = Node(root, "Handle Area");
            Stretch(handleArea, h * 0.5f, 0f, h * 0.5f, 0f);

            Image handle = Img(handleArea, "Handle", SpriteBank.Ellipse("handle", 48, 48, Pal.White, Pal.A(Pal.Hex("6B3A66"), 0.4f), 4), Color.white, true);
            handle.rectTransform.anchorMin = new Vector2(0f, 0f);
            handle.rectTransform.anchorMax = new Vector2(0f, 1f);
            handle.rectTransform.sizeDelta = new Vector2(h * 0.82f, h * 0.82f);
            slider.handleRect = handle.rectTransform;
            slider.targetGraphic = handle;

            slider.value = value;
            if (onChange != null) slider.onValueChanged.AddListener(v => onChange(v));
            return slider;
        }

        public static Slider SliderInt(Transform parent, float w, float h, int steps, int value,
            Color fillColor, Color trackColor, Action<int> onChange)
        {
            Slider s = Slider(parent, w, h, steps <= 1 ? 0f : value / (float)(steps - 1), fillColor, trackColor, null);
            s.wholeNumbers = false;
            s.onValueChanged.AddListener(v =>
            {
                int idx = Mathf.RoundToInt(v * (steps - 1));
                onChange?.Invoke(idx);
            });
            return s;
        }

        /// <summary>Interruptor tipo pastilla.</summary>
        public static Toggle Switch(Transform parent, float w, float h, bool value, Color onColor, Action<bool> onChange)
        {
            RectTransform root = Node(parent, "Switch");
            root.sizeDelta = new Vector2(w, h);
            Toggle toggle = root.gameObject.AddComponent<Toggle>();
            toggle.transition = Selectable.Transition.None;

            Image track = Img(root, "Track", Round((int)(h * 0.5f), value ? onColor : Pal.Hex("D8CBE6"), Pal.A(Pal.Hex("6B3A66"), 0.28f), 3), Color.white);
            Stretch(track.rectTransform);
            toggle.targetGraphic = track;

            Image knob = Img(root, "Knob", SpriteBank.Ellipse("knob", 64, 64, Pal.White, Pal.A(Pal.Hex("6B3A66"), 0.25f), 3), Color.white, true);
            RectTransform kr = knob.rectTransform;
            kr.sizeDelta = new Vector2(h * 0.8f, h * 0.8f);
            kr.anchorMin = new Vector2(0f, 0.5f);
            kr.anchorMax = new Vector2(0f, 0.5f);
            kr.anchoredPosition = new Vector2(value ? w - h * 0.5f : h * 0.5f, 0f);

            toggle.isOn = value;
            toggle.onValueChanged.AddListener(v =>
            {
                kr.anchoredPosition = new Vector2(v ? w - h * 0.5f : h * 0.5f, 0f);
                track.sprite = Round((int)(h * 0.5f), v ? onColor : Pal.Hex("D8CBE6"), Pal.A(Pal.Hex("6B3A66"), 0.28f), 3);
                track.color = Color.white;
                onChange?.Invoke(v);
            });
            return toggle;
        }

        // ---------------------------------------------------------------- misc

        public static CanvasGroup Group(GameObject go)
        {
            CanvasGroup g = go.GetComponent<CanvasGroup>();
            if (g == null) g = go.AddComponent<CanvasGroup>();
            return g;
        }

        public static void SetAlpha(GameObject go, float a)
        {
            Group(go).alpha = a;
        }
    }

    /// <summary>Animación de escala al pulsar.</summary>
    public sealed class ClickPunch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public RectTransform target;
        float scale = 1f;
        float goal = 1f;
        bool hover;

        void OnEnable()
        {
            scale = 1f;
            goal = 1f;
            if (target != null) target.localScale = Vector3.one;
        }

        public void OnPointerDown(PointerEventData e)
        {
            goal = 0.93f;
        }

        public void OnPointerUp(PointerEventData e)
        {
            goal = hover ? 1.04f : 1f;
        }

        public void OnPointerEnter(PointerEventData e)
        {
            hover = true;
            goal = 1.04f;
        }

        public void OnPointerExit(PointerEventData e)
        {
            hover = false;
            goal = 1f;
        }

        void Update()
        {
            if (target == null) return;
            scale = Mathf.Lerp(scale, goal, 1f - Mathf.Exp(-18f * Time.unscaledDeltaTime));
            target.localScale = new Vector3(scale, scale, 1f);
        }
    }
}
