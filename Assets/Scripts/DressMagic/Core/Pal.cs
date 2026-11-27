using UnityEngine;

namespace DressMagic
{
    /// <summary>Paleta pastel central del juego.</summary>
    public static class Pal
    {
        public static readonly Color Ink       = Hex("4A2B48");
        public static readonly Color InkSoft   = Hex("7A5580");
        public static readonly Color TextSoft  = Hex("8A6A8C");
        public static readonly Color BgLilac   = Hex("D9CDF5");
        public static readonly Color BgLilac2  = Hex("BFA9EE");
        public static readonly Color Panel     = Hex("F1E9FC");
        public static readonly Color PanelDeep = Hex("DED0F5");
        public static readonly Color PanelEdge = Hex("B9A2E4");
        public static readonly Color Cream     = Hex("FFF7EC");
        public static readonly Color CreamDeep = Hex("FFE7CB");
        public static readonly Color Pink      = Hex("F7A8C4");
        public static readonly Color PinkDeep  = Hex("E8779F");
        public static readonly Color PinkLight = Hex("FDD6E3");
        public static readonly Color Rose      = Hex("F4B8CE");
        public static readonly Color Mint      = Hex("A9E5D8");
        public static readonly Color MintDeep  = Hex("6FCFBC");
        public static readonly Color Sky       = Hex("AEDCF7");
        public static readonly Color SkyDeep   = Hex("74B9E8");
        public static readonly Color Butter    = Hex("FFE197");
        public static readonly Color Lilac     = Hex("CDB8F2");
        public static readonly Color Peach     = Hex("FFCFB0");
        public static readonly Color White     = Color.white;

        public static Color Hex(string h)
        {
            Color c;
            if (!ColorUtility.TryParseHtmlString("#" + h, out c)) c = Color.magenta;
            return c;
        }

        public static Color A(Color c, float a)
        {
            c.a = a;
            return c;
        }

        public static Color Darken(Color c, float f)
        {
            return new Color(c.r * (1f - f), c.g * (1f - f), c.b * (1f - f), c.a);
        }

        public static Color Lighten(Color c, float f)
        {
            return new Color(c.r + (1f - c.r) * f, c.g + (1f - c.g) * f, c.b + (1f - c.b) * f, c.a);
        }

        public static Color Mix(Color a, Color b, float t)
        {
            return Color.Lerp(a, b, Mathf.Clamp01(t));
        }
    }

    /// <summary>
    /// Proporciones fijas de la muñeca en espacio de diseño 512 x 1024 (origen abajo-izquierda).
    /// Toda la ropa se dibuja en este mismo espacio, así encaja siempre.
    /// </summary>
    public static class Rig
    {
        public const int W = 512;
        public const int H = 1024;

        public const float CX = 256f;

        public const float FootY     = 96f;
        public const float AnkleY    = 150f;
        public const float HipY      = 470f;
        public const float WaistY    = 556f;
        public const float ChestY    = 690f;
        public const float ShoulderY = 706f;
        public const float NeckY     = 726f;

        public const float HeadCY = 836f;
        public const float HeadRX = 96f;
        public const float HeadRY = 105f;

        public const float HipHalf      = 62f;
        public const float ShoulderHalf = 66f;
        public const float LegX         = 32f;
        public const float LegR         = 27f;
        public const float ArmX         = 66f;
        public const float HandY        = 512f;
        public const float ArmR         = 18f;

        public const float SlipTopY   = 692f;
        public const float SlipHemY   = 402f;
        public const float SlipHalfTop = 62f;
        public const float SlipHalfHem = 112f;
    }
}
