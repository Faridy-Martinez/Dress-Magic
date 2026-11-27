using UnityEngine;

namespace DressMagic.Art
{
    /// <summary>
    /// Rasterizador 2D simple con supersampling y blending alfa correcto.
    /// Todo el arte del juego se dibuja con esto en tiempo de ejecución.
    /// </summary>
    public sealed class Canvas2D
    {
        public readonly int W;
        public readonly int H;

        readonly int ss;
        readonly int bw;
        readonly int bh;
        readonly Color32[] buf;

        float sx = 1f, sy = 1f, ox, oy;

        public Canvas2D(int w, int h, int superSample = 2)
        {
            W = Mathf.Max(1, w);
            H = Mathf.Max(1, h);
            ss = Mathf.Clamp(superSample, 1, 4);
            bw = W * ss;
            bh = H * ss;
            buf = new Color32[bw * bh];
        }

        public void SetTransform(float scale, float offX = 0f, float offY = 0f)
        {
            sx = scale;
            sy = scale;
            ox = offX;
            oy = offY;
        }

        Vector2 T(Vector2 p)
        {
            return new Vector2((p.x * sx + ox) * ss, (p.y * sy + oy) * ss);
        }

        float TS(float v) => v * sx * ss;

        // ---------------------------------------------------------------- core

        static byte B(float v)
        {
            int i = Mathf.RoundToInt(v * 255f);
            return (byte)(i < 0 ? 0 : (i > 255 ? 255 : i));
        }

        public void Blend(int x, int y, Color c)
        {
            if (c.a <= 0.0001f) return;
            if (x < 0 || y < 0 || x >= bw || y >= bh) return;
            int i = y * bw + x;
            Color32 d = buf[i];
            float a = c.a > 1f ? 1f : c.a;
            float da = d.a * (1f / 255f);
            float oa = a + da * (1f - a);
            if (oa <= 0.0008f) return;
            float inv = 1f / oa;
            float k = da * (1f - a) * inv;
            float fa = a * inv;
            float r = c.r * fa + (d.r * (1f / 255f)) * k;
            float g = c.g * fa + (d.g * (1f / 255f)) * k;
            float b = c.b * fa + (d.b * (1f / 255f)) * k;
            buf[i] = new Color32(B(r), B(g), B(b), B(oa));
        }

        /// <summary>Reemplaza el píxel sin mezclar (para borrados suaves).</summary>
        public void Erase(int x, int y, float amount)
        {
            if (x < 0 || y < 0 || x >= bw || y >= bh) return;
            int i = y * bw + x;
            Color32 d = buf[i];
            float a = d.a * (1f - Mathf.Clamp01(amount));
            buf[i] = new Color32(d.r, d.g, d.b, B(a * (1f / 255f)));
        }

        public void FillAll(Color c)
        {
            for (int i = 0; i < buf.Length; i++) buf[i] = new Color32(B(c.r), B(c.g), B(c.b), B(c.a));
        }

        // ------------------------------------------------------------ primitives

        public void Rect(float x0, float y0, float x1, float y1, Color c)
        {
            if (x1 < x0) { float t = x0; x0 = x1; x1 = t; }
            if (y1 < y0) { float t = y0; y0 = y1; y1 = t; }
            Vector2 a = T(new Vector2(x0, y0));
            Vector2 b = T(new Vector2(x1, y1));
            int ix0 = Mathf.Max(0, Mathf.FloorToInt(a.x));
            int ix1 = Mathf.Min(bw - 1, Mathf.CeilToInt(b.x));
            int iy0 = Mathf.Max(0, Mathf.FloorToInt(a.y));
            int iy1 = Mathf.Min(bh - 1, Mathf.CeilToInt(b.y));
            for (int y = iy0; y <= iy1; y++)
                for (int x = ix0; x <= ix1; x++)
                    Blend(x, y, c);
        }

        public void Ellipse(float cx, float cy, float rx, float ry, Color c)
        {
            if (rx <= 0f || ry <= 0f || c.a <= 0f) return;
            Vector2 pc = T(new Vector2(cx, cy));
            float ex = TS(rx);
            float ey = TS(ry);
            int x0 = Mathf.Max(0, Mathf.FloorToInt(pc.x - ex));
            int x1 = Mathf.Min(bw - 1, Mathf.CeilToInt(pc.x + ex));
            int y0 = Mathf.Max(0, Mathf.FloorToInt(pc.y - ey));
            int y1 = Mathf.Min(bh - 1, Mathf.CeilToInt(pc.y + ey));
            float invx = 1f / (ex * ex);
            float invy = 1f / (ey * ey);
            for (int y = y0; y <= y1; y++)
            {
                float dy = y + 0.5f - pc.y;
                float dy2 = dy * dy * invy;
                if (dy2 > 1f) continue;
                for (int x = x0; x <= x1; x++)
                {
                    float dx = x + 0.5f - pc.x;
                    if (dx * dx * invx + dy2 <= 1f) Blend(x, y, c);
                }
            }
        }

