using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DressMagic.Art;
using DressMagic.Core;
using DressMagic.Data;
using DressMagic.UI;

namespace DressMagic.Screens
{
    // ==================================================== ARMARIO SECRETO

    public sealed class CollectiblesScreen : GameScreen
    {
        RectTransform gridHost;
        Text counter;

        public override void Build()
        {
            Image bg = Ui.Img(Root, "Bg", null, Pal.Hex("E2D6F7"));
            Ui.Stretch(bg.rectTransform);

            Image glow = Ui.Img(Root, "Glow", SpriteBank.Glow(512), Pal.A(Pal.White, 0.35f));
            Ui.PinAt(glow.rectTransform, 0.5f, 0.5f, 0f, 120f, 1200f, 700f);

            SparkleField sparkles = SparkleField.Create(Root, 26, Pal.A(Pal.Butter, 0.95f), 0.9f);
            if (!Game.Save.sparkles) sparkles.SetActive(false);

            Text title = Ui.LabelWithShadow(Root, "Armario Secreto", 66, Pal.Hex("7A3F72"), Pal.A(Pal.White, 0.9f));
            Ui.PinAt(title.rectTransform, 0.5f, 1f, 0f, -58f, 1200f, 84f);

            counter = Ui.Label(Root, "", 28, Pal.Hex("5A3155"));
            Ui.PinAt(counter.rectTransform, 0.5f, 1f, 0f, -124f, 1200f, 40f);

            RectTransform track = Ui.Node(Root, "ProgressTrack");
            Ui.PinAt(track, 0.5f, 1f, 0f, -166f, 660f, 24f);
            Image trackImg = track.gameObject.AddComponent<Image>();
            trackImg.sprite = Ui.Round(12, Pal.A(Pal.White, 0.85f), Pal.A(Pal.Hex("8A6AB0"), 0.4f), 3);
            trackImg.type = Image.Type.Sliced;

            RectTransform fill = Ui.Node(track, "Fill");
            float pct = Mathf.Clamp01(Game.Save.unlocked.Count / 8f);
            fill.anchorMin = new Vector2(0f, 0f);
            fill.anchorMax = new Vector2(Mathf.Max(0.001f, pct), 1f);
            fill.offsetMin = new Vector2(3f, 3f);
            fill.offsetMax = new Vector2(-3f, -3f);
            Image fillImg = fill.gameObject.AddComponent<Image>();
            fillImg.sprite = Ui.Round(10, Pal.Pink, Color.clear);
            fillImg.type = Image.Type.Sliced;

            gridHost = Ui.Node(Root, "Grid");
            Ui.Stretch(gridHost);

            RefreshCards();

            Button back = Ui.Pill(Root, "← VOLVER", 240f, 80f, Pal.White, Pal.Hex("6B4A6E"), () =>
            {
                Game.Audio.Click();
                Game.Go(Game.ReturnTarget == ScreenId.Collectibles ? ScreenId.Menu : Game.ReturnTarget);
            }, 30, 30f);
            Ui.PinAt(back.GetComponent<RectTransform>(), 0f, 0f, 160f, 66f, 240f, 80f);

            Button dress = Ui.Pill(Root, "Ir a vestir", 260f, 80f, Pal.Lilac, Pal.Hex("4A2B55"), () =>
            {
                Game.Audio.Click();
                Game.Go(ScreenId.DressUp);
            }, 30, 30f);
            Ui.PinAt(dress.GetComponent<RectTransform>(), 1f, 0f, -170f, 66f, 260f, 80f);
        }

