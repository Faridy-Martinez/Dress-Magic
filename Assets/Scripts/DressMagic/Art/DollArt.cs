using UnityEngine;

namespace DressMagic.Art
{
    /// <summary>Dibuja las cuatro muñecas base (piel, pelo, cara, camisón).</summary>
    public static class DollArt
    {
        public struct DollDef
        {
            public string name;
            public Color skin;
            public Color skinShade;
            public Color hair;
            public Color hairShade;
            public Color eye;
            public int style;
            public bool eyesClosed;
            public bool freckles;
        }

        public static readonly DollDef[] Dolls =
        {
            new DollDef
            {
                name = "Luna", skin = Pal.Hex("F8CDAB"), skinShade = Pal.Hex("E7B189"),
                hair = Pal.Hex("A9744F"), hairShade = Pal.Hex("8A5B3B"),
                eye = Pal.Hex("5A3A22"), style = 0, eyesClosed = false, freckles = false
            },
            new DollDef
            {
                name = "Zoe", skin = Pal.Hex("9C6444"), skinShade = Pal.Hex("84502F"),
                hair = Pal.Hex("2E211D"), hairShade = Pal.Hex("1D1512"),
                eye = Pal.Hex("3A2418"), style = 1, eyesClosed = false, freckles = false
            },
            new DollDef
            {
                name = "Sofía", skin = Pal.Hex("FAD5B6"), skinShade = Pal.Hex("EDBB98"),
                hair = Pal.Hex("F0D488"), hairShade = Pal.Hex("D6B463"),
                eye = Pal.Hex("7A5A2E"), style = 2, eyesClosed = true, freckles = false
            },
            new DollDef
            {
                name = "Nina", skin = Pal.Hex("8A5638"), skinShade = Pal.Hex("6F432A"),
                hair = Pal.Hex("9B8AE0"), hairShade = Pal.Hex("7B69C4"),
                eye = Pal.Hex("3A2418"), style = 3, eyesClosed = true, freckles = true
            },
        };

        public const float Ink = 5f;

        public static void Draw(Canvas2D c, int dollIndex)
        {
            Draw(c, Dolls[Mathf.Clamp(dollIndex, 0, Dolls.Length - 1)]);
        }

