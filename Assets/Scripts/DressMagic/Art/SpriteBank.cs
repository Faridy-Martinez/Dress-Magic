using System.Collections.Generic;
using UnityEngine;
using DressMagic.Data;

namespace DressMagic.Art
{
    /// <summary>Genera y guarda en caché todos los sprites del juego.</summary>
    public static class SpriteBank
    {
        const int MaxWearables = 34;
        const int MaxIcons = 220;

        static readonly Dictionary<string, Sprite> wearables = new Dictionary<string, Sprite>();
        static readonly List<string> wearOrder = new List<string>();
        static readonly HashSet<string> pinned = new HashSet<string>();

        static readonly Dictionary<string, Sprite> icons = new Dictionary<string, Sprite>();
        static readonly List<string> iconOrder = new List<string>();

        static readonly Dictionary<int, Sprite> dolls = new Dictionary<int, Sprite>();
        static readonly Dictionary<int, Sprite> backgrounds = new Dictionary<int, Sprite>();
        static readonly Dictionary<string, Sprite> misc = new Dictionary<string, Sprite>();

        public static void ClearAll()
        {
            foreach (Sprite s in wearables.Values) Destroy(s);
            foreach (Sprite s in icons.Values) Destroy(s);
            foreach (Sprite s in dolls.Values) Destroy(s);
            foreach (Sprite s in backgrounds.Values) Destroy(s);
            wearables.Clear();
            icons.Clear();
            dolls.Clear();
            backgrounds.Clear();
            wearOrder.Clear();
            iconOrder.Clear();
        }

        static void Destroy(Sprite s)
        {
            if (s == null) return;
            Texture2D t = s.texture;
            Object.Destroy(s);
            if (t != null) Object.Destroy(t);
        }

        // ------------------------------------------------------------- dolls

        public static Sprite Doll(int index)
        {
            Sprite s;
            if (dolls.TryGetValue(index, out s) && s != null) return s;
            Canvas2D c = new Canvas2D(Rig.W, Rig.H, 2);
            DollArt.Draw(c, index);
            s = c.ToSprite(true);
            s.name = "doll" + index;
            dolls[index] = s;
            return s;
        }

