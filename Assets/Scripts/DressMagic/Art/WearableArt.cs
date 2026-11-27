using UnityEngine;
using DressMagic.Data;

namespace DressMagic.Art
{
    /// <summary>Dibuja cada prenda en el espacio de diseño de la muñeca (512 x 1024).</summary>
    public static class WearableArt
    {
        static readonly Color Gold = Pal.Hex("FFD25E");
        static readonly Color GoldDeep = Pal.Hex("E8A92E");
        static readonly Color Silver = Pal.Hex("E9EEF8");
        static readonly Color SilverDeep = Pal.Hex("B9C6DC");
        static readonly Color Shine = Pal.Hex("FFFFFF");
        static readonly Color Straw = Pal.Hex("F2D79B");

        static Color L(Color c) => Pal.Lighten(c, 0.34f);
        static Color D(Color c) => Pal.Darken(c, 0.24f);

        public static bool Draw(Canvas2D c, string id, Color main)
        {
            switch (id)
            {
                case "vestido_princesa": DressPrincess(c, main); return true;
                case "vestido_tutu": DressTutu(c, main); return true;
                case "vestido_pichi": DressPinafore(c, main); return true;
                case "vestido_fiesta": DressParty(c, main); return true;
                case "vestido_mariposa": DressButterfly(c, main); return true;
                case "vestido_verano": DressSummer(c, main); return true;

                case "blusa_lunar": TopSweater(c, main); return true;
                case "blusa_nube": TopCloud(c, main); return true;
                case "top_mariposa": TopButterfly(c, main); return true;
                case "blusa_lazo": TopBow(c, main); return true;

                case "pantalon_leggings": Pants(c, main, 700f, 55f, 0); return true;
                case "pantalon_mezclilla": Pants(c, main, 706f, 62f, 1); return true;
                case "pantalon_corto": Shorts(c, main); return true;
                case "mallas_estrella": Pants(c, main, 696f, 52f, 2); return true;

                case "falda_tutu": SkirtTutu(c, main); return true;
                case "falda_plisada": SkirtPleated(c, main); return true;
                case "falda_estrellas": SkirtStars(c, main); return true;

                case "zapatos_merceditas": Flats(c, main, 0); return true;
                case "zapatos_deportivos": Flats(c, main, 1); return true;
                case "zapatos_magicos": Flats(c, main, 2); return true;
                case "botas_invierno": Boots(c, main); return true;
                case "patines_hielo": Skates(c); return true;
                case "tacones_cristal": Heels(c, Silver, true); return true;
                case "tacones_rosa": Heels(c, main, false); return true;

                case "diadema_flores": HeadbandFlowers(c); return true;
                case "diadema_perlas": HeadbandPearls(c); return true;
                case "corona_dorada": Crown(c, Gold, false); return true;
                case "corona_corazon": Crown(c, main, true); return true;
                case "sombrero_invierno": Beanie(c, main); return true;
                case "sombrero_paja": StrawHat(c); return true;

                case "bolso_conchas": Bag(c, main, 0); return true;
                case "bolso_corazon": Bag(c, main, 1); return true;

                case "varita_luna": Wand(c, 0); return true;
                case "varita_estrella": Wand(c, 1); return true;

                case "corona_estelar": CrownStars(c); return true;
                case "tiara_estrellas": Tiara(c); return true;
                case "zapatillas_magicas": MagicSlippers(c); return true;
                case "bolso_magico": MagicBag(c); return true;
                case "vestido_magico": DressMagic(c); return true;
                case "capa_magica": Cape(c); return true;
                case "varita_arcana": WandArcane(c); return true;
                case "broche_estrella": Brooch(c); return true;
            }
            return false;
        }

        // ------------------------------------------------------------- helpers

        static void OP(Canvas2D c, Vector2[] pts, Color fill, float ow = 5f)
        {
            c.Stroke(pts, ow * 2f, Pal.Ink, true);
            c.Poly(pts, fill);
        }

        static void OE(Canvas2D c, float cx, float cy, float rx, float ry, Color fill, float ow = 5f)
        {
            c.Ellipse(cx, cy, rx + ow, ry + ow, Pal.Ink);
            c.Ellipse(cx, cy, rx, ry, fill);
        }

        static void Limb(Canvas2D c, Vector2[] pts, float w, Color fill, float ow = 5f)
        {
            c.Stroke(pts, w + ow * 2f, Pal.Ink);
            c.Stroke(pts, w, fill);
        }

        static Vector2[] LeftArm => new[]
        {
            new Vector2(Rig.CX - 62f, 698f),
            new Vector2(Rig.CX - 82f, 610f),
            new Vector2(Rig.CX - 90f, 524f),
        };

        static Vector2[] RightArm => new[]
        {
            new Vector2(Rig.CX + 62f, 698f),
            new Vector2(Rig.CX + 82f, 610f),
            new Vector2(Rig.CX + 90f, 524f),
        };

        static void Sparkles(Canvas2D c, float cx, float cy, float spread, int count, Color col, int seed)
        {
            Random.State prev = Random.state;
            Random.InitState(seed);
            for (int i = 0; i < count; i++)
            {
                float x = cx + Random.Range(-spread, spread);
                float y = cy + Random.Range(-spread * 0.8f, spread * 0.8f);
                float r = Random.Range(4f, 9f);
                c.Star(x, y, r, r * 0.42f, 4, Random.Range(0f, 90f), col);
            }
            Random.state = prev;
        }

        // -------------------------------------------------------------- dresses