        public static void Draw(Canvas2D c, DollDef d)
        {
            DrawHairBack(c, d);

            Limb(c, new[]
            {
                new Vector2(Rig.CX - Rig.LegX, Rig.HipY + 10f),
                new Vector2(Rig.CX - Rig.LegX - 3f, (Rig.HipY + Rig.AnkleY) * 0.5f),
                new Vector2(Rig.CX - Rig.LegX - 6f, Rig.AnkleY + 6f),
            }, Rig.LegR * 2f, d.skin, Ink);

            Limb(c, new[]
            {
                new Vector2(Rig.CX + Rig.LegX, Rig.HipY + 10f),
                new Vector2(Rig.CX + Rig.LegX + 3f, (Rig.HipY + Rig.AnkleY) * 0.5f),
                new Vector2(Rig.CX + Rig.LegX + 6f, Rig.AnkleY + 6f),
            }, Rig.LegR * 2f, d.skin, Ink);

            OutlinedEllipse(c, Rig.CX - Rig.LegX - 14f, Rig.FootY + 24f, 44f, 26f, d.skin);
            OutlinedEllipse(c, Rig.CX + Rig.LegX + 14f, Rig.FootY + 24f, 44f, 26f, d.skin);

            Limb(c, new[]
            {
                new Vector2(Rig.CX - Rig.ShoulderHalf + 4f, Rig.ShoulderY - 8f),
                new Vector2(Rig.CX - Rig.ArmX - 10f, 610f),
                new Vector2(Rig.CX - Rig.ArmX - 16f, Rig.HandY),
            }, Rig.ArmR * 2f, d.skin, Ink);

            Limb(c, new[]
            {
                new Vector2(Rig.CX + Rig.ShoulderHalf - 4f, Rig.ShoulderY - 8f),
                new Vector2(Rig.CX + Rig.ArmX + 10f, 610f),
                new Vector2(Rig.CX + Rig.ArmX + 16f, Rig.HandY),
            }, Rig.ArmR * 2f, d.skin, Ink);

            OutlinedEllipse(c, Rig.CX - Rig.ArmX - 18f, Rig.HandY - 4f, 17f, 19f, d.skin);
            OutlinedEllipse(c, Rig.CX + Rig.ArmX + 18f, Rig.HandY - 4f, 17f, 19f, d.skin);

            Limb(c, new[]
            {
                new Vector2(Rig.CX, Rig.ChestY - 6f),
                new Vector2(Rig.CX, Rig.NeckY + 26f),
            }, 46f, d.skin, Ink);

            OutlinedPoly(c, new[]
            {
                new Vector2(Rig.CX - 56f, Rig.ShoulderY),
                new Vector2(Rig.CX + 56f, Rig.ShoulderY),
                new Vector2(Rig.CX + 52f, 626f),
                new Vector2(Rig.CX + 50f, Rig.WaistY),
                new Vector2(Rig.CX + Rig.HipHalf, Rig.HipY),
                new Vector2(Rig.CX - Rig.HipHalf, Rig.HipY),
                new Vector2(Rig.CX - 50f, Rig.WaistY),
                new Vector2(Rig.CX - 52f, 626f),
            }, d.skin, Ink);

            OutlinedEllipse(c, Rig.CX - Rig.HeadRX + 2f, Rig.HeadCY - 24f, 15f, 21f, d.skin, 4f);
            OutlinedEllipse(c, Rig.CX + Rig.HeadRX - 2f, Rig.HeadCY - 24f, 15f, 21f, d.skin, 4f);

            OutlinedEllipse(c, Rig.CX, Rig.HeadCY, Rig.HeadRX, Rig.HeadRY, d.skin, Ink);

            DrawFace(c, d);
            DrawSlip(c);
            DrawHairFront(c, d);
        }

        static void OutlinedEllipse(Canvas2D c, float cx, float cy, float rx, float ry, Color fill, float ow = Ink)
        {
            c.Ellipse(cx, cy, rx + ow, ry + ow, Pal.Ink);
            c.Ellipse(cx, cy, rx, ry, fill);
        }

        static void Limb(Canvas2D c, Vector2[] pts, float width, Color fill, float ow)
        {
            c.Stroke(pts, width + ow * 2f, Pal.Ink);
            c.Stroke(pts, width, fill);
        }

        static void OutlinedPoly(Canvas2D c, Vector2[] pts, Color fill, float ow = Ink)
        {
            c.Stroke(pts, ow * 2f, Pal.Ink, true);
            c.Poly(pts, fill);
        }