        void RefreshCards()
        {
            if (gridHost == null) return;
            foreach (Transform child in gridHost) Destroy(child.gameObject);

            List<ItemDef> all = ItemDatabase.AllMagic();
            int unlocked = 0;
            foreach (ItemDef d in all) if (Game.Save.unlocked.Contains(d.id)) unlocked++;
            counter.text = unlocked + " de " + all.Count + " objetos mágicos descubiertos";

            const float cardW = 330f;
            const float cardH = 240f;
            const float gapX = 26f;
            const float gapY = 30f;

            for (int i = 0; i < all.Count && i < 8; i++)
            {
                ItemDef def = all[i];
                int col = i % 4;
                int row = i / 4;
                float x = -534f + col * (cardW + gapX);
                float y = 150f - row * (cardH + gapY);

                bool open = Game.Save.unlocked.Contains(def.id);

                RectTransform card = Ui.Node(gridHost, "card_" + def.id);
                Ui.PinAt(card, 0.5f, 0.5f, x, y, cardW, cardH);

                Image cardBg = card.gameObject.AddComponent<Image>();
                cardBg.sprite = Ui.Round(30, open ? Pal.Cream : Pal.A(Pal.White, 0.75f),
                    open ? Pal.PinkDeep : Pal.A(Pal.Hex("8A6AB0"), 0.4f), open ? 5 : 3);
                cardBg.type = Image.Type.Sliced;
                cardBg.raycastTarget = true;

                Image icon = Ui.Img(card, "Icon",
                    open ? SpriteBank.Icon(def.id, 0) : SpriteBank.LockedIcon(), Color.white);
                Ui.PinAt(icon.rectTransform, 0.5f, 1f, 0f, -62f, 110f, 110f);
                icon.preserveAspect = true;

                Text name = Ui.LabelWithShadow(card, open ? def.name : "???", 24,
                    open ? Pal.Hex("7A3F72") : Pal.InkSoft, Pal.A(Pal.White, 0.6f));
                Ui.PinAt(name.rectTransform, 0.5f, 1f, 0f, -142f, cardW - 24f, 34f);

                Text hint = Ui.Label(card, open ? "¡Combinación descubierta!" : def.hint, 17,
                    open ? Pal.Hex("A0345E") : Pal.TextSoft);
                Ui.PinAt(hint.rectTransform, 0.5f, 1f, 0f, -184f, cardW - 36f, 52f);

                if (open)
                {
                    Image badge = Ui.Img(card, "Badge", SpriteBank.Star(5, 64), Pal.Butter);
                    Ui.PinAt(badge.rectTransform, 1f, 1f, -24f, -24f, 34f, 34f);
                }

                Button b = card.gameObject.AddComponent<Button>();
                b.transition = Selectable.Transition.None;
                b.targetGraphic = cardBg;
                ItemDef captured = def;
                bool isOpen = open;
                b.onClick.AddListener(() =>
                {
                    if (!isOpen)
                    {
                        Game.Audio.Click();
                        Game.ShowToast("Pista: " + captured.hint, 3f);
                        return;
                    }
                    Game.ToggleItem(captured.id);
                    RefreshCards();
                    Game.ShowToast("¡" + captured.name + " puesto!", 1.8f);
                });

                ClickPunch punch = card.gameObject.AddComponent<ClickPunch>();
                punch.target = card;
            }
        }
    }

    // ========================================================= POLAROID

    public sealed class PolaroidScreen : GameScreen
    {
        static readonly Slot[] PhotoOrder =
        {
            Slot.Legs, Slot.Skirt, Slot.Dress, Slot.Top, Slot.Feet, Slot.Neck, Slot.Head, Slot.Bag, Slot.Hand
        };

        Image photoImage;