        public void Ellipse(Vector2 c, float rx, float ry, Color col) => Ellipse(c.x, c.y, rx, ry, col);

        public void Circle(float cx, float cy, float r, Color c) => Ellipse(cx, cy, r, r, c);

        /// <summary>Contorno elíptico de grosor dado.</summary>
        public void EllipseRing(float cx, float cy, float rx, float ry, float width, Color c)
        {
            Vector2 pc = T(new Vector2(cx, cy));
            float ex = TS(rx), ey = TS(ry), w = TS(width);
            int x0 = Mathf.Max(0, Mathf.FloorToInt(pc.x - ex - w));
            int x1 = Mathf.Min(bw - 1, Mathf.CeilToInt(pc.x + ex + w));
            int y0 = Mathf.Max(0, Mathf.FloorToInt(pc.y - ey - w));
            int y1 = Mathf.Min(bh - 1, Mathf.CeilToInt(pc.y + ey + w));
            float inner = 1f - w / Mathf.Max(ex, ey);
            for (int y = y0; y <= y1; y++)
            {
                for (int x = x0; x <= x1; x++)
                {
                    float dx = (x + 0.5f - pc.x) / ex;
                    float dy = (y + 0.5f - pc.y) / ey;
                    float d = dx * dx + dy * dy;
                    if (d > 1f) continue;
                    if (d >= inner * inner) Blend(x, y, c);
                }
            }
        }

        public void RoundRect(float x0, float y0, float x1, float y1, float radius, Color c)
        {
            if (x1 < x0) { float t = x0; x0 = x1; x1 = t; }
            if (y1 < y0) { float t = y0; y0 = y1; y1 = t; }
            float r = Mathf.Min(radius, Mathf.Min(x1 - x0, y1 - y0) * 0.5f);
            Vector2 a = T(new Vector2(x0, y0));
            Vector2 b = T(new Vector2(x1, y1));
            float rr = TS(r);
            int ix0 = Mathf.Max(0, Mathf.FloorToInt(a.x));
            int ix1 = Mathf.Min(bw - 1, Mathf.CeilToInt(b.x));
            int iy0 = Mathf.Max(0, Mathf.FloorToInt(a.y));
            int iy1 = Mathf.Min(bh - 1, Mathf.CeilToInt(b.y));
            for (int y = iy0; y <= iy1; y++)
            {
                float py = y + 0.5f;
                for (int x = ix0; x <= ix1; x++)
                {
                    float px = x + 0.5f;
                    float dx = 0f, dy = 0f;
                    if (px < a.x + rr) dx = a.x + rr - px;
                    else if (px > b.x - rr) dx = px - (b.x - rr);
                    if (py < a.y + rr) dy = a.y + rr - py;
                    else if (py > b.y - rr) dy = py - (b.y - rr);
                    if (dx * dx + dy * dy <= rr * rr) Blend(x, y, c);
                }
            }
        }