        static void DressPrincess(Canvas2D c, Color m)
        {
            Color light = L(m);
            Color dark = D(m);

            OP(c, new[]
            {
                new Vector2(Rig.CX - 52f, 560f),
                new Vector2(Rig.CX + 52f, 560f),
                new Vector2(Rig.CX + 172f, 336f),
                new Vector2(Rig.CX + 96f, 312f),
                new Vector2(Rig.CX + 30f, 330f),
                new Vector2(Rig.CX - 30f, 330f),
                new Vector2(Rig.CX - 96f, 312f),
                new Vector2(Rig.CX - 172f, 336f),
                new Vector2(Rig.CX - 150f, 360f),
            }, m);

            c.Stroke(new[]
            {
                new Vector2(Rig.CX - 120f, 430f),
                new Vector2(Rig.CX - 72f, 520f),
            }, 3f, Pal.A(light, 0.7f));
            c.Stroke(new[]
            {
                new Vector2(Rig.CX + 120f, 430f),
                new Vector2(Rig.CX + 72f, 520f),
            }, 3f, Pal.A(light, 0.7f));

            OP(c, new[]
            {
                new Vector2(Rig.CX - 58f, 706f),
                new Vector2(Rig.CX + 58f, 706f),
                new Vector2(Rig.CX + 54f, 558f),
                new Vector2(Rig.CX - 54f, 558f),
            }, light);

            OE(c, Rig.CX - 66f, 686f, 28f, 26f, light);
            OE(c, Rig.CX + 66f, 686f, 28f, 26f, light);

            c.Rect(Rig.CX - 56f, 552f, Rig.CX + 56f, 578f, dark);
            c.Stroke(new[]
            {
                new Vector2(Rig.CX - 56f, 565f),
                new Vector2(Rig.CX + 56f, 565f),
            }, 4f, Pal.Ink);

            // lazo
            c.Poly(new[]
            {
                new Vector2(Rig.CX, 566f),
                new Vector2(Rig.CX - 30f, 592f),
                new Vector2(Rig.CX - 32f, 542f),
            }, Pal.PinkDeep);
            c.Poly(new[]
            {
                new Vector2(Rig.CX, 566f),
                new Vector2(Rig.CX + 30f, 592f),
                new Vector2(Rig.CX + 32f, 542f),
            }, Pal.PinkDeep);
            OE(c, Rig.CX, 566f, 10f, 10f, Pal.Pink, 4f);

            Sparkles(c, Rig.CX, 400f, 70f, 5, Pal.A(Shine, 0.9f), 11);
        }

        static void DressTutu(Canvas2D c, Color m)
        {
            Color light = L(m);
            OE(c, Rig.CX, 494f, 176f, 96f, Pal.A(Pal.Lighten(m, 0.55f), 0.95f));
            OE(c, Rig.CX, 534f, 152f, 92f, Pal.A(Pal.Lighten(m, 0.45f), 0.96f));
            OE(c, Rig.CX, 574f, 128f, 88f, Pal.A(Pal.Lighten(m, 0.38f), 0.97f));

            c.Stroke(new[]
            {
                new Vector2(Rig.CX - 170f, 470f),
                new Vector2(Rig.CX - 90f, 430f),
                new Vector2(Rig.CX, 418f),
                new Vector2(Rig.CX + 90f, 430f),
                new Vector2(Rig.CX + 170f, 470f),
            }, 3f, Pal.A(D(m), 0.45f));

            OP(c, new[]
            {
                new Vector2(Rig.CX - 58f, 700f),
                new Vector2(Rig.CX + 58f, 700f),
                new Vector2(Rig.CX + 54f, 558f),
                new Vector2(Rig.CX - 54f, 558f),
            }, light);

            c.Rect(Rig.CX - 58f, 552f, Rig.CX + 58f, 574f, D(m));
            c.Stroke(new[]
            {
                new Vector2(Rig.CX - 58f, 563f),
                new Vector2(Rig.CX + 58f, 563f),
            }, 4f, Pal.Ink);

            Sparkles(c, Rig.CX, 460f, 90f, 4, Pal.A(Shine, 0.8f), 23);
        }

        static void DressPinafore(Canvas2D c, Color m)
        {
            Color light = L(m);
            OP(c, new[]
            {
                new Vector2(Rig.CX - 50f, 556f),
                new Vector2(Rig.CX + 50f, 556f),
                new Vector2(Rig.CX + 132f, 372f),
                new Vector2(Rig.CX + 66f, 356f),
                new Vector2(Rig.CX, 368f),
                new Vector2(Rig.CX - 66f, 356f),
                new Vector2(Rig.CX - 132f, 372f),
            }, m);

            OP(c, new[]
            {
                new Vector2(Rig.CX - 48f, 556f),
                new Vector2(Rig.CX + 48f, 556f),
                new Vector2(Rig.CX + 40f, 646f),
                new Vector2(Rig.CX - 40f, 646f),
            }, light);

            c.Stroke(new[] { new Vector2(Rig.CX - 38f, 646f), new Vector2(Rig.CX - 44f, 706f) }, 10f, Pal.Ink);
            c.Stroke(new[] { new Vector2(Rig.CX - 38f, 646f), new Vector2(Rig.CX - 44f, 706f) }, 5f, light);
            c.Stroke(new[] { new Vector2(Rig.CX + 38f, 646f), new Vector2(Rig.CX + 44f, 706f) }, 10f, Pal.Ink);
            c.Stroke(new[] { new Vector2(Rig.CX + 38f, 646f), new Vector2(Rig.CX + 44f, 706f) }, 5f, light);

            c.Star(Rig.CX, 600f, 22f, 10f, 5, 90f, Pal.Butter);
            c.Circle(Rig.CX, 600f, 6f, Pal.White);

            c.Stroke(new[]
            {
                new Vector2(Rig.CX - 110f, 430f),
                new Vector2(Rig.CX - 60f, 520f),
            }, 3f, Pal.A(L(m), 0.6f));
            c.Stroke(new[]
            {
                new Vector2(Rig.CX + 110f, 430f),
                new Vector2(Rig.CX + 60f, 520f),
            }, 3f, Pal.A(L(m), 0.6f));
        }

        static void DressParty(Canvas2D c, Color m)
        {
            Color light = L(m);
            Color dark = D(m);

            OP(c, new[]
            {
                new Vector2(Rig.CX - 50f, 552f),
                new Vector2(Rig.CX + 50f, 552f),
                new Vector2(Rig.CX + 158f, 344f),
                new Vector2(Rig.CX + 74f, 330f),
                new Vector2(Rig.CX, 344f),
                new Vector2(Rig.CX - 74f, 330f),
                new Vector2(Rig.CX - 158f, 344f),
            }, m);

            OP(c, new[]
            {
                new Vector2(Rig.CX - 60f, 660f),
                new Vector2(Rig.CX - 26f, 678f),
                new Vector2(Rig.CX + 26f, 678f),
                new Vector2(Rig.CX + 60f, 660f),
                new Vector2(Rig.CX + 52f, 552f),
                new Vector2(Rig.CX - 52f, 552f),
            }, light);

            c.Stroke(new[]
            {
                new Vector2(Rig.CX - 52f, 620f),
                new Vector2(Rig.CX - 44f, 560f),
            }, 3f, Pal.A(dark, 0.6f));
            c.Stroke(new[]
            {
                new Vector2(Rig.CX + 52f, 620f),
                new Vector2(Rig.CX + 44f, 560f),
            }, 3f, Pal.A(dark, 0.6f));

            c.Rect(Rig.CX - 52f, 546f, Rig.CX + 52f, 566f, Pal.A(Gold, 0.9f));
            for (int i = -2; i <= 2; i++) c.Circle(Rig.CX + i * 22f, 556f, 4f, Pal.A(Shine, 0.9f));

            Sparkles(c, Rig.CX, 430f, 76f, 6, Pal.A(Shine, 0.95f), 47);
        }