        static void DrawFace(Canvas2D c, DollDef d)
        {
            float eyeY = 818f;
            float dx = 39f;

            c.Ellipse(Rig.CX - 62f, 782f, 21f, 12f, Pal.A(Pal.Hex("F09CA8"), 0.45f));
            c.Ellipse(Rig.CX + 62f, 782f, 21f, 12f, Pal.A(Pal.Hex("F09CA8"), 0.45f));

            if (d.freckles)
            {
                for (int i = 0; i < 4; i++)
                {
                    c.Circle(Rig.CX - 52f - (i % 2) * 13f, 790f - (i / 2) * 9f, 2.4f, Pal.A(Pal.Hex("6B3A22"), 0.55f));
                    c.Circle(Rig.CX + 52f + (i % 2) * 13f, 790f - (i / 2) * 9f, 2.4f, Pal.A(Pal.Hex("6B3A22"), 0.55f));
                }
            }

            for (int s = -1; s <= 1; s += 2)
            {
                float ex = Rig.CX + s * dx;
                c.Stroke(new[]
                {
                    new Vector2(ex - 16f, eyeY + 34f),
                    new Vector2(ex, eyeY + 42f),
                    new Vector2(ex + 16f, eyeY + 35f),
                }, 4.5f, Pal.A(d.hairShade, 0.9f));

                if (d.eyesClosed)
                {
                    c.Stroke(new[]
                    {
                        new Vector2(ex - 17f, eyeY + 2f),
                        new Vector2(ex, eyeY - 10f),
                        new Vector2(ex + 17f, eyeY + 2f),
                    }, 6f, Pal.Ink);
                }
                else
                {
                    c.Ellipse(ex, eyeY, 19f, 22f, Pal.Hex("FFFDFA"));
                    c.Ellipse(ex, eyeY - 1f, 15f, 18f, d.eye);
                    c.Ellipse(ex, eyeY - 9f, 9f, 7f, Pal.A(Pal.Hex("FFFFFF"), 0.18f));
                    c.Circle(ex - 5f, eyeY + 7f, 6f, Pal.Hex("FFFFFF"));
                    c.Circle(ex + 5f, eyeY - 8f, 3f, Pal.A(Pal.Hex("FFFFFF"), 0.85f));
                    c.EllipseRing(ex, eyeY, 19f, 22f, 2.5f, Pal.A(Pal.Ink, 0.55f));
                }
            }

            c.Stroke(new[]
            {
                new Vector2(Rig.CX - 4f, 790f),
                new Vector2(Rig.CX + 5f, 787f),
            }, 3.4f, Pal.A(d.skinShade, 0.9f));

            Vector2[] mouth = new Vector2[13];
            for (int i = 0; i < 13; i++)
            {
                float t = i / 12f;
                float a = Mathf.Lerp(200f, 340f, t) * Mathf.Deg2Rad;
                mouth[i] = new Vector2(Rig.CX + Mathf.Cos(a) * 21f, 776f + Mathf.Sin(a) * 15f);
            }
            c.Poly(mouth, Pal.Hex("8A3A50"));
            c.Ellipse(Rig.CX, 768f, 9f, 6f, Pal.Hex("EF7C93"));
            c.Stroke(new[]
            {
                new Vector2(Rig.CX - 20f, 776f),
                new Vector2(Rig.CX + 20f, 776f),
            }, 3f, Pal.A(Pal.Hex("FFFDFA"), 0.85f));
        }

        static void DrawSlip(Canvas2D c)
        {
            Color fabric = Pal.Cream;
            Color shade = Pal.CreamDeep;

            Vector2[] pts =
            {
                new Vector2(Rig.CX - 58f, 690f),
                new Vector2(Rig.CX - 40f, 700f),
                new Vector2(Rig.CX + 40f, 700f),
                new Vector2(Rig.CX + 58f, 690f),
                new Vector2(Rig.CX + 96f, 470f),
                new Vector2(Rig.CX + 112f, 412f),
                new Vector2(Rig.CX + 60f, 398f),
                new Vector2(Rig.CX, 392f),
                new Vector2(Rig.CX - 60f, 398f),
                new Vector2(Rig.CX - 112f, 412f),
                new Vector2(Rig.CX - 96f, 470f),
            };
            OutlinedPoly(c, pts, fabric, Ink);

            c.Stroke(new[] { new Vector2(Rig.CX - 44f, 694f), new Vector2(Rig.CX - 50f, 734f) }, 9f, Pal.Ink);
            c.Stroke(new[] { new Vector2(Rig.CX - 44f, 694f), new Vector2(Rig.CX - 50f, 734f) }, 4.5f, fabric);
            c.Stroke(new[] { new Vector2(Rig.CX + 44f, 694f), new Vector2(Rig.CX + 50f, 734f) }, 9f, Pal.Ink);
            c.Stroke(new[] { new Vector2(Rig.CX + 44f, 694f), new Vector2(Rig.CX + 50f, 734f) }, 4.5f, fabric);

            c.Stroke(new[]
            {
                new Vector2(Rig.CX - 84f, 452f),
                new Vector2(Rig.CX - 40f, 470f),
                new Vector2(Rig.CX + 10f, 462f),
                new Vector2(Rig.CX + 60f, 474f),
                new Vector2(Rig.CX + 96f, 452f),
            }, 3f, Pal.A(shade, 0.75f));
        }