        /// <summary>Relleno de polígono (even-odd) con suavizado por supersampling.</summary>
        public void Poly(Vector2[] pts, Color c)
        {
            int n = pts.Length;
            if (n < 3) return;
            Vector2[] tp = new Vector2[n];
            float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;
            for (int i = 0; i < n; i++)
            {
                tp[i] = T(pts[i]);
                if (tp[i].x < minX) minX = tp[i].x;
                if (tp[i].y < minY) minY = tp[i].y;
                if (tp[i].x > maxX) maxX = tp[i].x;
                if (tp[i].y > maxY) maxY = tp[i].y;
            }

            int x0 = Mathf.Max(0, Mathf.FloorToInt(minX));
            int x1 = Mathf.Min(bw - 1, Mathf.CeilToInt(maxX));
            int y0 = Mathf.Max(0, Mathf.FloorToInt(minY));
            int y1 = Mathf.Min(bh - 1, Mathf.CeilToInt(maxY));

            float[] xs = new float[16];
            for (int y = y0; y <= y1; y++)
            {
                float sy = y + 0.5f;
                int cnt = 0;
                for (int i = 0; i < n; i++)
                {
                    Vector2 p1 = tp[i];
                    Vector2 p2 = tp[(i + 1) % n];
                    if (Mathf.Approximately(p1.y, p2.y)) continue;
                    if ((sy >= p1.y && sy < p2.y) || (sy >= p2.y && sy < p1.y))
                    {
                        float t = (sy - p1.y) / (p2.y - p1.y);
                        float xCross = p1.x + (p2.x - p1.x) * t;
                        if (cnt == xs.Length) System.Array.Resize(ref xs, xs.Length * 2);
                        xs[cnt++] = xCross;
                    }
                }
                if (cnt < 2) continue;
                System.Array.Sort(xs, 0, cnt);
                for (int i = 0; i + 1 < cnt; i += 2)
                {
                    int a = Mathf.Max(x0, Mathf.CeilToInt(xs[i] - 0.5f));
                    int b = Mathf.Min(x1, Mathf.FloorToInt(xs[i + 1] - 0.5f));
                    for (int x = a; x <= b; x++) Blend(x, y, c);
                }
            }
        }

        public void Line(Vector2 a, Vector2 b, float width, Color c)
        {
            Vector2 ta = T(a), tb = T(b);
            float hw = Mathf.Max(0.5f, TS(width) * 0.5f);
            float minX = Mathf.Min(ta.x, tb.x) - hw, maxX = Mathf.Max(ta.x, tb.x) + hw;
            float minY = Mathf.Min(ta.y, tb.y) - hw, maxY = Mathf.Max(ta.y, tb.y) + hw;
            int x0 = Mathf.Max(0, Mathf.FloorToInt(minX));
            int x1 = Mathf.Min(bw - 1, Mathf.CeilToInt(maxX));
            int y0 = Mathf.Max(0, Mathf.FloorToInt(minY));
            int y1 = Mathf.Min(bh - 1, Mathf.CeilToInt(maxY));
            Vector2 d = tb - ta;
            float len2 = d.sqrMagnitude;
            for (int y = y0; y <= y1; y++)
            {
                for (int x = x0; x <= x1; x++)
                {
                    Vector2 p = new Vector2(x + 0.5f, y + 0.5f);
                    float t = len2 <= 0.0001f ? 0f : Mathf.Clamp01(Vector2.Dot(p - ta, d) / len2);
                    Vector2 closest = ta + d * t;
                    if ((p - closest).sqrMagnitude <= hw * hw) Blend(x, y, c);
                }
            }
        }

        public void Stroke(Vector2[] pts, float width, Color c, bool closed = false)
        {
            for (int i = 0; i + 1 < pts.Length; i++) Line(pts[i], pts[i + 1], width, c);
            if (closed && pts.Length > 2) Line(pts[pts.Length - 1], pts[0], width, c);
            float r = width * 0.5f;
            for (int i = 0; i < pts.Length; i++) Circle(pts[i].x, pts[i].y, r, c);
        }

        /// <summary>Halo radial suave.</summary>
        public void Glow(float cx, float cy, float radius, Color c, float power = 2f)
        {
            Vector2 pc = T(new Vector2(cx, cy));
            float r = TS(radius);
            int x0 = Mathf.Max(0, Mathf.FloorToInt(pc.x - r));
            int x1 = Mathf.Min(bw - 1, Mathf.CeilToInt(pc.x + r));
            int y0 = Mathf.Max(0, Mathf.FloorToInt(pc.y - r));
            int y1 = Mathf.Min(bh - 1, Mathf.CeilToInt(pc.y + r));
            for (int y = y0; y <= y1; y++)
            {
                for (int x = x0; x <= x1; x++)
                {
                    float dx = x + 0.5f - pc.x;
                    float dy = y + 0.5f - pc.y;
                    float d = Mathf.Sqrt(dx * dx + dy * dy) / r;
                    if (d > 1f) continue;
                    float f = Mathf.Pow(1f - d, power);
                    Color cc = c;
                    cc.a = c.a * f;
                    Blend(x, y, cc);
                }
            }
        }

        public void Star(float cx, float cy, float rOuter, float rInner, int points, float rotationDeg, Color c)
        {
            Vector2[] p = new Vector2[points * 2];
            float step = Mathf.PI / points;
            float rot = rotationDeg * Mathf.Deg2Rad;
            for (int i = 0; i < p.Length; i++)
            {
                float r = (i % 2 == 0) ? rOuter : rInner;
                float a = rot + i * step;
                p[i] = new Vector2(cx + Mathf.Cos(a) * r, cy + Mathf.Sin(a) * r);
            }
            Poly(p, c);
        }