        static void DressButterfly(Canvas2D c, Color m)
        {
            Color light = L(m);
            // alas
            for (int s = -1; s <= 1; s += 2)
            {
                Vector2[] wing =
                {
                    new Vector2(Rig.CX + s * 34f, 700f),
                    new Vector2(Rig.CX + s * 96f, 720f),
                    new Vector2(Rig.CX + s * 128f, 672f),
                    new Vector2(Rig.CX + s * 96f, 632f),
                    new Vector2(Rig.CX + s * 40f, 656f),
                };
                OP(c, wing, Pal.A(light, 0.97f));
            }

            OP(c, new[]
            {
                new Vector2(Rig.CX - 50f, 552f),
                new Vector2(Rig.CX + 50f, 552f),
                new Vector2(Rig.CX + 148f, 384f),
                new Vector2(Rig.CX + 72f, 366f),
                new Vector2(Rig.CX, 380f),
                new Vector2(Rig.CX - 72f, 366f),
                new Vector2(Rig.CX - 148f, 384f),
            }, m);

            OP(c, new[]
            {
                new Vector2(Rig.CX - 52f, 700f),
                new Vector2(Rig.CX + 52f, 700f),
                new Vector2(Rig.CX + 46f, 552f),
                new Vector2(Rig.CX - 46f, 552f),
            }, light);

            c.Rect(Rig.CX - 50f, 546f, Rig.CX + 50f, 564f, D(m));

            for (int s = -1; s <= 1; s += 2)
            {
                c.Circle(Rig.CX + s * 96f, 676f, 12f, Pal.A(Pal.Butter, 0.85f));
                c.Circle(Rig.CX + s * 96f, 676f, 12f, Pal.A(Gold, 0.55f));
            }
        }

        static void DressSummer(Canvas2D c, Color m)
        {
            Color light = L(m);
            OP(c, new[]
            {
                new Vector2(Rig.CX - 52f, 560f),
                new Vector2(Rig.CX + 52f, 560f),
                new Vector2(Rig.CX + 122f, 396f),
                new Vector2(Rig.CX + 58f, 382f),
                new Vector2(Rig.CX, 392f),
                new Vector2(Rig.CX - 58f, 382f),
                new Vector2(Rig.CX - 122f, 396f),
            }, m);

            OP(c, new[]
            {
                new Vector2(Rig.CX - 52f, 560f),
                new Vector2(Rig.CX + 52f, 560f),
                new Vector2(Rig.CX + 44f, 648f),
                new Vector2(Rig.CX - 44f, 648f),
            }, light);

            c.Stroke(new[] { new Vector2(Rig.CX - 42f, 648f), new Vector2(Rig.CX - 46f, 702f) }, 9f, Pal.Ink);
            c.Stroke(new[] { new Vector2(Rig.CX - 42f, 648f), new Vector2(Rig.CX - 46f, 702f) }, 4f, light);
            c.Stroke(new[] { new Vector2(Rig.CX + 42f, 648f), new Vector2(Rig.CX + 46f, 702f) }, 9f, Pal.Ink);
            c.Stroke(new[] { new Vector2(Rig.CX + 42f, 648f), new Vector2(Rig.CX + 46f, 702f) }, 4f, light);

            c.Rect(Rig.CX - 54f, 552f, Rig.CX + 54f, 570f, D(m));
            OE(c, Rig.CX, 561f, 9f, 9f, Pal.Butter, 3f);

            for (int i = -1; i <= 1; i++)
            {
                float fx = Rig.CX + i * 62f;
                float fy = 470f - Mathf.Abs(i) * 26f;
                for (int p = 0; p < 5; p++)
                {
                    float a = p / 5f * Mathf.PI * 2f;
                    c.Circle(fx + Mathf.Cos(a) * 7f, fy + Mathf.Sin(a) * 7f, 5.5f, Pal.White);
                }
                c.Circle(fx, fy, 5f, Pal.Butter);
            }
        }

        // ---------------------------------------------------------------- tops

        static void TopSweater(Canvas2D c, Color m)
        {
            Color light = L(m);
            OP(c, new[]
            {
                new Vector2(Rig.CX - 62f, 710f),
                new Vector2(Rig.CX + 62f, 710f),
                new Vector2(Rig.CX + 58f, 542f),
                new Vector2(Rig.CX - 58f, 542f),
            }, m);

            Limb(c, LeftArm, 44f, m);
            Limb(c, RightArm, 44f, m);

            c.Rect(Rig.CX - 58f, 542f, Rig.CX + 58f, 558f, D(m));
            c.Stroke(new[]
            {
                new Vector2(Rig.CX - 60f, 700f),
                new Vector2(Rig.CX + 60f, 700f),
            }, 3f, Pal.A(light, 0.7f));

            OE(c, Rig.CX - 42f, 632f, 17f, 17f, Pal.A(Pal.Butter, 0.9f), 3f);
            c.Circle(Rig.CX - 35f, 632f, 14f, m);
        }

        static void TopCloud(Canvas2D c, Color m)
        {
            Color light = L(m);
            OE(c, Rig.CX - 68f, 690f, 32f, 30f, light);
            OE(c, Rig.CX + 68f, 690f, 32f, 30f, light);

            OP(c, new[]
            {
                new Vector2(Rig.CX - 54f, 704f),
                new Vector2(Rig.CX + 54f, 704f),
                new Vector2(Rig.CX + 48f, 552f),
                new Vector2(Rig.CX - 48f, 552f),
            }, m);

            c.Stroke(new[]
            {
                new Vector2(Rig.CX - 54f, 620f),
                new Vector2(Rig.CX + 54f, 620f),
            }, 3f, Pal.A(Pal.White, 0.7f));

            c.Circle(Rig.CX - 16f, 660f, 12f, Pal.White);
            c.Circle(Rig.CX + 2f, 668f, 15f, Pal.White);
            c.Circle(Rig.CX + 20f, 658f, 11f, Pal.White);
        }