        static void DrawHairBack(Canvas2D c, DollDef d)
        {
            switch (d.style)
            {
                case 1:
                {
                    Vector2 center = new Vector2(Rig.CX, Rig.HeadCY + 22f);
                    for (int i = 0; i < 14; i++)
                    {
                        float a = i / 14f * Mathf.PI * 2f;
                        Vector2 p = center + new Vector2(Mathf.Cos(a) * 108f, Mathf.Sin(a) * 96f);
                        c.Circle(p.x, p.y, 54f, Pal.Ink);
                    }
                    c.Ellipse(center.x, center.y, 150f, 136f, Pal.Ink);
                    for (int i = 0; i < 14; i++)
                    {
                        float a = i / 14f * Mathf.PI * 2f;
                        Vector2 p = center + new Vector2(Mathf.Cos(a) * 108f, Mathf.Sin(a) * 96f);
                        c.Circle(p.x, p.y, 47f, d.hair);
                    }
                    c.Ellipse(center.x, center.y, 143f, 129f, d.hair);
                    for (int i = 0; i < 9; i++)
                    {
                        float a = (i / 9f + 0.1f) * Mathf.PI * 2f;
                        Vector2 p = center + new Vector2(Mathf.Cos(a) * 66f, Mathf.Sin(a) * 60f);
                        c.Circle(p.x, p.y, 16f, Pal.A(d.hairShade, 0.55f));
                    }
                    break;
                }
                case 2:
                {
                    OutlinedPoly(c, new[]
                    {
                        new Vector2(Rig.CX - 104f, Rig.HeadCY + 30f),
                        new Vector2(Rig.CX - 96f, 720f),
                        new Vector2(Rig.CX + 96f, 720f),
                        new Vector2(Rig.CX + 104f, Rig.HeadCY + 30f),
                        new Vector2(Rig.CX, Rig.HeadCY + 108f),
                    }, d.hair);

                    OutlinedEllipse(c, Rig.CX + 150f, Rig.HeadCY + 34f, 40f, 44f, d.hair, 4f);
                    OutlinedEllipse(c, Rig.CX + 168f, 780f, 44f, 92f, d.hair, 4f);
                    OutlinedEllipse(c, Rig.CX + 176f, 700f, 26f, 34f, d.hair, 4f);
                    c.Stroke(new[]
                    {
                        new Vector2(Rig.CX + 150f, Rig.HeadCY + 36f),
                        new Vector2(Rig.CX + 168f, 830f),
                        new Vector2(Rig.CX + 174f, 730f),
                    }, 3f, Pal.A(d.hairShade, 0.6f));
                    break;
                }
                case 3:
                {
                    OutlinedPoly(c, new[]
                    {
                        new Vector2(Rig.CX - 112f, 620f),
                        new Vector2(Rig.CX - 118f, Rig.HeadCY + 10f),
                        new Vector2(Rig.CX - 60f, Rig.HeadCY + 100f),
                        new Vector2(Rig.CX + 60f, Rig.HeadCY + 100f),
                        new Vector2(Rig.CX + 118f, Rig.HeadCY + 10f),
                        new Vector2(Rig.CX + 112f, 620f),
                        new Vector2(Rig.CX, 600f),
                    }, d.hair);
                    break;
                }
                default:
                {
                    OutlinedPoly(c, new[]
                    {
                        new Vector2(Rig.CX - 116f, 548f),
                        new Vector2(Rig.CX - 122f, Rig.HeadCY + 20f),
                        new Vector2(Rig.CX - 62f, Rig.HeadCY + 104f),
                        new Vector2(Rig.CX + 62f, Rig.HeadCY + 104f),
                        new Vector2(Rig.CX + 122f, Rig.HeadCY + 20f),
                        new Vector2(Rig.CX + 116f, 548f),
                        new Vector2(Rig.CX + 60f, 520f),
                        new Vector2(Rig.CX - 60f, 520f),
                    }, d.hair);
                    c.Stroke(new[] { new Vector2(Rig.CX - 92f, 700f), new Vector2(Rig.CX - 88f, 560f) }, 4f, Pal.A(d.hairShade, 0.5f));
                    c.Stroke(new[] { new Vector2(Rig.CX + 92f, 700f), new Vector2(Rig.CX + 88f, 560f) }, 4f, Pal.A(d.hairShade, 0.5f));
                    break;
                }
            }
        }