        public override void Build()
        {
            int scenario = SceneArt.IndexOf(Game.Save.scenario);

            Image bg = Ui.Img(Root, "Bg", SpriteBank.Background(scenario), Color.white);
            Ui.Stretch(bg.rectTransform);
            Image veil = Ui.Img(Root, "Veil", null, Pal.A(Pal.Hex("FFF3FB"), 0.82f));
            Ui.Stretch(veil.rectTransform);

            SparkleField sparkles = SparkleField.Create(Root, 30, Pal.A(Pal.Butter, 0.95f), 0.8f);
            if (!Game.Save.sparkles) sparkles.SetActive(false);

            // tarjeta polaroid
            RectTransform card = Ui.Node(Root, "Polaroid");
            Ui.PinAt(card, 0.5f, 0.5f, 0f, 74f, 560f, 700f);
            Image cardBg = card.gameObject.AddComponent<Image>();
            cardBg.sprite = Ui.Round(18, Pal.White, Pal.A(Pal.Hex("B0A0C8"), 0.6f), 4);
            cardBg.type = Image.Type.Sliced;
            cardBg.color = Color.white;
            PopIn pop = card.gameObject.AddComponent<PopIn>();
            pop.duration = 0.5f;

            Image shadowCard = Ui.Img(card, "SoftShadow", SpriteBank.Glow(256), Pal.A(Pal.Hex("6B3A66"), 0.22f));
            Ui.PinAt(shadowCard.rectTransform, 0.5f, 0.5f, 0f, -24f, 700f, 820f);
            shadowCard.transform.SetAsFirstSibling();

            photoImage = Ui.Img(card, "Photo", BuildPhoto(scenario), Color.white);
            Ui.PinAt(photoImage.rectTransform, 0.5f, 1f, 0f, -288f, 480f, 520f);

            Text caption = Ui.LabelWithShadow(card, "Dress Magic", 38, Pal.Hex("E8779F"), Pal.A(Pal.White, 0.9f));
            Ui.PinAt(caption.rectTransform, 0.5f, 0f, 0f, 116f, 500f, 44f);

            Text where = Ui.Label(card, SceneArt.All[scenario].name, 24, Pal.TextSoft);
            Ui.PinAt(where.rectTransform, 0.5f, 0f, 0f, 72f, 500f, 30f);

            Text dollName = Ui.Label(card, "Modelo: " + DollArt.Dolls[Game.Save.doll].name, 18, Pal.A(Pal.TextSoft, 0.9f));
            Ui.PinAt(dollName.rectTransform, 0.5f, 0f, 0f, 38f, 500f, 24f);

            Button again = Ui.Pill(Root, "JUGAR OTRA VEZ", 320f, 88f, Pal.Pink, Pal.White, () =>
            {
                Game.Audio.Click();
                Game.Go(ScreenId.Scenarios);
            }, 30, 32f);
            Ui.PinAt(again.GetComponent<RectTransform>(), 0.5f, 0.5f, -180f, -370f, 320f, 88f);

            Button finish = Ui.Pill(Root, "TERMINAR JUEGO", 340f, 88f, Pal.Mint, Pal.Hex("2E6B5C"), () =>
            {
                Game.Audio.Click();
                Game.Save.seenIntro = true;
                Game.Persist();
                Game.Go(ScreenId.Menu);
            }, 30, 32f);
            Ui.PinAt(finish.GetComponent<RectTransform>(), 0.5f, 0.5f, 180f, -370f, 340f, 88f);

            Button save = Ui.Pill(Root, "Guardar foto", 250f, 70f, Pal.Butter, Pal.Hex("6B4A2E"), SavePhoto, 24, 28f);
            Ui.PinAt(save.GetComponent<RectTransform>(), 1f, 1f, -180f, -66f, 250f, 70f);

            Game.Audio.Shutter();
        }

        /// <summary>Escribe la polaroid como PNG en la carpeta persistente del juego.</summary>
        void SavePhoto()
        {
            Game.Audio.Shutter();

            if (photoImage == null || photoImage.sprite == null)
            {
                Game.ShowToast("Aún no hay foto para guardar", 2.2f);
                return;
            }

            try
            {
                byte[] png = photoImage.sprite.texture.EncodeToPNG();
                string folder = System.IO.Path.Combine(Application.persistentDataPath, "DressMagic");
                System.IO.Directory.CreateDirectory(folder);

                string file = System.IO.Path.Combine(folder,
                    "Foto_" + (Game.Save.photoCount + 1).ToString("000") + ".png");
                System.IO.File.WriteAllBytes(file, png);

                Game.Save.photoCount++;
                Game.Persist();
                Game.ShowToast("¡Foto guardada en tu álbum! (" + Game.Save.photoCount + ")", 2.2f);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("[DressMagic] No se pudo guardar la foto: " + e.Message);
                Game.ShowToast("No se pudo guardar la foto", 2.2f);
            }
        }

        Sprite BuildPhoto(int scenario)
        {
            Game.PinOutfit();
            List<Sprite> layers = new List<Sprite>();

            ItemDef cape = Game.EquippedIn(Slot.Back);
            if (cape != null) layers.Add(SpriteBank.Wearable(cape.id, Game.ColorOf(cape.id)));

            layers.Add(SpriteBank.Doll(Game.Save.doll));

            foreach (Slot slot in PhotoOrder)
            {
                ItemDef d = Game.EquippedIn(slot);
                if (d == null) continue;
                layers.Add(SpriteBank.Wearable(d.id, Game.ColorOf(d.id)));
            }

            return SpriteBank.Photo(scenario, layers.ToArray(), 480, 520);
        }
    }
}