        static void TopButterfly(Canvas2D c, Color m)
        {
            Color light = L(m);
            OP(c, new[]
            {
                new Vector2(Rig.CX - 56f, 668f),
                new Vector2(Rig.CX + 56f, 668f),
                new Vector2(Rig.CX + 50f, 552f),
                new Vector2(Rig.CX - 50f, 552f),
            }, light);

            for (int s = -1; s <= 1; s += 2)
            {
                OP(c, new[]
                {
                    new Vector2(Rig.CX + s * 42f, 676f),
                    new Vector2(Rig.CX + s * 92f, 692f),
                    new Vector2(Rig.CX + s * 84f, 646f),
                    new Vector2(Rig.CX + s * 44f, 654f),
                }, m);
            }

            c.Stroke(new[]
            {
                new Vector2(Rig.CX - 56f, 668f),
                new Vector2(Rig.CX, 656f),
                new Vector2(Rig.CX + 56f, 668f),
            }, 6f, D(m));

            c.Heart(Rig.CX, 610f, 22f, Pal.PinkDeep);
        }

        static void TopBow(Canvas2D c, Color m)
        {
            Color light = L(m);
            OP(c, new[]
            {
                new Vector2(Rig.CX - 56f, 706f),
                new Vector2(Rig.CX + 56f, 706f),
                new Vector2(Rig.CX + 50f, 556f),
                new Vector2(Rig.CX - 50f, 556f),
            }, m);

            OE(c, Rig.CX - 64f, 692f, 24f, 22f, light, 4f);
            OE(c, Rig.CX + 64f, 692f, 24f, 22f, light, 4f);

            c.Poly(new[]
            {
                new Vector2(Rig.CX, 690f),
                new Vector2(Rig.CX - 34f, 714f),
                new Vector2(Rig.CX - 36f, 664f),
            }, Pal.PinkDeep);
            c.Poly(new[]
            {
                new Vector2(Rig.CX, 690f),
                new Vector2(Rig.CX + 34f, 714f),
                new Vector2(Rig.CX + 36f, 664f),
            }, Pal.PinkDeep);
            OE(c, Rig.CX, 690f, 10f, 10f, Pal.Pink, 3f);

            for (int i = 0; i < 3; i++) OE(c, Rig.CX, 640f - i * 24f, 5f, 5f, Pal.Butter, 2f);
        }

        // --------------------------------------------------------------- pants

        static void Pants(Canvas2D c, Color m, float topY, float width, int pattern)
        {
            Color dark = D(m);
            OP(c, new[]
            {
                new Vector2(Rig.CX - 62f, topY),
                new Vector2(Rig.CX + 62f, topY),
                new Vector2(Rig.CX + 60f, Rig.HipY),
                new Vector2(Rig.CX - 60f, Rig.HipY),
            }, m);

            for (int s = -1; s <= 1; s += 2)
            {
                Vector2[] leg =
                {
                    new Vector2(Rig.CX + s * Rig.LegX, Rig.HipY + 6f),
                    new Vector2(Rig.CX + s * (Rig.LegX + 3f), (Rig.HipY + Rig.AnkleY) * 0.5f),
                    new Vector2(Rig.CX + s * (Rig.LegX + 6f), Rig.AnkleY - 4f),
                };
                Limb(c, leg, width, m, 5f);
            }

            c.Rect(Rig.CX - 62f, topY - 16f, Rig.CX + 62f, topY, dark);
            c.Stroke(new[] { new Vector2(Rig.CX - 62f, topY - 8f), new Vector2(Rig.CX + 62f, topY - 8f) }, 3f, Pal.Ink);

            if (pattern == 1)
            {
                c.Stroke(new[] { new Vector2(Rig.CX - 20f, Rig.HipY + 6f), new Vector2(Rig.CX - 20f, 300f) }, 2.5f, Pal.A(dark, 0.7f));
                c.Stroke(new[] { new Vector2(Rig.CX + 20f, Rig.HipY + 6f), new Vector2(Rig.CX + 20f, 300f) }, 2.5f, Pal.A(dark, 0.7f));
            }
            else if (pattern == 2)
            {
                for (int i = 0; i < 5; i++)
                {
                    float y = 240f + i * 44f;
                    c.Star(Rig.CX - Rig.LegX - 4f, y, 7f, 3f, 5, 90f, Pal.Butter);
                    c.Star(Rig.CX + Rig.LegX + 4f, y + 22f, 7f, 3f, 5, 90f, Pal.Butter);
                }
            }
        }

        static void Shorts(Canvas2D c, Color m)
        {
            OP(c, new[]
            {
                new Vector2(Rig.CX - 64f, 700f),
                new Vector2(Rig.CX + 64f, 700f),
                new Vector2(Rig.CX + 66f, 420f),
                new Vector2(Rig.CX + 14f, 420f),
                new Vector2(Rig.CX + 10f, 460f),
                new Vector2(Rig.CX - 10f, 460f),
                new Vector2(Rig.CX - 14f, 420f),
                new Vector2(Rig.CX - 66f, 420f),
            }, m);

            c.Rect(Rig.CX - 64f, 684f, Rig.CX + 64f, 700f, D(m));
            c.Stroke(new[] { new Vector2(Rig.CX - 64f, 692f), new Vector2(Rig.CX + 64f, 692f) }, 3f, Pal.Ink);
            c.Stroke(new[] { new Vector2(Rig.CX - 62f, 430f), new Vector2(Rig.CX + 62f, 430f) }, 2.5f, Pal.A(D(m), 0.8f));
        }

        // -------------------------------------------------------------- skirts

        static void SkirtTutu(Canvas2D c, Color m)
        {
            OE(c, Rig.CX, 486f, 168f, 92f, Pal.A(Pal.Lighten(m, 0.55f), 0.95f));
            OE(c, Rig.CX, 524f, 148f, 88f, Pal.A(Pal.Lighten(m, 0.45f), 0.96f));
            OE(c, Rig.CX, 560f, 122f, 82f, Pal.A(Pal.Lighten(m, 0.38f), 0.97f));
            c.Rect(Rig.CX - 60f, 566f, Rig.CX + 60f, 588f, D(m));
            c.Stroke(new[] { new Vector2(Rig.CX - 60f, 577f), new Vector2(Rig.CX + 60f, 577f) }, 4f, Pal.Ink);
        }

        static void SkirtPleated(Canvas2D c, Color m)
        {
            OP(c, new[]
            {
                new Vector2(Rig.CX - 58f, 580f),
                new Vector2(Rig.CX + 58f, 580f),
                new Vector2(Rig.CX + 140f, 356f),
                new Vector2(Rig.CX + 60f, 344f),
                new Vector2(Rig.CX, 356f),
                new Vector2(Rig.CX - 60f, 344f),
                new Vector2(Rig.CX - 140f, 356f),
            }, m);

            for (int i = -2; i <= 2; i++)
            {
                c.Stroke(new[]
                {
                    new Vector2(Rig.CX + i * 24f, 578f),
                    new Vector2(Rig.CX + i * 56f, 356f),
                }, 2.5f, Pal.A(D(m), 0.55f));
            }

            c.Rect(Rig.CX - 60f, 574f, Rig.CX + 60f, 596f, D(m));
            c.Stroke(new[] { new Vector2(Rig.CX - 60f, 585f), new Vector2(Rig.CX + 60f, 585f) }, 4f, Pal.Ink);
        }

