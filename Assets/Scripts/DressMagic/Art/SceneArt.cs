using UnityEngine;

namespace DressMagic.Art
{
    public struct ScenarioDef
    {
        public string id;
        public string name;
        public string tagline;
        public Color accent;
    }

    /// <summary>Fondos dibujados a mano para cada escenario (espacio de diseño 1600 x 900).</summary>
    public static class SceneArt
    {
        public const int DW = 1600;
        public const int DH = 900;

        public static readonly ScenarioDef[] All =
        {
            new ScenarioDef { id = "dulces", name = "Dulcería Mágica", tagline = "Dulces y caramelos", accent = Pal.Hex("F7A8C4") },
            new ScenarioDef { id = "pasteleria", name = "Pastelería Encantada", tagline = "Pasteles y cupcakes", accent = Pal.Hex("FFCFB0") },
            new ScenarioDef { id = "parque", name = "Parque de Diversiones", tagline = "Rueda y globos", accent = Pal.Hex("A7D8F7") },
            new ScenarioDef { id = "cine", name = "Cine Estelar", tagline = "Estrellas y luces", accent = Pal.Hex("B9A2E4") },
            new ScenarioDef { id = "hielo", name = "Pista de Hielo", tagline = "Fiesta sobre el hielo", accent = Pal.Hex("A8E6EF") },
        };

        public static int IndexOf(string id)
        {
            for (int i = 0; i < All.Length; i++) if (All[i].id == id) return i;
            return 0;
        }

        public static void Draw(Canvas2D c, int index)
        {
            switch (Mathf.Clamp(index, 0, All.Length - 1))
            {
                case 0: Dulces(c); break;
                case 1: Pasteleria(c); break;
                case 2: Parque(c); break;
                case 3: Cine(c); break;
                case 4: Hielo(c); break;
            }
        }

        // ------------------------------------------------------------- helpers

        static void Cloud(Canvas2D c, float x, float y, float s, Color col)
        {
            c.Circle(x, y, 34f * s, col);
            c.Circle(x + 40f * s, y + 16f * s, 46f * s, col);
            c.Circle(x + 90f * s, y + 2f * s, 32f * s, col);
            c.Ellipse(x + 44f * s, y - 12f * s, 72f * s, 28f * s, col);
        }

        static void SparkleField(Canvas2D c, int count, float yMin, float yMax, Color col, int seed)
        {
            Random.State prev = Random.state;
            Random.InitState(seed);
            for (int i = 0; i < count; i++)
            {
                float x = Random.Range(20f, DW - 20f);
                float y = Random.Range(yMin, yMax);
                float r = Random.Range(3f, 9f);
                c.Star(x, y, r, r * 0.35f, 4, Random.Range(0f, 90f), Pal.A(col, Random.Range(0.35f, 0.9f)));
            }
            Random.state = prev;
        }

        static void Bokeh(Canvas2D c, int count, Color col, int seed)
        {
            Random.State prev = Random.state;
            Random.InitState(seed);
            for (int i = 0; i < count; i++)
            {
                float x = Random.Range(0f, DW);
                float y = Random.Range(380f, DH);
                c.Glow(x, y, Random.Range(40f, 120f), Pal.A(col, Random.Range(0.05f, 0.16f)), 2.2f);
            }
            Random.state = prev;
        }

        static void Bunting(Canvas2D c, float y, float sag, int count, Color[] cols)
        {
            Vector2 prev2 = new Vector2(-10f, y);
            for (int i = 1; i <= 12; i++)
            {
                float t = i / 12f;
                Vector2 p = new Vector2(Mathf.Lerp(-10f, DW + 10f, t), y - Mathf.Sin(t * Mathf.PI) * sag);
                c.Line(prev2, p, 5f, Pal.A(Pal.Hex("FFF7EC"), 0.9f));
                prev2 = p;
            }

            for (int i = 0; i < count; i++)
            {
                float t = (i + 0.5f) / count;
                float x = Mathf.Lerp(20f, DW - 20f, t);
                float yy = y - Mathf.Sin(t * Mathf.PI) * sag;
                Color col = cols[i % cols.Length];
                Vector2[] tri =
                {
                    new Vector2(x - 26f, yy),
                    new Vector2(x + 26f, yy),
                    new Vector2(x, yy - 54f),
                };
                c.Poly(tri, col);
                c.Circle(x, yy - 44f, 5f, Pal.A(Pal.White, 0.6f));
            }
        }