        static void DrawHairFront(Canvas2D c, DollDef d)
        {
            Vector2[] cap;
            switch (d.style)
            {
                case 1:
                    cap = new[]
                    {
                        new Vector2(Rig.CX - 104f, Rig.HeadCY + 40f),
                        new Vector2(Rig.CX - 92f, Rig.HeadCY + 96f),
                        new Vector2(Rig.CX - 40f, Rig.HeadCY + 128f),
                        new Vector2(Rig.CX + 40f, Rig.HeadCY + 128f),
                        new Vector2(Rig.CX + 92f, Rig.HeadCY + 96f),
                        new Vector2(Rig.CX + 104f, Rig.HeadCY + 40f),
                        new Vector2(Rig.CX + 62f, Rig.HeadCY + 62f),
                        new Vector2(Rig.CX, Rig.HeadCY + 76f),
                        new Vector2(Rig.CX - 62f, Rig.HeadCY + 62f),
                    };
                    break;
                case 2:
                    cap = new[]
                    {
                        new Vector2(Rig.CX - 106f, Rig.HeadCY + 30f),
                        new Vector2(Rig.CX - 96f, Rig.HeadCY + 100f),
                        new Vector2(Rig.CX - 30f, Rig.HeadCY + 132f),
                        new Vector2(Rig.CX + 46f, Rig.HeadCY + 124f),
                        new Vector2(Rig.CX + 106f, Rig.HeadCY + 60f),
                        new Vector2(Rig.CX + 62f, Rig.HeadCY + 52f),
                        new Vector2(Rig.CX - 6f, Rig.HeadCY + 96f),
                        new Vector2(Rig.CX - 58f, Rig.HeadCY + 60f),
                    };
                    break;
                case 3:
                    cap = new[]
                    {
                        new Vector2(Rig.CX - 106f, Rig.HeadCY + 34f),
                        new Vector2(Rig.CX - 98f, Rig.HeadCY + 98f),
                        new Vector2(Rig.CX - 40f, Rig.HeadCY + 132f),
                        new Vector2(Rig.CX + 40f, Rig.HeadCY + 132f),
                        new Vector2(Rig.CX + 98f, Rig.HeadCY + 98f),
                        new Vector2(Rig.CX + 106f, Rig.HeadCY + 34f),
                        new Vector2(Rig.CX + 78f, Rig.HeadCY + 46f),
                        new Vector2(Rig.CX + 40f, Rig.HeadCY + 66f),
                        new Vector2(Rig.CX - 20f, Rig.HeadCY + 62f),
                        new Vector2(Rig.CX - 70f, Rig.HeadCY + 46f),
                    };
                    break;
                default:
                    cap = new[]
                    {
                        new Vector2(Rig.CX - 100f, Rig.HeadCY + 28f),
                        new Vector2(Rig.CX - 96f, Rig.HeadCY + 96f),
                        new Vector2(Rig.CX - 46f, Rig.HeadCY + 130f),
                        new Vector2(Rig.CX + 46f, Rig.HeadCY + 130f),
                        new Vector2(Rig.CX + 96f, Rig.HeadCY + 96f),
                        new Vector2(Rig.CX + 100f, Rig.HeadCY + 28f),
                        new Vector2(Rig.CX + 56f, Rig.HeadCY + 56f),
                        new Vector2(Rig.CX + 8f, Rig.HeadCY + 84f),
                        new Vector2(Rig.CX - 8f, Rig.HeadCY + 84f),
                        new Vector2(Rig.CX - 56f, Rig.HeadCY + 56f),
                    };
                    break;
            }

            OutlinedPoly(c, cap, d.hair);
            c.Stroke(new[]
            {
                new Vector2(Rig.CX + 10f, Rig.HeadCY + 118f),
                new Vector2(Rig.CX + 60f, Rig.HeadCY + 66f),
            }, 4f, Pal.A(d.hairShade, 0.45f));
        }
    }
}