        public void Heart(float cx, float cy, float size, Color c)
        {
            Vector2[] p = new Vector2[40];
            for (int i = 0; i < p.Length; i++)
            {
                float t = i / (float)p.Length * Mathf.PI * 2f;
                float x = 16f * Mathf.Pow(Mathf.Sin(t), 3f);
                float y = 13f * Mathf.Cos(t) - 5f * Mathf.Cos(2f * t) - 2f * Mathf.Cos(3f * t) - Mathf.Cos(4f * t);
                p[i] = new Vector2(cx + x * size / 16f, cy + y * size / 16f);
            }
            Poly(p, c);
        }

        public void VGradient(Color top, Color bottom)
        {
            for (int y = 0; y < bh; y++)
            {
                float t = y / (float)(bh - 1);
                Color c = Color.Lerp(bottom, top, t);
                Color32 c32 = new Color32(B(c.r), B(c.g), B(c.b), B(c.a));
                int row = y * bw;
                for (int x = 0; x < bw; x++) buf[row + x] = c32;
            }
        }

        public void MultiGradient(Color c0, Color c1, Color c2, float mid)
        {
            for (int y = 0; y < bh; y++)
            {
                float t = y / (float)(bh - 1);
                Color c = t < mid ? Color.Lerp(c0, c1, t / Mathf.Max(0.001f, mid))
                                  : Color.Lerp(c1, c2, (t - mid) / Mathf.Max(0.001f, 1f - mid));
                Color32 c32 = new Color32(B(c.r), B(c.g), B(c.b), B(c.a));
                int row = y * bw;
                for (int x = 0; x < bw; x++) buf[row + x] = c32;
            }
        }

        // ------------------------------------------------------------- transfer

        /// <summary>Copia una textura RGBA (ya "bleeded") escalada y centrada. Muestreo bilineal.</summary>
        public void BlitBuffer(Color32[] src, int sw, int sh, Vector2 destCenter, float destScale, float rotationDeg = 0f)
        {
            BlitRect(src, sw, sh, 0f, 0f, 1f, 1f, destCenter, destScale, rotationDeg);
        }

        /// <summary>Copia una región (uv 0..1) de una textura RGBA escalada y centrada.</summary>
        public void BlitRect(Color32[] src, int sw, int sh, float u0, float v0, float u1, float v1,
            Vector2 destCenter, float destScale, float rotationDeg = 0f)
        {
            if (src == null || sw <= 0 || sh <= 0) return;
            float dw = (u1 - u0) * sw * destScale * sx;
            float dh = (v1 - v0) * sh * destScale * sy;
            float rot = rotationDeg * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rot), sin = Mathf.Sin(rot);
            Vector2 pc = T(destCenter);
            float halfDiag = Mathf.Sqrt(dw * dw + dh * dh) * 0.5f + 2f;
            int x0 = Mathf.Max(0, Mathf.FloorToInt(pc.x - halfDiag));
            int x1 = Mathf.Min(bw - 1, Mathf.CeilToInt(pc.x + halfDiag));
            int y0 = Mathf.Max(0, Mathf.FloorToInt(pc.y - halfDiag));
            int y1 = Mathf.Min(bh - 1, Mathf.CeilToInt(pc.y + halfDiag));

            for (int y = y0; y <= y1; y++)
            {
                for (int x = x0; x <= x1; x++)
                {
                    float dx = x + 0.5f - pc.x;
                    float dy = y + 0.5f - pc.y;
                    float lx = dx * cos + dy * sin;
                    float ly = -dx * sin + dy * cos;
                    float u = lx / dw + 0.5f;
                    float v = ly / dh + 0.5f;
                    if (u < 0f || u > 1f || v < 0f || v > 1f) continue;
                    Color c = SampleBilinear(src, sw, sh, Mathf.Lerp(u0, u1, u), Mathf.Lerp(v0, v1, v));
                    if (c.a <= 0.002f) continue;
                    Blend(x, y, c);
                }
            }
        }