        static void SkirtStars(Canvas2D c, Color m)
        {
            OP(c, new[]
            {
                new Vector2(Rig.CX - 58f, 580f),
                new Vector2(Rig.CX + 58f, 580f),
                new Vector2(Rig.CX + 152f, 366f),
                new Vector2(Rig.CX + 76f, 352f),
                new Vector2(Rig.CX, 366f),
                new Vector2(Rig.CX - 76f, 352f),
                new Vector2(Rig.CX - 152f, 366f),
            }, m);

            Sparkles(c, Rig.CX, 466f, 76f, 7, Pal.A(Pal.Butter, 0.95f), 71);
            c.Rect(Rig.CX - 58f, 574f, Rig.CX + 58f, 594f, D(m));
            c.Stroke(new[] { new Vector2(Rig.CX - 58f, 584f), new Vector2(Rig.CX + 58f, 584f) }, 4f, Pal.Ink);
        }

        // --------------------------------------------------------------- shoes

        static void Flats(Canvas2D c, Color m, int style)
        {
            Color light = L(m);
            for (int s = -1; s <= 1; s += 2)
            {
                float fx = Rig.CX + s * (Rig.LegX + 14f);
                float fy = Rig.FootY + 20f;

                if (style == 1)
                {
                    OE(c, fx, fy, 47f, 27f, m);
                    c.Rect(fx - 46f, fy - 26f, fx + 46f, fy - 14f, Pal.White);
                    c.Stroke(new[] { new Vector2(fx - 20f, fy + 2f), new Vector2(fx + 16f, fy + 6f) }, 3f, Pal.White);
                    c.Stroke(new[] { new Vector2(fx - 18f, fy + 12f), new Vector2(fx + 14f, fy + 16f) }, 3f, Pal.White);
                }
                else
                {
                    OE(c, fx, fy, 45f, 26f, m);
                    c.Stroke(new[]
                    {
                        new Vector2(fx - 30f, fy + 22f),
                        new Vector2(fx, fy + 34f),
                        new Vector2(fx + 30f, fy + 22f),
                    }, 6f, D(m));
                    c.EllipseRing(fx, fy + 6f, 30f, 14f, 2f, Pal.A(light, 0.8f));
                }

                if (style == 2)
                {
                    c.Star(fx, fy + 6f, 13f, 6f, 5, 90f, Pal.Butter);
                    c.Star(fx, fy + 6f, 13f, 6f, 5, 90f, Pal.A(Gold, 0.75f));
                    c.Circle(fx - 26f, fy + 18f, 4f, Pal.White);
                }
            }
        }

        static void Boots(Canvas2D c, Color m)
        {
            Color light = L(m);
            for (int s = -1; s <= 1; s += 2)
            {
                float fx = Rig.CX + s * (Rig.LegX + 12f);
                OP(c, new[]
                {
                    new Vector2(fx - 34f, 312f),
                    new Vector2(fx + 34f, 312f),
                    new Vector2(fx + 38f, 118f),
                    new Vector2(fx + 52f, 96f),
                    new Vector2(fx - 52f, 96f),
                    new Vector2(fx - 38f, 118f),
                }, m);

                c.Rect(fx - 36f, 290f, fx + 36f, 318f, Pal.White);
                c.Stroke(new[] { new Vector2(fx - 36f, 304f), new Vector2(fx + 36f, 304f) }, 3f, Pal.A(D(m), 0.8f));
                c.Rect(fx - 52f, 96f, fx + 52f, 112f, D(m));
                c.Stroke(new[] { new Vector2(fx - 34f, 118f), new Vector2(fx + 34f, 118f) }, 3f, Pal.Ink);
                c.Circle(fx + 20f, 250f, 8f, Pal.Butter);
            }
        }

        static void Skates(Canvas2D c)
        {
            for (int s = -1; s <= 1; s += 2)
            {
                float fx = Rig.CX + s * (Rig.LegX + 12f);
                OP(c, new[]
                {
                    new Vector2(fx - 32f, 240f),
                    new Vector2(fx + 32f, 240f),
                    new Vector2(fx + 36f, 112f),
                    new Vector2(fx + 50f, 94f),
                    new Vector2(fx - 50f, 94f),
                    new Vector2(fx - 36f, 112f),
                }, Pal.White);

                c.Rect(fx - 34f, 222f, fx + 34f, 242f, Pal.Pink);
                c.Stroke(new[] { new Vector2(fx - 34f, 232f), new Vector2(fx + 34f, 232f) }, 3f, Pal.A(Pal.Ink, 0.6f));

                c.Rect(fx - 46f, 76f, fx + 46f, 90f, Pal.Hex("DCE7F5"));
                c.Stroke(new[] { new Vector2(fx - 46f, 83f), new Vector2(fx + 46f, 83f) }, 4f, SilverDeep);
                c.Stroke(new[] { new Vector2(fx - 20f, 92f), new Vector2(fx - 20f, 76f) }, 5f, SilverDeep);
                c.Stroke(new[] { new Vector2(fx + 20f, 92f), new Vector2(fx + 20f, 76f) }, 5f, SilverDeep);
            }
        }

        static void Heels(Canvas2D c, Color m, bool crystal)
        {
            for (int s = -1; s <= 1; s += 2)
            {
                float fx = Rig.CX + s * (Rig.LegX + 14f);
                Color fill = crystal ? Pal.A(Pal.Lighten(m, 0.55f), 0.92f) : m;

                OP(c, new[]
                {
                    new Vector2(fx - 42f, 128f),
                    new Vector2(fx + 40f, 122f),
                    new Vector2(fx + 26f, 104f),
                    new Vector2(fx - 34f, 106f),
                }, fill);

                c.Stroke(new[] { new Vector2(fx + 24f, 108f), new Vector2(fx + 30f, 78f) }, 2f, Pal.Ink);
                c.Poly(new[]
                {
                    new Vector2(fx + 22f, 110f),
                    new Vector2(fx + 32f, 110f),
                    new Vector2(fx + 30f, 78f),
                    new Vector2(fx + 24f, 78f),
                }, fill);

                c.Stroke(new[]
                {
                    new Vector2(fx - 34f, 126f),
                    new Vector2(fx - 6f, 150f),
                    new Vector2(fx + 26f, 140f),
                }, 5f, crystal ? Pal.A(Pal.White, 0.85f) : D(m));

                if (crystal) c.Star(fx - 6f, 126f, 9f, 4f, 5, 90f, Pal.A(Pal.White, 0.9f));
                else c.Star(fx - 6f, 124f, 9f, 4f, 5, 90f, Pal.Butter);
            }
        }

