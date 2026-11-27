using System.Collections.Generic;
using UnityEngine;

namespace DressMagic.Data
{
    public enum Slot
    {
        Back = 0,
        Legs = 1,
        Skirt = 2,
        Dress = 3,
        Top = 4,
        Feet = 5,
        Neck = 6,
        Head = 7,
        Bag = 8,
        Hand = 9
    }

    public sealed class ItemDef
    {
        public string id;
        public string name;
        public string category;
        public Slot slot;
        public bool colorable;
        public int defaultColor;
        public bool magic;
        public string hint;

        public ItemDef(string id, string name, string category, Slot slot, bool colorable = true, int defaultColor = 0, bool magic = false, string hint = "")
        {
            this.id = id;
            this.name = name;
            this.category = category;
            this.slot = slot;
            this.colorable = colorable;
            this.defaultColor = defaultColor;
            this.magic = magic;
            this.hint = hint;
        }
    }

    public sealed class CategoryDef
    {
        public string id;
        public string label;
        public bool isPalette;

        public CategoryDef(string id, string label, bool isPalette = false)
        {
            this.id = id;
            this.label = label;
            this.isPalette = isPalette;
        }
    }

    public static class ItemDatabase
    {
        public static readonly CategoryDef[] Categories =
        {
            new CategoryDef("vestidos", "Vestidos"),
            new CategoryDef("blusas", "Blusas"),
            new CategoryDef("pantalones", "Pantalones"),
            new CategoryDef("faldas", "Faldas"),
            new CategoryDef("zapatos", "Zapatos"),
            new CategoryDef("tacones", "Tacones"),
            new CategoryDef("colores", "Colores", true),
            new CategoryDef("diademas", "Diademas"),
            new CategoryDef("coronas", "Coronas"),
            new CategoryDef("sombreros", "Sombreros"),
            new CategoryDef("bolsos", "Bolsos"),
            new CategoryDef("varitas", "Varitas"),
            new CategoryDef("especial", "Especial"),
        };

        public static readonly string[] ColorNames =
        {
            "Rosa", "Lila", "Menta", "Cielo", "Mantequilla", "Coral", "Blanco", "Fucsia"
        };

        public static readonly Color[] ColorValues =
        {
            Pal.Hex("F7A8C4"),
            Pal.Hex("C9B2F0"),
            Pal.Hex("A5E7D6"),
            Pal.Hex("A7D8F7"),
            Pal.Hex("FFE197"),
            Pal.Hex("FFB59B"),
            Pal.Hex("FFF7EF"),
            Pal.Hex("F062A0"),
        };

        static readonly Dictionary<string, ItemDef> byId = new Dictionary<string, ItemDef>();
        static readonly Dictionary<string, List<ItemDef>> byCategory = new Dictionary<string, List<ItemDef>>();