        public static Sprite DollFace(int index)
        {
            string key = "face" + index;
            Sprite s;
            if (misc.TryGetValue(key, out s) && s != null) return s;

            Canvas2D src = new Canvas2D(Rig.W, Rig.H, 1);
            DollArt.Draw(src, index);
            Color32[] px = src.Resolve();
            Canvas2D.BleedAlpha(px, Rig.W, Rig.H, 2);

            const int Size = 220;
            int x0 = 106, y0 = 686, x1 = 406, y1 = 1024;
            float bw = x1 - x0;
            float bh = Mathf.Min(y1, Rig.H) - y0;
            float scale = Mathf.Min((Size - 12f) / bw, (Size - 12f) / bh);

            Canvas2D dst = new Canvas2D(Size, Size, 2);
            dst.BlitRect(px, Rig.W, Rig.H, x0 / (float)Rig.W, y0 / (float)Rig.H, x1 / (float)Rig.W, (y0 + bh) / Rig.H,
                new Vector2(Size * 0.5f, Size * 0.5f), scale);

            Color32[] outPx = dst.Resolve();
            Canvas2D.BleedAlpha(outPx, Size, Size, 1);
            Texture2D tex = Canvas2D.MakeTexture(outPx, Size, Size, true);
            s = Sprite.Create(tex, new Rect(0, 0, Size, Size), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
            s.name = key;
            misc[key] = s;
            return s;
        }

        // -------------------------------------------------------- wearables
        public static void Pin(string itemId)
        {
            pinned.Add(itemId);
        }

        public static void Unpin(string itemId)
        {
            pinned.Remove(itemId);
        }

        public static void UnpinAll()
        {
            pinned.Clear();
        }

        public static Sprite Wearable(string id, int colorIndex)
        {
            string key = id + "|" + colorIndex;
            Sprite s;
            if (wearables.TryGetValue(key, out s) && s != null) return s;

            Canvas2D c = new Canvas2D(Rig.W, Rig.H, 2);
            WearableArt.Draw(c, id, ItemDatabase.ColorValues[Mathf.Clamp(colorIndex, 0, ItemDatabase.ColorValues.Length - 1)]);
            s = c.ToSprite(true);
            s.name = key;
            wearables[key] = s;
            wearOrder.Add(key);
            TrimWearables();
            return s;
        }

        static void TrimWearables()
        {
            if (wearables.Count <= MaxWearables) return;
            for (int i = 0; i < wearOrder.Count && wearables.Count > MaxWearables; i++)
            {
                string k = wearOrder[i];
                string itemId = k.Substring(0, k.IndexOf('|'));
                if (pinned.Contains(itemId)) continue;
                if (wearables.ContainsKey(k))
                {
                    Destroy(wearables[k]);
                    wearables.Remove(k);
                }
                wearOrder.RemoveAt(i);
                i--;
            }
        }

        // ------------------------------------------------------------ icons

        public static Sprite Icon(string id, int colorIndex)
        {
            string key = id + "|" + colorIndex;
            Sprite s;
            if (icons.TryGetValue(key, out s) && s != null) return s;

            s = BuildIcon(id, ItemDatabase.ColorValues[Mathf.Clamp(colorIndex, 0, ItemDatabase.ColorValues.Length - 1)]);
            s.name = "icon_" + key;
            icons[key] = s;
            iconOrder.Add(key);
            if (icons.Count > MaxIcons)
            {
                string oldest = iconOrder[0];
                iconOrder.RemoveAt(0);
                if (icons.ContainsKey(oldest))
                {
                    Destroy(icons[oldest]);
                    icons.Remove(oldest);
                }
            }
            return s;
        }

        static Sprite BuildIcon(string id, Color col)
        {
            const int Size = 220;
            const int Pad = 16;

            Canvas2D src = new Canvas2D(Rig.W, Rig.H, 1);
            WearableArt.Draw(src, id, col);
            Color32[] px = src.Resolve();

            int minX = Rig.W, minY = Rig.H, maxX = -1, maxY = -1;
            for (int y = 0; y < Rig.H; y++)
            {
                int row = y * Rig.W;
                for (int x = 0; x < Rig.W; x++)
                {
                    if (px[row + x].a <= 10) continue;
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }
            }
            if (maxX < minX)
            {
                return MakeFlatSprite(Size, Pal.PanelDeep);
            }

            int bw = maxX - minX + 1;
            int bh = maxY - minY + 1;
            float scale = Mathf.Min((Size - Pad * 2f) / bw, (Size - Pad * 2f) / bh);
            scale = Mathf.Min(scale, 2.4f);

            Canvas2D dst = new Canvas2D(Size, Size, 2);
            Color32[] filled = new Color32[px.Length];
            System.Array.Copy(px, filled, px.Length);
            Canvas2D.BleedAlpha(filled, Rig.W, Rig.H, 2);
            dst.BlitRect(filled, Rig.W, Rig.H,
                minX / (float)Rig.W, minY / (float)Rig.H, (maxX + 1) / (float)Rig.W, (maxY + 1) / (float)Rig.H,
                new Vector2(Size * 0.5f, Size * 0.5f), scale);

            Color32[] outPx = dst.Resolve();
            Canvas2D.BleedAlpha(outPx, Size, Size, 1);
            Texture2D tex = Canvas2D.MakeTexture(outPx, Size, Size, true);
            return Sprite.Create(tex, new Rect(0, 0, Size, Size), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
        }

        // ------------------------------------------------------- ui sprites

        static Sprite MakeFlatSprite(int size, Color col)
        {
            Canvas2D c = new Canvas2D(size, size, 1);
            c.FillAll(col);
            return c.ToSprite(true);
        }

        public static Sprite Nine(string key, int radius, Color fill, Color border, int borderWidth)
        {
            Sprite s;
            if (misc.TryGetValue(key, out s) && s != null) return s;

            int pad = 3;
            int size = radius * 2 + pad * 2 + 4;
            Canvas2D c = new Canvas2D(size, size, 2);
            if (borderWidth > 0) c.RoundRect(pad, pad, size - pad, size - pad, radius, border);
            int inset = pad + borderWidth;
            c.RoundRect(inset, inset, size - inset, size - inset, Mathf.Max(2, radius - borderWidth), fill);

            Texture2D tex = c.ToTexture(true);
            Vector4 b = new Vector4(radius + pad, radius + pad, radius + pad, radius + pad);
            s = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, b);
            s.name = "nine_" + key;
            misc[key] = s;
            return s;
        }

        public static Sprite Ellipse(string key, int w, int h, Color fill, Color border, int borderWidth, float softness = 0f)
        {
            Sprite s;
            if (misc.TryGetValue(key, out s) && s != null) return s;

            Canvas2D c = new Canvas2D(w, h, 2);
            float cx = w * 0.5f, cy = h * 0.5f;
            float rx = w * 0.5f - borderWidth - 1f, ry = h * 0.5f - borderWidth - 1f;
            if (softness > 0f)
            {
                c.Glow(cx, cy, Mathf.Max(rx, ry) * 1.1f, fill, softness);
            }
            else
            {
                if (borderWidth > 0) c.Ellipse(cx, cy, w * 0.5f - 1f, h * 0.5f - 1f, border);
                c.Ellipse(cx, cy, rx, ry, fill);
            }

            Texture2D tex = c.ToTexture(true);
            s = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
            s.name = "ell_" + key;
            misc[key] = s;
            return s;
        }

        public static Sprite Star(int points = 5, int size = 96, bool sparkle = false)
        {
            string key = sparkle ? "sparkle" + points + "_" + size : "star" + points + "_" + size;
            Sprite s;
            if (misc.TryGetValue(key, out s) && s != null) return s;

            Canvas2D c = new Canvas2D(size, size, 3);
            float cx = size * 0.5f, cy = size * 0.5f;
            float r = size * 0.5f - 2f;
            if (sparkle)
            {
                c.Glow(cx, cy, r * 1.0f, Pal.A(Pal.White, 0.55f), 1.7f);
                Vector2[] pts = new Vector2[8];
                for (int i = 0; i < 8; i++)
                {
                    float a = i * Mathf.PI / 4f;
                    float rr = i % 2 == 0 ? r : r * 0.16f;
                    pts[i] = new Vector2(cx + Mathf.Cos(a) * rr, cy + Mathf.Sin(a) * rr);
                }
                c.Poly(pts, Pal.White);
            }
            else
            {
                c.Star(cx, cy, r, r * 0.44f, points, 90f, Pal.White);
            }

            Texture2D tex = c.ToTexture(true);
            s = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
            s.name = key;
            misc[key] = s;
            return s;
        }

        public static Sprite Glow(int size = 256)
        {
            string key = "glow" + size;
            Sprite s;
            if (misc.TryGetValue(key, out s) && s != null) return s;

            Canvas2D c = new Canvas2D(size, size, 1);
            c.Glow(size * 0.5f, size * 0.5f, size * 0.5f, Pal.White, 2.1f);
            Texture2D tex = c.ToTexture(true);
            s = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
            s.name = key;
            misc[key] = s;
            return s;
        }

        public static Sprite Heart(int size = 96)
        {
            string key = "heart" + size;
            Sprite s;
            if (misc.TryGetValue(key, out s) && s != null) return s;
            Canvas2D c = new Canvas2D(size, size, 3);
            c.Heart(size * 0.5f, size * 0.5f, size * 0.46f, Pal.White);
            Texture2D tex = c.ToTexture(true);
            s = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
            s.name = key;
            misc[key] = s;
            return s;
        }

        public static Sprite LockedIcon()
        {
            Sprite s;
            if (misc.TryGetValue("locked", out s) && s != null) return s;

            const int size = 220;
            Canvas2D c = new Canvas2D(size, size, 2);
            c.RoundRect(46f, 40f, size - 46f, size - 76f, 28f, Pal.A(Pal.InkSoft, 0.55f));
            c.Stroke(new[]
            {
                new Vector2(size * 0.5f - 34f, size - 78f),
                new Vector2(size * 0.5f - 34f, size - 132f),
                new Vector2(size * 0.5f + 34f, size - 132f),
                new Vector2(size * 0.5f + 34f, size - 78f),
            }, 16f, Pal.A(Pal.InkSoft, 0.55f));
            for (int i = 0; i < 6; i++)
            {
                float x = 70f + (i % 3) * 40f;
                float y = 96f + (i / 3) * 34f;
                c.Circle(x, y, 5f, Pal.A(Pal.White, 0.45f));
            }
            Texture2D tex = c.ToTexture(true);
            s = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
            s.name = "locked";
            misc["locked"] = s;
            return s;
        }

        // ------------------------------------------------------ backgrounds

        public static Sprite Background(int index)
        {
            Sprite s;
            if (backgrounds.TryGetValue(index, out s) && s != null) return s;

            const int W = 960;
            const int H = 540;
            Canvas2D c = new Canvas2D(W, H, 2);
            c.SetTransform(W / (float)SceneArt.DW);
            SceneArt.Draw(c, index);
            s = c.ToSprite(true);
            s.name = "bg" + index;
            backgrounds[index] = s;
            return s;
        }

        static void MaskRounded(Color32[] px, int w, int h, float radius)
        {
            float cx = w * 0.5f;
            float cy = h * 0.5f;
            float r = Mathf.Min(radius, Mathf.Min(w, h) * 0.5f);
            float halfW = w * 0.5f - r;
            float halfH = h * 0.5f - r;

            for (int y = 0; y < h; y++)
            {
                float py = y + 0.5f - cy;
                for (int x = 0; x < w; x++)
                {
                    float pxx = x + 0.5f - cx;
                    float qx = Mathf.Abs(pxx) - halfW;
                    float qy = Mathf.Abs(py) - halfH;
                    float ox = Mathf.Max(qx, 0f);
                    float oy = Mathf.Max(qy, 0f);
                    float d = Mathf.Min(Mathf.Max(qx, qy), 0f) + Mathf.Sqrt(ox * ox + oy * oy) - r;
                    if (d <= -1f) continue;
                    float k = d < 0f ? 1f + d : Mathf.Clamp01(1f - d);
                    int i = y * w + x;
                    Color32 c = px[i];
                    c.a = (byte)Mathf.RoundToInt(c.a * k);
                    px[i] = c;
                }
            }
        }

        /// <summary>Fondo recortado tipo "cover" al tamaño pedido, con esquinas redondeadas.</summary>
        public static Sprite BackgroundFit(int index, int w, int h, int radius)
        {
            string key = "bgf" + index + "_" + w + "x" + h + "_" + radius;
            Sprite s;
            if (misc.TryGetValue(key, out s) && s != null) return s;

            float scale = Mathf.Max(w / (float)SceneArt.DW, h / (float)SceneArt.DH);
            Canvas2D c = new Canvas2D(w, h, 2);
            c.SetTransform(scale, (w - SceneArt.DW * scale) * 0.5f, (h - SceneArt.DH * scale) * 0.5f);
            SceneArt.Draw(c, index);

            Color32[] px = c.Resolve();
            if (radius > 0) MaskRounded(px, w, h, radius);
            Canvas2D.BleedAlpha(px, w, h, 1);
            Texture2D tex = Canvas2D.MakeTexture(px, w, h, true);
            s = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
            s.name = key;
            misc[key] = s;
            return s;
        }

        /// <summary>Composición de la polaroid: fondo del escenario + capas de la muñeca.</summary>
        public static Sprite Photo(int scenarioIndex, Sprite[] layers, int width = 760, int height = 940)
        {
            Canvas2D c = new Canvas2D(width, height, 2);
            c.FillAll(Pal.White);

            Sprite bg = Background(scenarioIndex);
            Texture2D bgt = bg.texture;
            Color32[] bgPx = bgt.GetPixels32();
            Color32[] bgCopy = new Color32[bgPx.Length];
            System.Array.Copy(bgPx, bgCopy, bgPx.Length);
            Canvas2D.BleedAlpha(bgCopy, bgt.width, bgt.height, 1);

            float bgScale = Mathf.Max(width / (float)bgt.width, height / (float)bgt.height);
            c.BlitBuffer(bgCopy, bgt.width, bgt.height, new Vector2(width * 0.5f, height * 0.5f), bgScale);

            // suelo suave para que la muñeca se apoye
            c.Ellipse(width * 0.5f, height * 0.20f, width * 0.30f, height * 0.045f, Pal.A(Pal.White, 0.20f));

            float dollScale = (height * 0.80f) / Rig.H;
            Vector2 center = new Vector2(width * 0.5f, height * 0.525f);

            foreach (Sprite layer in layers)
            {
                if (layer == null) continue;
                Texture2D t = layer.texture;
                Color32[] px = t.GetPixels32();
                Color32[] copy = new Color32[px.Length];
                System.Array.Copy(px, copy, px.Length);
                Canvas2D.BleedAlpha(copy, t.width, t.height, 1);
                c.BlitBuffer(copy, t.width, t.height, center, dollScale);
            }

            return c.ToSprite(true);
        }
    }
}
