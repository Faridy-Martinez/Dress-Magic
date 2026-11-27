using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DressMagic.Art;

namespace DressMagic.UI
{
    /// <summary>Campo de destellos animados en uGUI.</summary>
    public sealed class SparkleField : MonoBehaviour
    {
        struct Spark
        {
            public RectTransform rt;
            public Image img;
            public Vector2 norm;
            public Vector2 vel;
            public float phase;
            public float baseSize;
            public float rotSpeed;
            public float baseAlpha;
        }

        readonly List<Spark> sparks = new List<Spark>();
        RectTransform area;
        Color tint = Color.white;
        float speedScale = 1f;
        bool activeFx = true;

        public static SparkleField Create(RectTransform parent, int count, Color color, float speed = 1f)
        {
            RectTransform holder = Ui.Node(parent, "Sparkles");
            Ui.Stretch(holder);
            SparkleField f = holder.gameObject.AddComponent<SparkleField>();
            f.area = holder;
            f.tint = color;
            f.speedScale = speed;
            f.Spawn(count);
            return f;
        }

        void Spawn(int count)
        {
            Sprite sprite = SpriteBank.Star(4, 64, true);
            for (int i = 0; i < count; i++)
            {
                Image img = Ui.Img(transform, "spark", sprite, Color.white);
                img.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                img.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                img.rectTransform.pivot = new Vector2(0.5f, 0.5f);

                Spark s = new Spark
                {
                    rt = img.rectTransform,
                    img = img,
                    norm = new Vector2(Random.value, Random.value),
                    vel = new Vector2(Random.Range(-0.012f, 0.012f), Random.Range(0.03f, 0.12f)),
                    phase = Random.Range(0f, 10f),
                    baseSize = Random.Range(10f, 34f),
                    rotSpeed = Random.Range(-40f, 40f),
                    baseAlpha = Random.Range(0.35f, 0.95f),
                };
                sparks.Add(s);
            }
        }

        public void SetTint(Color c)
        {
            tint = c;
        }

        public void SetActive(bool on)
        {
            activeFx = on;
            if (area != null) area.gameObject.SetActive(on);
        }

        void Update()
        {
            if (!activeFx || area == null) return;
            float dt = Time.unscaledDeltaTime;
            Rect r = area.rect;
            if (r.width <= 1f || r.height <= 1f) return;

            for (int i = 0; i < sparks.Count; i++)
            {
                Spark s = sparks[i];

                s.norm += s.vel * (dt * speedScale);
                if (s.norm.y > 1.05f)
                {
                    s.norm.y = -0.05f;
                    s.norm.x = Random.value;
                }
                if (s.norm.x < -0.05f) s.norm.x = 1.05f;
                if (s.norm.x > 1.05f) s.norm.x = -0.05f;

                Vector2 pos = new Vector2((s.norm.x - 0.5f) * r.width, (s.norm.y - 0.5f) * r.height);
                s.rt.anchoredPosition = pos;

                float twinkle = 0.45f + 0.55f * Mathf.Abs(Mathf.Sin(Time.unscaledTime * 2.4f + s.phase));
                Color col = tint;
                col.a = s.baseAlpha * twinkle;
                s.img.color = col;
                s.rt.sizeDelta = Vector2.one * (s.baseSize * (0.7f + 0.4f * twinkle));
                s.rt.Rotate(0f, 0f, s.rotSpeed * dt);
            }
        }
    }

    /// <summary>Movimiento flotante suave.</summary>
    public sealed class FloatBob : MonoBehaviour
    {
        public float amplitude = 6f;
        public float speed = 1.2f;
        public float phase;
        RectTransform rt;
        Vector2 origin;

        void Awake()
        {
            rt = (RectTransform)transform;
            origin = rt.anchoredPosition;
        }

        void OnEnable()
        {
            if (rt != null) rt.anchoredPosition = origin;
        }

        void Update()
        {
            if (rt == null) return;
            float t = Time.unscaledTime * speed + phase;
            rt.anchoredPosition = origin + new Vector2(Mathf.Sin(t * 0.7f) * amplitude * 0.4f, Mathf.Sin(t) * amplitude);
        }
    }

    /// <summary>Latido suave para botones destacados.</summary>
    public sealed class Pulse : MonoBehaviour
    {
        public float amount = 0.035f;
        public float speed = 2.2f;
        RectTransform rt;
        Vector3 baseScale = Vector3.one;

        void Awake()
        {
            rt = (RectTransform)transform;
            baseScale = rt.localScale;
        }

        void Update()
        {
            if (rt == null) return;
            float s = 1f + Mathf.Sin(Time.unscaledTime * speed) * amount;
            rt.localScale = baseScale * s;
        }
    }

    /// <summary>Entrada con desvanecido + desplazamiento.</summary>
    public sealed class FadeIn : MonoBehaviour
    {
        public float duration = 0.35f;
        public float delay = 0f;
        public float rise = 24f;
        CanvasGroup group;
        Vector2 startPos;
        RectTransform rt;
        float t;
        bool done;

        void Awake()
        {
            group = Ui.Group(gameObject);
            rt = (RectTransform)transform;
            startPos = rt.anchoredPosition;
        }

        void OnEnable()
        {
            t = 0f;
            done = false;
            if (group != null) group.alpha = 0f;
        }

        void Update()
        {
            if (done) return;
            t += Time.unscaledDeltaTime;
            if (t < delay) return;
            float k = Mathf.Clamp01((t - delay) / Mathf.Max(0.01f, duration));
            float e = 1f - Mathf.Pow(1f - k, 3f);
            if (group != null) group.alpha = e;
            if (rt != null) rt.anchoredPosition = startPos + new Vector2(0f, (1f - e) * rise);
            if (k >= 1f) done = true;
        }
    }
}