        // ---------------------------------------------------------------- head

        static void HeadbandFlowers(Canvas2D c)
        {
            Vector2[] arc = new Vector2[9];
            for (int i = 0; i < 9; i++)
            {
                float t = i / 8f;
                float a = Mathf.Lerp(200f, 340f, t) * Mathf.Deg2Rad;
                arc[i] = new Vector2(Rig.CX + Mathf.Cos(a) * (Rig.HeadRX + 4f), Rig.HeadCY + 44f + Mathf.Sin(a) * (Rig.HeadRY + 4f) * 0.55f);
            }
            c.Stroke(arc, 15f, Pal.Ink);
            c.Stroke(arc, 8f, Pal.Pink);

            for (int i = 0; i < 5; i++)
            {
                float t = i / 4f;
                float a = Mathf.Lerp(205f, 335f, t) * Mathf.Deg2Rad;
                float x = Rig.CX + Mathf.Cos(a) * (Rig.HeadRX + 4f);
                float y = Rig.HeadCY + 44f + Mathf.Sin(a) * (Rig.HeadRY + 4f) * 0.55f;
                for (int p = 0; p < 5; p++)
                {
                    float pa = p / 5f * Mathf.PI * 2f;
                    c.Circle(x + Mathf.Cos(pa) * 7f, y + Mathf.Sin(pa) * 7f, 6f, Pal.Ink);
                }
                for (int p = 0; p < 5; p++)
                {
                    float pa = p / 5f * Mathf.PI * 2f;
                    c.Circle(x + Mathf.Cos(pa) * 7f, y + Mathf.Sin(pa) * 7f, 4.5f, i % 2 == 0 ? Pal.White : Pal.Hex("FFD9E6"));
                }
                c.Circle(x, y, 4.5f, Pal.Butter);
            }
        }

        static void HeadbandPearls(Canvas2D c)
        {
            Vector2[] arc = new Vector2[13];
            for (int i = 0; i < 13; i++)
            {
                float t = i / 12f;
                float a = Mathf.Lerp(198f, 342f, t) * Mathf.Deg2Rad;
                arc[i] = new Vector2(Rig.CX + Mathf.Cos(a) * (Rig.HeadRX + 3f), Rig.HeadCY + 40f + Mathf.Sin(a) * (Rig.HeadRY + 3f) * 0.58f);
            }
            c.Stroke(arc, 8f, Pal.Ink);
            c.Stroke(arc, 4f, Pal.PinkLight);

            for (int i = 0; i < arc.Length; i++)
            {
                OE(c, arc[i].x, arc[i].y - 6f, 7f, 7f, Pal.White, 3f);
                c.Circle(arc[i].x - 2f, arc[i].y - 4f, 2.2f, Pal.A(Pal.White, 0.9f));
            }
        }

        static void Crown(Canvas2D c, Color band, bool hearts)
        {
            float baseY = 900f;
            float topY = baseY + 30f;

            for (int i = -2; i <= 2; i++)
            {
                float x = Rig.CX + i * 34f;
                float h = i == 0 ? 108f : (Mathf.Abs(i) == 1 ? 92f : 74f);
                c.Poly(new[]
                {
                    new Vector2(x - 18f, topY),
                    new Vector2(x, topY + h),
                    new Vector2(x + 18f, topY),
                }, Pal.Ink);
                c.Poly(new[]
                {
                    new Vector2(x - 14f, topY),
                    new Vector2(x, topY + h - 5f),
                    new Vector2(x + 14f, topY),
                }, band);
            }

            c.RoundRect(Rig.CX - 96f, baseY - 34f, Rig.CX + 96f, topY, 16f, Pal.Ink);
            c.RoundRect(Rig.CX - 92f, baseY - 30f, Rig.CX + 92f, topY - 4f, 13f, band);

            for (int i = -2; i <= 2; i++)
            {
                float x = Rig.CX + i * 34f;
                float h = i == 0 ? 108f : (Mathf.Abs(i) == 1 ? 92f : 74f);
                if (hearts) c.Heart(x, topY + h - 12f, 13f, Pal.PinkDeep);
                else c.Circle(x, topY + h - 12f, 8f, Pal.PinkDeep);
            }

            for (int i = -2; i <= 2; i++)
            {
                c.Circle(Rig.CX + i * 40f, baseY - 12f, 9f, Pal.Ink);
                c.Circle(Rig.CX + i * 40f, baseY - 12f, 6.5f, i % 2 == 0 ? Pal.Mint : Pal.Sky);
            }
        }

        static void CrownStars(Canvas2D c)
        {
            float baseY = 906f;
            float topY = baseY + 32f;

            for (int i = -2; i <= 2; i++)
            {
                float x = Rig.CX + i * 36f;
                float h = i == 0 ? 112f : (Mathf.Abs(i) == 1 ? 96f : 76f);
                Vector2[] spike =
                {
                    new Vector2(x - 20f, topY),
                    new Vector2(x, topY + h),
                    new Vector2(x + 20f, topY),
                };
                c.Stroke(spike, 10f, Pal.Ink, true);
                c.Poly(spike, Pal.Lilac);
            }

            c.RoundRect(Rig.CX - 100f, baseY - 36f, Rig.CX + 100f, topY, 18f, Pal.Ink);
            c.RoundRect(Rig.CX - 96f, baseY - 32f, Rig.CX + 96f, topY - 4f, 15f, Pal.Lilac);
            c.Rect(Rig.CX - 90f, baseY - 24f, Rig.CX + 90f, baseY - 8f, Pal.A(Gold, 0.85f));

            for (int i = -2; i <= 2; i++)
            {
                float x = Rig.CX + i * 36f;
                float h = i == 0 ? 112f : (Mathf.Abs(i) == 1 ? 96f : 76f);
                c.Star(x, topY + h - 8f, 16f, 7f, 5, 90f, Pal.Butter);
                c.Star(x, topY + h - 8f, 16f, 7f, 5, 90f, Pal.A(Gold, 0.65f));
            }

            for (int i = -2; i <= 2; i++) c.Circle(Rig.CX + i * 40f, baseY - 16f, 7f, Pal.A(Pal.White, 0.9f));
            Sparkles(c, Rig.CX, baseY + 30f, 80f, 5, Pal.A(Pal.White, 0.9f), 91);
        }