        static void Snowflake(Canvas2D c, float x, float y, float r, Color col)
        {
            for (int i = 0; i < 6; i++)
            {
                float a = (i * 60f + 15f) * Mathf.Deg2Rad;
                Vector2 p = new Vector2(x + Mathf.Cos(a) * r, y + Mathf.Sin(a) * r);
                Vector2 m = new Vector2(x + Mathf.Cos(a + 0.7f) * r * 0.5f, y + Mathf.Sin(a + 0.7f) * r * 0.5f);
                Vector2 m2 = new Vector2(x + Mathf.Cos(a - 0.7f) * r * 0.5f, y + Mathf.Sin(a - 0.7f) * r * 0.5f);
                c.Line(new Vector2(x, y), p, 3.4f, col);
                c.Line(new Vector2(x, y), m, 2.4f, Pal.A(col, 0.7f));
                c.Line(new Vector2(x, y), m2, 2.4f, Pal.A(col, 0.7f));
            }
        }

        static void Vignette(Canvas2D c, float strength)
        {
            c.Glow(DW * 0.5f, DH * 0.5f, DW * 0.78f, Pal.A(Pal.White, 0f));
            c.Glow(DW * 0.5f, DH * 0.48f, DW * 0.85f, Pal.A(new Color(1, 1, 1), 0f));
            // esquinas suaves
            c.Glow(0f, DH, DW * 0.55f, Pal.A(Pal.Ink, strength * 0.35f), 1.6f);
            c.Glow(DW, DH, DW * 0.55f, Pal.A(Pal.Ink, strength * 0.35f), 1.6f);
        }

        // --------------------------------------------------------- 0 · DULCES