        static ItemDatabase()
        {
            Add(new ItemDef("vestido_princesa", "Vestido Princesa", "vestidos", Slot.Dress, true, 0));
            Add(new ItemDef("vestido_tutu", "Vestido Tutú", "vestidos", Slot.Dress, true, 1));
            Add(new ItemDef("vestido_pichi", "Pichi Estrellado", "vestidos", Slot.Dress, true, 3));
            Add(new ItemDef("vestido_fiesta", "Vestido de Fiesta", "vestidos", Slot.Dress, true, 2));
            Add(new ItemDef("vestido_mariposa", "Vestido Mariposa", "vestidos", Slot.Dress, true, 1));
            Add(new ItemDef("vestido_verano", "Vestido Verano", "vestidos", Slot.Dress, true, 4));

            Add(new ItemDef("blusa_lunar", "Suéter Lunar", "blusas", Slot.Top, true, 1));
            Add(new ItemDef("blusa_nube", "Blusa Nube", "blusas", Slot.Top, true, 6));
            Add(new ItemDef("top_mariposa", "Top Mariposa", "blusas", Slot.Top, true, 0));
            Add(new ItemDef("blusa_lazo", "Blusa con Lazo", "blusas", Slot.Top, true, 5));

            Add(new ItemDef("pantalon_leggings", "Leggings", "pantalones", Slot.Legs, true, 7));
            Add(new ItemDef("pantalon_mezclilla", "Mezclilla", "pantalones", Slot.Legs, true, 3));
            Add(new ItemDef("pantalon_corto", "Shorts", "pantalones", Slot.Legs, true, 2));
            Add(new ItemDef("mallas_estrella", "Mallas Estrella", "pantalones", Slot.Legs, true, 1));

            Add(new ItemDef("falda_tutu", "Falda Tutú", "faldas", Slot.Skirt, true, 0));
            Add(new ItemDef("falda_plisada", "Falda Plisada", "faldas", Slot.Skirt, true, 3));
            Add(new ItemDef("falda_estrellas", "Falda Estrellas", "faldas", Slot.Skirt, true, 1));

            Add(new ItemDef("zapatos_merceditas", "Merceditas", "zapatos", Slot.Feet, true, 7));
            Add(new ItemDef("zapatos_deportivos", "Deportivos", "zapatos", Slot.Feet, true, 6));
            Add(new ItemDef("botas_invierno", "Botas de Invierno", "zapatos", Slot.Feet, true, 1));
            Add(new ItemDef("zapatos_magicos", "Zapatos Mágicos", "zapatos", Slot.Feet, true, 0));
            Add(new ItemDef("patines_hielo", "Patines de Hielo", "zapatos", Slot.Feet, false, 6));

            Add(new ItemDef("tacones_cristal", "Tacones de Cristal", "tacones", Slot.Feet, false, 3));
            Add(new ItemDef("tacones_rosa", "Tacones Rosa", "tacones", Slot.Feet, true, 0));

            Add(new ItemDef("diadema_flores", "Diadema de Flores", "diademas", Slot.Head, false, 0));
            Add(new ItemDef("diadema_perlas", "Diadema de Perlas", "diademas", Slot.Head, false, 6));

            Add(new ItemDef("corona_dorada", "Corona Dorada", "coronas", Slot.Head, false, 0));
            Add(new ItemDef("corona_corazon", "Corona de Corazón", "coronas", Slot.Head, false, 0));

            Add(new ItemDef("sombrero_invierno", "Gorro de Invierno", "sombreros", Slot.Head, true, 0));
            Add(new ItemDef("sombrero_paja", "Sombrero de Paja", "sombreros", Slot.Head, false, 0));

            Add(new ItemDef("bolso_conchas", "Bolso de Conchas", "bolsos", Slot.Bag, true, 1));
            Add(new ItemDef("bolso_corazon", "Bolso Corazón", "bolsos", Slot.Bag, true, 0));

            Add(new ItemDef("varita_luna", "Varita Luna", "varitas", Slot.Hand, false, 0));
            Add(new ItemDef("varita_estrella", "Varita Estrella", "varitas", Slot.Hand, false, 0));

            Add(new ItemDef("corona_estelar", "Corona Estelar", "especial", Slot.Head, false, 0, true, "Brilla cuando el vestido y la corona combinan"));
            Add(new ItemDef("tiara_estrellas", "Tiara de Estrellas", "especial", Slot.Head, false, 0, true, "Unos tacones de cristal la llaman"));
            Add(new ItemDef("zapatillas_magicas", "Zapatillas Mágicas", "especial", Slot.Feet, false, 0, true, "Los zapatos mágicos y una diadema"));
            Add(new ItemDef("bolso_magico", "Bolso Mágico", "especial", Slot.Bag, false, 0, true, "Una blusa nube con falda de tutú"));
            Add(new ItemDef("vestido_magico", "Vestido Mágico", "especial", Slot.Dress, false, 0, true, "Vestido de fiesta y varita lunar"));
            Add(new ItemDef("capa_magica", "Capa Mágica", "especial", Slot.Back, false, 0, true, "Falda de estrellas y merceditas"));
            Add(new ItemDef("varita_arcana", "Varita Arcana", "especial", Slot.Hand, false, 0, true, "Vestido mariposa y corona de corazón"));
            Add(new ItemDef("broche_estrella", "Broche Estrella", "especial", Slot.Neck, false, 0, true, "Suéter lunar y bolso de conchas"));
        }

        static void Add(ItemDef d)
        {
            byId[d.id] = d;
            List<ItemDef> list;
            if (!byCategory.TryGetValue(d.category, out list))
            {
                list = new List<ItemDef>();
                byCategory[d.category] = list;
            }
            list.Add(d);
        }

        public static ItemDef Get(string id)
        {
            ItemDef d;
            return id != null && byId.TryGetValue(id, out d) ? d : null;
        }

        public static List<ItemDef> InCategory(string category)
        {
            List<ItemDef> list;
            return byCategory.TryGetValue(category, out list) ? list : new List<ItemDef>();
        }

        public static IEnumerable<ItemDef> All => byId.Values;

        public static List<ItemDef> AllMagic()
        {
            List<ItemDef> list = new List<ItemDef>();
            foreach (ItemDef d in byId.Values) if (d.magic) list.Add(d);
            return list;
        }
    }

    public sealed class MagicCombo
    {
        public string id;
        public string itemA;
        public string itemB;
        public string reward;

        public MagicCombo(string id, string a, string b, string reward)
        {
            this.id = id;
            itemA = a;
            itemB = b;
            this.reward = reward;
        }
    }

    public static class MagicCombos
    {
        public static readonly MagicCombo[] All =
        {
            new MagicCombo("m1", "vestido_princesa", "corona_dorada", "corona_estelar"),
            new MagicCombo("m2", "blusa_nube", "falda_tutu", "bolso_magico"),
            new MagicCombo("m3", "zapatos_magicos", "diadema_perlas", "zapatillas_magicas"),
            new MagicCombo("m4", "tacones_cristal", "sombrero_invierno", "tiara_estrellas"),
            new MagicCombo("m5", "vestido_fiesta", "varita_luna", "vestido_magico"),
            new MagicCombo("m6", "blusa_lunar", "bolso_conchas", "broche_estrella"),
            new MagicCombo("m7", "falda_estrellas", "zapatos_merceditas", "capa_magica"),
            new MagicCombo("m8", "vestido_mariposa", "corona_corazon", "varita_arcana"),
        };

        /// <summary>Devuelve el combo que se cumple exactamente con el conjunto dado (o null).</summary>
        public static MagicCombo Match(HashSet<string> equipped)
        {
            foreach (MagicCombo c in All)
            {
                if (equipped.Contains(c.itemA) && equipped.Contains(c.itemB))
                    return c;
            }
            return null;
        }

        public static MagicCombo ForReward(string rewardId)
        {
            foreach (MagicCombo c in All) if (c.reward == rewardId) return c;
            return null;
        }
    }
}