        static void Tiara(Canvas2D c)
        {
            Vector2[] arc = new Vector2[11];
            for (int i = 0; i < 11; i++)
            {
                float t = i / 10f;
                float a = Mathf.Lerp(200f, 340f, t) * Mathf.Deg2Rad;
                arc[i] = new Vector2(Rig.CX + Mathf.Cos(a) * (Rig.HeadRX - 4f), Rig.HeadCY + 60f + Mathf.Sin(a) * (Rig.HeadRY - 6f) * 0.5f);
            }
            c.Stroke(arc, 14f, Pal.Ink);
            c.Stroke(arc, 8f, Silver);

            for (int i = 0; i < arc.Length; i += 2)
            {
                float h = i == 5 ? 34f : 20f;
                c.Poly(new[]
                {
                    new Vector2(arc[i].x - 8f, arc[i].y + 4f),
                    new Vector2(arc[i].x, arc[i].y + h),
                    new Vector2(arc[i].x + 8f, arc[i].y + 4f),
                }, Pal.Ink);
                c.Poly(new[]
                {
                    new Vector2(arc[i].x - 5f, arc[i].y + 4f),
                    new Vector2(arc[i].x, arc[i].y + h - 4f),
                    new Vector2(arc[i].x + 5f, arc[i].y + 4f),
                }, Silver);
                c.Star(arc[i].x, arc[i].y + h, 6f, 2.6f, 5, 90f, Pal.Butter);
            }
        }

        static void Beanie(Canvas2D c, Color m)
        {
            c.Ellipse(Rig.CX, Rig.HeadCY + 62f, 116f, 92f, Pal.Ink);
            c.Ellipse(Rig.CX, Rig.HeadCY + 62f, 111f, 87f, m);

            c.RoundRect(Rig.CX - 114f, Rig.HeadCY + 26f, Rig.CX + 114f, Rig.HeadCY + 60f, 18f, Pal.Ink);
            c.RoundRect(Rig.CX - 110f, Rig.HeadCY + 29f, Rig.CX + 110f, Rig.HeadCY + 57f, 15f, Pal.White);

            for (int i = -2; i <= 2; i++)
                c.Stroke(new[] { new Vector2(Rig.CX + i * 22f, Rig.HeadCY + 58f), new Vector2(Rig.CX + i * 24f, Rig.HeadCY + 100f) }, 2.5f, Pal.A(D(m), 0.6f));

            c.Circle(Rig.CX, Rig.HeadCY + 158f, 26f, Pal.Ink);
            c.Circle(Rig.CX, Rig.HeadCY + 158f, 21f, Pal.White);
        }

        static void StrawHat(Canvas2D c)
        {
            c.Ellipse(Rig.CX, Rig.HeadCY + 66f, 172f, 44f, Pal.Ink);
            c.Ellipse(Rig.CX, Rig.HeadCY + 66f, 166f, 39f, Straw);

            c.Ellipse(Rig.CX, Rig.HeadCY + 84f, 96f, 76f, Pal.Ink);
            c.Ellipse(Rig.CX, Rig.HeadCY + 86f, 90f, 71f, Straw);

            c.Rect(Rig.CX - 92f, Rig.HeadCY + 56f, Rig.CX + 92f, Rig.HeadCY + 84f, Pal.Pink);
            c.Stroke(new[] { new Vector2(Rig.CX - 92f, Rig.HeadCY + 70f), new Vector2(Rig.CX + 92f, Rig.HeadCY + 70f) }, 3f, Pal.A(Pal.Ink, 0.5f));

            for (int i = -1; i <= 1; i++)
            {
                c.Star(Rig.CX + i * 34f, Rig.HeadCY + 116f, 9f, 4f, 5, 90f, Pal.A(Pal.White, 0.75f));
            }
        }

        // ----------------------------------------------------------------- bag

        static void Bag(Canvas2D c, Color m, int style)
        {
            float bx = Rig.CX - 148f;
            float by = 420f;

            c.Stroke(new[]
            {
                new Vector2(bx - 22f, by + 44f),
                new Vector2(bx, by + 92f),
                new Vector2(bx + 22f, by + 44f),
            }, 9f, Pal.Ink);
            c.Stroke(new[]
            {
                new Vector2(bx - 22f, by + 44f),
                new Vector2(bx, by + 92f),
                new Vector2(bx + 22f, by + 44f),
            }, 4.5f, D(m));

            if (style == 1)
            {
                c.Heart(bx, by - 4f, 62f, Pal.Ink);
                c.Heart(bx, by - 2f, 54f, m);
                c.Heart(bx - 14f, by + 8f, 14f, Pal.A(Pal.White, 0.6f));
            }
            else
            {
                OP(c, new[]
                {
                    new Vector2(bx - 48f, by + 44f),
                    new Vector2(bx + 48f, by + 44f),
                    new Vector2(bx + 42f, by - 46f),
                    new Vector2(bx - 42f, by - 46f),
                }, m);
                c.Stroke(new[] { new Vector2(bx - 48f, by + 24f), new Vector2(bx + 48f, by + 24f) }, 4f, Pal.A(D(m), 0.8f));
                OE(c, bx, by + 8f, 14f, 14f, Pal.Butter, 3f);
                c.Star(bx, by + 8f, 9f, 4f, 5, 90f, Pal.A(Pal.White, 0.85f));
            }
        }

        static void MagicBag(Canvas2D c)
        {
            float bx = Rig.CX - 148f;
            float by = 424f;

            c.Stroke(new[]
            {
                new Vector2(bx - 24f, by + 44f),
                new Vector2(bx, by + 98f),
                new Vector2(bx + 24f, by + 44f),
            }, 10f, Pal.Ink);
            c.Stroke(new[]
            {
                new Vector2(bx - 24f, by + 44f),
                new Vector2(bx, by + 98f),
                new Vector2(bx + 24f, by + 44f),
            }, 5f, Gold);

            OP(c, new[]
            {
                new Vector2(bx - 50f, by + 46f),
                new Vector2(bx + 50f, by + 46f),
                new Vector2(bx + 44f, by - 50f),
                new Vector2(bx - 44f, by - 50f),
            }, Pal.Hex("9C86E0"));

            c.Rect(bx - 50f, by + 26f, bx + 50f, by + 46f, Pal.A(Gold, 0.9f));
            c.Star(bx, by - 6f, 22f, 9f, 5, 90f, Pal.Butter);
            c.Circle(bx, by - 6f, 7f, Pal.White);
            Sparkles(c, bx, by, 42f, 5, Pal.A(Pal.White, 0.9f), 131);
        }

        // ---------------------------------------------------------------- wand