        static Color SampleBilinear(Color32[] src, int sw, int sh, float u, float v)
        {
            float fx = u * (sw - 1);
            float fy = v * (sh - 1);
            int x0 = Mathf.FloorToInt(fx);
            int y0 = Mathf.FloorToInt(fy);
            int x1 = Mathf.Min(x0 + 1, sw - 1);
            int y1 = Mathf.Min(y0 + 1, sh - 1);
            float tx = fx - x0;
            float ty = fy - y0;
            Color32 a = src[y0 * sw + x0];
            Color32 b = src[y0 * sw + x1];
            Color32 c = src[y1 * sw + x0];
            Color32 d = src[y1 * sw + x1];
            // acumulación premultiplicada para evitar halos oscuros
            float w00 = (1 - tx) * (1 - ty), w10 = tx * (1 - ty), w01 = (1 - tx) * ty, w11 = tx * ty;
            float aa = a.a / 255f, ba = b.a / 255f, ca = c.a / 255f, da = d.a / 255f;
            float alpha = aa * w00 + ba * w10 + ca * w01 + da * w11;
            if (alpha <= 0.0001f) return new Color(0, 0, 0, 0);
            float r = (a.r / 255f * aa * w00 + b.r / 255f * ba * w10 + c.r / 255f * ca * w01 + d.r / 255f * da * w11) / alpha;
            float g = (a.g / 255f * aa * w00 + b.g / 255f * ba * w10 + c.g / 255f * ca * w01 + d.g / 255f * da * w11) / alpha;
            float bl = (a.b / 255f * aa * w00 + b.b / 255f * ba * w10 + c.b / 255f * ca * w01 + d.b / 255f * da * w11) / alpha;
            return new Color(r, g, bl, alpha);
        }

        // ------------------------------------------------------------- resolve

        /// <summary>Reduce el buffer supersampleado a la resolución final (filtro de caja).</summary>
        public Color32[] Resolve()
        {
            Color32[] outPx = new Color32[W * H];
            for (int y = 0; y < H; y++)
            {
                for (int x = 0; x < W; x++)
                {
                    float r = 0, g = 0, b = 0, a = 0;
                    int baseY = y * ss;
                    int baseX = x * ss;
                    for (int j = 0; j < ss; j++)
                    {
                        int row = (baseY + j) * bw;
                        for (int i = 0; i < ss; i++)
                        {
                            Color32 s = buf[row + baseX + i];
                            float sa = s.a / 255f;
                            r += s.r / 255f * sa;
                            g += s.g / 255f * sa;
                            b += s.b / 255f * sa;
                            a += sa;
                        }
                    }
                    if (a <= 0.0001f) { outPx[y * W + x] = new Color32(0, 0, 0, 0); continue; }
                    float inv = 1f / a;
                    outPx[y * W + x] = new Color32(B(r * inv), B(g * inv), B(b * inv), B(a / (ss * ss)));
                }
            }
            return outPx;
        }

        /// <summary>Rellena el RGB de los píxeles transparentes con el color vecino (evita halos).</summary>
        public static void BleedAlpha(Color32[] px, int w, int h, int passes = 2)
        {
            Color32[] copy = new Color32[px.Length];
            for (int p = 0; p < passes; p++)
            {
                System.Array.Copy(px, copy, px.Length);
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        int i = y * w + x;
                        if (copy[i].a > 8) continue;
                        int r = 0, g = 0, b = 0, n = 0;
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            int yy = y + dy;
                            if (yy < 0 || yy >= h) continue;
                            for (int dx = -1; dx <= 1; dx++)
                            {
                                int xx = x + dx;
                                if (xx < 0 || xx >= w) continue;
                                if (dx == 0 && dy == 0) continue;
                                Color32 s = copy[yy * w + xx];
                                if (s.a <= 8) continue;
                                r += s.r; g += s.g; b += s.b; n++;
                            }
                        }
                        if (n == 0) continue;
                        px[i] = new Color32((byte)(r / n), (byte)(g / n), (byte)(b / n), copy[i].a);
                    }
                }
            }
        }

        public static Texture2D MakeTexture(Color32[] px, int w, int h, bool readable = true)
        {
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.SetPixels32(px);
            tex.Apply(false, !readable);
            return tex;
        }

        public Texture2D ToTexture(bool readable = true)
        {
            Color32[] px = Resolve();
            BleedAlpha(px, W, H, 2);
            return MakeTexture(px, W, H, readable);
        }

        public Sprite ToSprite(bool readable = true)
        {
            Texture2D tex = ToTexture(readable);
            Sprite s = Sprite.Create(tex, new Rect(0, 0, W, H), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
            s.name = "gen";
            return s;
        }
    }
}