        static void Dulces(Canvas2D c)
        {
            c.MultiGradient(Pal.Hex("FFEFF7"), Pal.Hex("FFD8EA"), Pal.Hex("F9C2DC"), 0.55f);
            Bokeh(c, 12, Pal.Pink, 301);

            // estanterías
            for (int row = 0; row < 2; row++)
            {
                float y = 380f + row * 210f;
                c.Rect(0f, y, DW, y + 22f, Pal.Hex("F5C3D8"));
                c.Rect(0f, y + 16f, DW, y + 22f, Pal.A(Pal.White, 0.55f));

                for (int i = 0; i < 9; i++)
                {
                    float x = 60f + i * 174f;
                    float jh = 66f + (i % 3) * 16f;
                    c.RoundRect(x - 34f, y + 22f, x + 34f, y + 22f + jh, 14f, Pal.A(Pal.White, 0.75f));
                    c.RoundRect(x - 30f, y + 26f, x + 30f, y + 18f + jh, 11f, Pal.A(i % 2 == 0 ? Pal.Mint : Pal.Lilac, 0.85f));
                    for (int k = 0; k < 6; k++)
                    {
                        float cx = x - 22f + (k % 3) * 22f;
                        float cy = y + 40f + (k / 3) * 26f;
                        c.Circle(cx, cy, 8f, k % 2 == 0 ? Pal.Hex("FFD9E6") : Pal.Hex("FFF3B0"));
                    }
                }
            }

            // mostrador inferior
            c.Rect(0f, 0f, DW, 210f, Pal.Hex("FFD3E4"));
            c.MultiGradient(Pal.A(Pal.White, 0f), Pal.A(Pal.White, 0f), Pal.A(Pal.Hex("F7A8C4"), 0.35f), 0.5f);

            // piruleta grande izquierda
            c.Stroke(new[] { new Vector2(210f, 60f), new Vector2(210f, 330f) }, 16f, Pal.Hex("FFF7EC"));
            c.Circle(210f, 400f, 96f, Pal.Hex("FF9CC4"));
            c.Circle(210f, 400f, 84f, Pal.White);
            for (int i = 0; i < 5; i++)
            {
                float t = i / 5f * Mathf.PI * 2.4f;
                Vector2 p = new Vector2(210f + Mathf.Cos(t) * (14f + i * 14f), 400f + Mathf.Sin(t) * (14f + i * 14f));
                c.Circle(p.x, p.y, 8f, Pal.Hex("FF9CC4"));
            }

            // bastón de caramelo derecha
            for (int i = 0; i < 2; i++)
            {
                float x = 1380f + i * 90f;
                c.Stroke(new[]
                {
                    new Vector2(x, 60f), new Vector2(x, 420f),
                    new Vector2(x + 20f, 470f), new Vector2(x + 60f, 470f),
                }, 40f, Pal.White);
                for (int k = 0; k < 6; k++)
                {
                    float yy = 100f + k * 60f;
                    c.Stroke(new[] { new Vector2(x - 18f, yy), new Vector2(x + 18f, yy + 34f) }, 12f, Pal.Hex("F062A0"));
                }
            }

            // toldo superior
            for (int i = 0; i < 16; i++)
            {
                float x = i * 100f;
                c.Poly(new[]
                {
                    new Vector2(x, DH), new Vector2(x + 100f, DH),
                    new Vector2(x + 100f, DH - 150f), new Vector2(x + 50f, DH - 196f),
                    new Vector2(x, DH - 150f),
                }, i % 2 == 0 ? Pal.Hex("FFB3D1") : Pal.White);
            }
            c.Rect(0f, DH - 158f, DW, DH - 140f, Pal.A(Pal.Hex("F062A0"), 0.25f));

            SparkleField(c, 26, 220f, 820f, Pal.White, 311);
            Vignette(c, 0.5f);
        }

        // ----------------------------------------------------- 1 · PASTELERÍA