        static void Wand(Canvas2D c, int style)
        {
            Vector2 grip = new Vector2(Rig.CX + 90f, 512f);
            Vector2 tip = new Vector2(Rig.CX + 140f, 664f);

            c.Stroke(new[] { grip, tip }, 14f, Pal.Ink);
            c.Stroke(new[] { grip, tip }, 8f, style == 0 ? Pal.Lilac : Pal.White);

            if (style == 0)
            {
                c.Ellipse(tip.x, tip.y, 26f, 26f, Pal.Ink);
                c.Ellipse(tip.x, tip.y, 21f, 21f, Pal.Butter);
                c.Ellipse(tip.x + 8f, tip.y + 4f, 17f, 18f, Pal.A(Pal.White, 0f));
                c.Ellipse(tip.x + 10f, tip.y + 2f, 15f, 15f, Pal.Hex("A7D8F7"));
            }
            else
            {
                c.Star(tip.x, tip.y, 34f, 15f, 5, 90f, Pal.Ink);
                c.Star(tip.x, tip.y, 29f, 12f, 5, 90f, Pal.Butter);
                c.Star(tip.x, tip.y, 15f, 6f, 5, 90f, Pal.White);
            }

            c.Glow(tip.x, tip.y, 62f, Pal.A(Pal.Butter, 0.35f));
        }

        static void WandArcane(Canvas2D c)
        {
            Vector2 grip = new Vector2(Rig.CX + 90f, 508f);
            Vector2 tip = new Vector2(Rig.CX + 144f, 676f);

            c.Stroke(new[] { grip, tip }, 16f, Pal.Ink);
            c.Stroke(new[] { grip, tip }, 10f, Gold);
            c.Stroke(new[] { grip, grip + (tip - grip) * 0.35f }, 12f, Pal.Hex("8A76D6"));

            c.Glow(tip.x, tip.y, 96f, Pal.A(Pal.Lilac, 0.45f));
            c.Star(tip.x, tip.y, 46f, 20f, 5, 90f, Pal.Ink);
            c.Star(tip.x, tip.y, 40f, 16f, 5, 90f, Pal.Lilac);
            c.Star(tip.x, tip.y, 22f, 9f, 5, 90f, Pal.White);

            for (int i = 0; i < 3; i++)
            {
                float a = 40f + i * 120f;
                c.Circle(tip.x + Mathf.Cos(a * Mathf.Deg2Rad) * 54f, tip.y + Mathf.Sin(a * Mathf.Deg2Rad) * 54f, 5f, Pal.A(Pal.Butter, 0.9f));
            }
        }

        // --------------------------------------------------------------- cape

        static void Cape(Canvas2D c)
        {
            OP(c, new[]
            {
                new Vector2(Rig.CX - 74f, 716f),
                new Vector2(Rig.CX + 74f, 716f),
                new Vector2(Rig.CX + 186f, 300f),
                new Vector2(Rig.CX + 96f, 268f),
                new Vector2(Rig.CX, 286f),
                new Vector2(Rig.CX - 96f, 268f),
                new Vector2(Rig.CX - 186f, 300f),
            }, Pal.Hex("7E6BC8"));

            c.Stroke(new[] { new Vector2(Rig.CX - 74f, 700f), new Vector2(Rig.CX + 74f, 700f) }, 20f, Pal.Ink);
            c.Stroke(new[] { new Vector2(Rig.CX - 74f, 700f), new Vector2(Rig.CX + 74f, 700f) }, 12f, Pal.A(Gold, 0.9f));

            Sparkles(c, Rig.CX, 480f, 110f, 9, Pal.A(Pal.Butter, 0.9f), 173);
            c.Rect(Rig.CX - 188f, 292f, Rig.CX + 188f, 306f, Pal.A(Gold, 0.55f));
        }

        static void Brooch(Canvas2D c)
        {
            c.Star(Rig.CX, 700f, 26f, 11f, 5, 90f, Pal.Ink);
            c.Star(Rig.CX, 700f, 21f, 9f, 5, 90f, Pal.Butter);
            c.Circle(Rig.CX, 700f, 7f, Pal.PinkDeep);
            c.Glow(Rig.CX, 700f, 48f, Pal.A(Pal.Butter, 0.35f));
        }

        static void MagicSlippers(Canvas2D c)
        {
            for (int s = -1; s <= 1; s += 2)
            {
                float fx = Rig.CX + s * (Rig.LegX + 14f);
                float fy = Rig.FootY + 20f;
                OE(c, fx, fy, 46f, 27f, Pal.Hex("A7D8F7"));

                c.Stroke(new[]
                {
                    new Vector2(fx - 30f, fy + 22f),
                    new Vector2(fx, fy + 36f),
                    new Vector2(fx + 30f, fy + 22f),
                }, 6f, Pal.A(Gold, 0.95f));

                c.Star(fx, fy + 8f, 15f, 6f, 5, 90f, Pal.Butter);
                c.Star(fx, fy + 8f, 15f, 6f, 5, 90f, Pal.A(Pal.White, 0.6f));
                c.Glow(fx, fy + 8f, 40f, Pal.A(Pal.Butter, 0.35f));
            }
        }

        static void DressMagic(Canvas2D c)
        {
            Color a = Pal.Hex("C9B2F0");
            Color b = Pal.Hex("A7D8F7");

            OE(c, Rig.CX, 486f, 190f, 104f, Pal.A(b, 0.95f));
            OE(c, Rig.CX, 534f, 164f, 96f, Pal.A(Pal.Mix(a, b, 0.5f), 0.96f));
            OE(c, Rig.CX, 580f, 136f, 90f, Pal.A(a, 0.97f));

            OP(c, new[]
            {
                new Vector2(Rig.CX - 60f, 706f),
                new Vector2(Rig.CX + 60f, 706f),
                new Vector2(Rig.CX + 56f, 552f),
                new Vector2(Rig.CX - 56f, 552f),
            }, Pal.Hex("EFE4FF"));

            OE(c, Rig.CX - 70f, 690f, 32f, 30f, Pal.Hex("EFE4FF"));
            OE(c, Rig.CX + 70f, 690f, 32f, 30f, Pal.Hex("EFE4FF"));

            c.Rect(Rig.CX - 58f, 546f, Rig.CX + 58f, 574f, Pal.A(Gold, 0.9f));
            c.Star(Rig.CX, 560f, 20f, 8f, 5, 90f, Pal.Butter);

            Sparkles(c, Rig.CX, 470f, 96f, 10, Pal.A(Pal.White, 0.95f), 211);
            c.Glow(Rig.CX, 500f, 190f, Pal.A(Pal.Lilac, 0.22f));
        }
    }
}