        static void Pasteleria(Canvas2D c)
        {
            c.MultiGradient(Pal.Hex("FFF8EC"), Pal.Hex("FFEBD3"), Pal.Hex("FFDCB8"), 0.55f);
            Bokeh(c, 10, Pal.Peach, 401);

            // ventana arqueada
            float wx = 700f, wy = 470f;
            c.RoundRect(wx - 190f, wy - 210f, wx + 190f, wy + 250f, 150f, Pal.White);
            c.RoundRect(wx - 176f, wy - 196f, wx + 176f, wy + 236f, 138f, Pal.Hex("BFE0F7"));
            Cloud(c, wx - 130f, wy + 120f, 0.8f, Pal.A(Pal.White, 0.9f));
            Cloud(c, wx + 10f, wy + 60f, 0.6f, Pal.A(Pal.White, 0.8f));
            c.Rect(wx - 8f, wy - 196f, wx + 8f, wy + 236f, Pal.White);
            c.Rect(wx - 176f, wy - 10f, wx + 176f, wy + 6f, Pal.White);

            // cortinas
            c.Poly(new[]
            {
                new Vector2(wx - 250f, wy + 300f), new Vector2(wx - 130f, wy + 300f),
                new Vector2(wx - 190f, wy - 230f), new Vector2(wx - 270f, wy - 230f),
            }, Pal.A(Pal.Hex("FFCFB0"), 0.85f));
            c.Poly(new[]
            {
                new Vector2(wx + 250f, wy + 300f), new Vector2(wx + 130f, wy + 300f),
                new Vector2(wx + 190f, wy - 230f), new Vector2(wx + 270f, wy - 230f),
            }, Pal.A(Pal.Hex("FFCFB0"), 0.85f));

            // estante con pasteles
            c.Rect(0f, 330f, DW, 352f, Pal.Hex("E8B98E"));
            c.Rect(0f, 346f, DW, 352f, Pal.A(Pal.White, 0.6f));

            for (int i = 0; i < 5; i++)
            {
                float x = 120f + i * 300f;
                c.RoundRect(x - 60f, 352f, x + 60f, 430f, 12f, Pal.Hex("FFE3C9"));
                c.Ellipse(x, 430f, 58f, 18f, Pal.Hex("FFF3E3"));
                c.Ellipse(x, 448f, 40f, 14f, Pal.Hex("FF9CC4"));
                c.Circle(x, 476f, 12f, Pal.Hex("F062A0"));
            }

            // tarta de pisos derecha
            float cx2 = 1330f;
            for (int i = 2; i >= 0; i--)
            {
                float y = 60f + i * 96f;
                float r = 150f - i * 34f;
                c.RoundRect(cx2 - r, y, cx2 + r, y + 92f, 20f, Pal.Hex("FFEFE0"));
                c.Rect(cx2 - r, y + 74f, cx2 + r, y + 92f, Pal.Hex("FFB3D1"));
                for (int k = -2; k <= 2; k++) c.Circle(cx2 + k * (r / 3f), y + 92f, 16f, Pal.Hex("FFF3E3"));
            }
            c.Circle(cx2, 350f, 18f, Pal.Hex("F062A0"));
            c.Stroke(new[] { new Vector2(cx2, 368f), new Vector2(cx2, 410f) }, 6f, Pal.Hex("6FCFBC"));

            // cupcakes izquierda
            for (int i = 0; i < 3; i++)
            {
                float x = 250f + i * 150f;
                float y = 60f + (i == 1 ? 26f : 0f);
                c.Poly(new[]
                {
                    new Vector2(x - 52f, y), new Vector2(x + 52f, y),
                    new Vector2(x + 40f, y + 84f), new Vector2(x - 40f, y + 84f),
                }, Pal.Hex("F5C3D8"));
                c.Ellipse(x, y + 96f, 56f, 40f, Pal.Hex("FFF6EA"));
                c.Ellipse(x, y + 112f, 46f, 32f, Pal.Hex("FFCFB0"));
                c.Circle(x, y + 142f, 12f, Pal.Hex("F062A0"));
            }

            // suelo a cuadros
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    if ((i + j) % 2 == 0) continue;
                    c.Rect(i * 80f, j * 40f, i * 80f + 80f, j * 40f + 40f, Pal.A(Pal.Hex("E8B98E"), 0.22f));
                }
            }

            Bunting(c, 830f, 90f, 14, new[] { Pal.Hex("FF9CC4"), Pal.White, Pal.Hex("A9E5D8"), Pal.Hex("FFE197") });
            SparkleField(c, 22, 200f, 800f, Pal.White, 411);
            Vignette(c, 0.42f);
        }

        // ---------------------------------------------------------- 2 · PARQUE

        static void Parque(Canvas2D c)
        {
            c.MultiGradient(Pal.Hex("D8F0FF"), Pal.Hex("B7E0FB"), Pal.Hex("E8F7FF"), 0.6f);

            Cloud(c, 120f, 720f, 1.3f, Pal.A(Pal.White, 0.92f));
            Cloud(c, 520f, 800f, 0.9f, Pal.A(Pal.White, 0.8f));
            Cloud(c, 900f, 690f, 1.1f, Pal.A(Pal.White, 0.85f));
            Cloud(c, 1320f, 780f, 0.8f, Pal.A(Pal.White, 0.75f));

            // colinas
            c.Ellipse(300f, 130f, 520f, 240f, Pal.Hex("A8E0A0"));
            c.Ellipse(1150f, 110f, 640f, 260f, Pal.Hex("93D68F"));
            c.Rect(0f, 0f, DW, 130f, Pal.Hex("8ACD86"));

            // noria
            float fx = 1180f, fy = 520f, fr = 236f;
            c.Stroke(new[] { new Vector2(fx - 120f, 100f), new Vector2(fx, fy) }, 20f, Pal.Hex("D9A6E8"));
            c.Stroke(new[] { new Vector2(fx + 120f, 100f), new Vector2(fx, fy) }, 20f, Pal.Hex("D9A6E8"));
            c.EllipseRing(fx, fy, fr, fr, 16f, Pal.Hex("CDB8F2"));
            c.EllipseRing(fx, fy, fr - 26f, fr - 26f, 5f, Pal.A(Pal.White, 0.8f));

            Color[] cabinCols =
            {
                Pal.Hex("FF9CC4"), Pal.Hex("A9E5D8"), Pal.Hex("FFE197"),
                Pal.Hex("A7D8F7"), Pal.Hex("CDB8F2"), Pal.Hex("FFB59B"),
                Pal.Hex("F7A8C4"), Pal.Hex("A5E7D6"),
            };
            for (int i = 0; i < 8; i++)
            {
                float a = i / 8f * Mathf.PI * 2f + 0.2f;
                float px = fx + Mathf.Cos(a) * fr;
                float py = fy + Mathf.Sin(a) * fr;
                c.Line(new Vector2(fx, fy), new Vector2(px, py), 6f, Pal.A(Pal.Hex("CDB8F2"), 0.85f));
                c.Circle(px, py - 22f, 30f, Pal.Ink);
                c.Circle(px, py - 22f, 26f, cabinCols[i % cabinCols.Length]);
                c.Circle(px, py - 14f, 12f, Pal.A(Pal.White, 0.55f));
            }
            c.Circle(fx, fy, 34f, Pal.Ink);
            c.Circle(fx, fy, 28f, Pal.Hex("FFE197"));

            // carpa a la izquierda
            c.Poly(new[]
            {
                new Vector2(300f, 120f), new Vector2(620f, 120f),
                new Vector2(580f, 470f), new Vector2(340f, 470f),
            }, Pal.A(Pal.White, 0.9f));
            for (int i = 0; i < 5; i++)
            {
                float x = 340f + i * 48f;
                c.Poly(new[]
                {
                    new Vector2(x, 470f), new Vector2(x + 48f, 470f),
                    new Vector2(x + 54f, 120f), new Vector2(x + 6f, 120f),
                }, i % 2 == 0 ? Pal.A(Pal.Hex("FF9CC4"), 0.55f) : Pal.A(Pal.White, 0f));
            }
            c.Poly(new[]
            {
                new Vector2(320f, 470f), new Vector2(600f, 470f),
                new Vector2(460f, 590f),
            }, Pal.Hex("FF9CC4"));
            c.Circle(460f, 596f, 16f, Pal.Hex("FFE197"));
            c.Stroke(new[] { new Vector2(460f, 606f), new Vector2(460f, 660f) }, 5f, Pal.White);
            c.Poly(new[]
            {
                new Vector2(460f, 660f), new Vector2(520f, 640f), new Vector2(460f, 620f),
            }, Pal.Hex("A7D8F7"));

            // globos
            Random.State prev = Random.state;
            Random.InitState(421);
            for (int i = 0; i < 7; i++)
            {
                float x = 700f + i * 70f + Random.Range(-20f, 20f);
                float y = 620f + Random.Range(0f, 180f);
                Color col = cabinCols[i % cabinCols.Length];
                c.Ellipse(x, y, 34f, 42f, col);
                c.Circle(x - 10f, y + 12f, 8f, Pal.A(Pal.White, 0.55f));
                c.Stroke(new[] { new Vector2(x, y - 42f), new Vector2(x + 6f, y - 130f) }, 2.5f, Pal.A(Pal.White, 0.8f));
            }
            Random.state = prev;

            Bunting(c, 860f, 60f, 12, cabinCols);
            SparkleField(c, 30, 150f, 850f, Pal.White, 431);
            Vignette(c, 0.35f);
        }

        // ------------------------------------------------------------ 3 · CINE

        static void Cine(Canvas2D c)
        {
            c.MultiGradient(Pal.Hex("2B1B47"), Pal.Hex("4A2F7A"), Pal.Hex("6B4C9E"), 0.62f);

            // estrellas
            Random.State prev = Random.state;
            Random.InitState(501);
            for (int i = 0; i < 150; i++)
            {
                float x = Random.Range(0f, DW);
                float y = Random.Range(180f, DH);
                float r = Random.Range(1.5f, 5f);
                c.Circle(x, y, r, Pal.A(Pal.White, Random.Range(0.25f, 0.95f)));
            }
            Random.state = prev;

            // haz de luz
            c.Poly(new[]
            {
                new Vector2(300f, -40f), new Vector2(560f, -40f),
                new Vector2(980f, DH), new Vector2(520f, DH),
            }, Pal.A(Pal.Hex("BFD6FF"), 0.10f));
            c.Poly(new[]
            {
                new Vector2(1120f, -40f), new Vector2(1380f, -40f),
                new Vector2(1080f, DH), new Vector2(640f, DH),
            }, Pal.A(Pal.Hex("FFC9E4"), 0.09f));

            // marquesina
            c.RoundRect(360f, 600f, 1240f, 810f, 34f, Pal.Hex("FFE197"));
            c.RoundRect(374f, 614f, 1226f, 796f, 26f, Pal.Hex("8A3A6B"));
            for (int i = 0; i < 18; i++)
            {
                float x = 400f + i * 47f;
                c.Circle(x, 786f, 9f, Pal.A(Pal.Butter, 0.95f));
                c.Circle(x, 624f, 9f, Pal.A(Pal.Butter, 0.95f));
            }
            for (int i = 0; i < 4; i++)
            {
                c.Circle(388f, 660f + i * 34f, 9f, Pal.A(Pal.Butter, 0.95f));
                c.Circle(1212f, 660f + i * 34f, 9f, Pal.A(Pal.Butter, 0.95f));
            }

            // estrellas grandes en la marquesina
            for (int i = 0; i < 5; i++)
            {
                float x = 500f + i * 150f;
                c.Star(x, 706f, 44f, 19f, 5, 90f, Pal.A(Pal.Butter, 0.9f));
                c.Star(x, 706f, 24f, 10f, 5, 90f, Pal.Hex("8A3A6B"));
            }

            // cortinas
            c.Poly(new[]
            {
                new Vector2(0f, DH), new Vector2(300f, DH),
                new Vector2(190f, 0f), new Vector2(0f, 0f),
            }, Pal.Hex("8A2B4E"));
            c.Poly(new[]
            {
                new Vector2(DW, DH), new Vector2(DW - 300f, DH),
                new Vector2(DW - 190f, 0f), new Vector2(DW, 0f),
            }, Pal.Hex("8A2B4E"));
            for (int i = 0; i < 5; i++)
            {
                float x = 40f + i * 52f;
                c.Poly(new[]
                {
                    new Vector2(x, DH), new Vector2(x + 26f, DH), new Vector2(x - 22f, 0f), new Vector2(x - 48f, 0f),
                }, Pal.A(Pal.Hex("A93B62"), 0.7f));
                c.Poly(new[]
                {
                    new Vector2(DW - x, DH), new Vector2(DW - x - 26f, DH),
                    new Vector2(DW - x + 22f, 0f), new Vector2(DW - x + 48f, 0f),
                }, Pal.A(Pal.Hex("A93B62"), 0.7f));
            }

            // alfombra roja
            c.Poly(new[]
            {
                new Vector2(520f, 0f), new Vector2(1080f, 0f),
                new Vector2(880f, 300f), new Vector2(720f, 300f),
            }, Pal.Hex("C0334F"));
            c.Poly(new[]
            {
                new Vector2(600f, 0f), new Vector2(1000f, 0f),
                new Vector2(860f, 300f), new Vector2(740f, 300f),
            }, Pal.A(Pal.Hex("D94963"), 0.75f));

            SparkleField(c, 40, 100f, 850f, Pal.Butter, 511);
            Vignette(c, 0.55f);
        }

        // ----------------------------------------------------------- 4 · HIELO

        static void Hielo(Canvas2D c)
        {
            c.MultiGradient(Pal.Hex("F0FBFF"), Pal.Hex("CDEBFA"), Pal.Hex("A9DCF2"), 0.55f);

            // palacio lejano
            for (int i = 0; i < 3; i++)
            {
                float x = 300f + i * 500f;
                c.Poly(new[]
                {
                    new Vector2(x - 90f, 260f), new Vector2(x + 90f, 260f),
                    new Vector2(x, 470f),
                }, Pal.A(Pal.White, 0.85f));
                c.Poly(new[]
                {
                    new Vector2(x - 60f, 300f), new Vector2(x + 60f, 300f),
                    new Vector2(x, 430f),
                }, Pal.A(Pal.Hex("D9F1FF"), 0.9f));
            }

            // árboles nevados
            for (int i = 0; i < 8; i++)
            {
                float x = 90f + i * 200f;
                float h = 130f + (i % 3) * 40f;
                c.Poly(new[]
                {
                    new Vector2(x - 60f, 240f), new Vector2(x + 60f, 240f), new Vector2(x, 240f + h),
                }, Pal.Hex("7FBFA8"));
                c.Poly(new[]
                {
                    new Vector2(x - 34f, 240f + h * 0.55f), new Vector2(x + 34f, 240f + h * 0.55f), new Vector2(x, 240f + h),
                }, Pal.White);
            }

            // pista
            c.Ellipse(DW * 0.5f, 150f, 860f, 220f, Pal.Hex("BFE6F8"));
            c.Ellipse(DW * 0.5f, 158f, 800f, 196f, Pal.Hex("DFF4FF"));
            c.EllipseRing(DW * 0.5f, 150f, 860f, 220f, 8f, Pal.A(Pal.White, 0.85f));
            for (int i = 0; i < 5; i++)
            {
                float y = 90f + i * 46f;
                c.Stroke(new[]
                {
                    new Vector2(300f + i * 40f, y),
                    new Vector2(700f + i * 30f, y + 16f),
                    new Vector2(1100f - i * 30f, y),
                }, 3f, Pal.A(Pal.White, 0.7f));
            }

            // guirnalda de luces
            Vector2 prev2 = new Vector2(-20f, 800f);
            for (int i = 1; i <= 20; i++)
            {
                float t = i / 20f;
                Vector2 p = new Vector2(Mathf.Lerp(-20f, DW + 20f, t), 800f - Mathf.Sin(t * Mathf.PI) * 90f);
                c.Line(prev2, p, 4f, Pal.A(Pal.Hex("6B4C67"), 0.65f));
                if (i % 2 == 0)
                {
                    Color c1 = i % 4 == 0 ? Pal.Hex("FFE197") : Pal.Hex("FFB3D1");
                    c.Glow(p.x, p.y - 12f, 44f, Pal.A(c1, 0.5f));
                    c.Circle(p.x, p.y - 12f, 11f, c1);
                }
                prev2 = p;
            }

            // copos
            Random.State prev = Random.state;
            Random.InitState(601);
            for (int i = 0; i < 26; i++)
            {
                Snowflake(c, Random.Range(20f, DW - 20f), Random.Range(200f, 850f), Random.Range(8f, 22f), Pal.A(Pal.White, Random.Range(0.4f, 0.9f)));
            }
            Random.state = prev;

            SparkleField(c, 24, 200f, 860f, Pal.White, 611);
            Vignette(c, 0.3f);
        }
    }
}
