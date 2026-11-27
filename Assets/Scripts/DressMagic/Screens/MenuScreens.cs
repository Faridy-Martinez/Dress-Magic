using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DressMagic.Art;
using DressMagic.Core;
using DressMagic.Data;
using DressMagic.UI;

namespace DressMagic.Screens
{
    // ============================================================ MENÚ

    public sealed class MenuScreen : GameScreen
    {
        public override void Build()
        {
            RectTransform root = Root;

            Image bg = Ui.Img(root, "Sky", MakeMenuBackground(), Color.white);
            Ui.Stretch(bg.rectTransform);

            SparkleField sparkles = SparkleField.Create(root, 34, Pal.A(Pal.Butter, 0.95f), 1f);
            if (!Game.Save.sparkles) sparkles.SetActive(false);

            // decoraciones flotantes
            for (int i = 0; i < 7; i++)
            {
                Sprite deco = i % 3 == 0 ? SpriteBank.Heart(96)
                    : i % 3 == 1 ? SpriteBank.Star(5, 96) : SpriteBank.Star(4, 96, true);
                Image d = Ui.Img(root, "deco", deco, Pal.A(i % 2 == 0 ? Pal.PinkDeep : Pal.Lilac, 0.55f));
                float sz = 56f + (i % 4) * 26f;
                Vector2 p = new Vector2(Random.Range(-620f, 620f), Random.Range(-320f, 320f));
                Ui.PinAt(d.rectTransform, 0.5f, 0.5f, p.x, p.y, sz, sz);
                FloatBob bob = d.gameObject.AddComponent<FloatBob>();
                bob.amplitude = 16f + i * 4f;
                bob.speed = 0.5f + i * 0.09f;
                bob.phase = i * 1.7f;
            }

            // halo suave detrás del título
            Image ribbon = Ui.Img(root, "Ribbon", SpriteBank.Glow(256), Pal.A(Pal.White, 0.6f));
            Ui.PinAt(ribbon.rectTransform, 0.5f, 0.5f, 0f, 244f, 1280f, 340f);

            Text title = Ui.Label(root, TitleMarkup(), 132, Color.white);
            title.supportRichText = true;
            Outline outline = title.gameObject.AddComponent<Outline>();
            outline.effectColor = Pal.Hex("7A3568");
            outline.effectDistance = new Vector2(6f, -7f);
            Shadow sh = title.gameObject.AddComponent<Shadow>();
            sh.effectColor = new Color(0.42f, 0.14f, 0.36f, 0.45f);
            sh.effectDistance = new Vector2(0f, -14f);
            Ui.PinAt(title.rectTransform, 0.5f, 0.5f, 0f, 236f, 1400f, 200f);
            FloatBob titleBob = title.gameObject.AddComponent<FloatBob>();
            titleBob.amplitude = 7f;
            titleBob.speed = 1.1f;

            Text sub = Ui.LabelWithShadow(root, "Tu estilo, tu magia", 40, Pal.Hex("6B3A63"), Pal.A(Pal.White, 0.85f));
            Ui.PinAt(sub.rectTransform, 0.5f, 0.5f, 0f, 120f, 900f, 60f);

            Button play = Ui.Pill(root, "JUGAR", 520f, 118f, Pal.Pink, Pal.White, OnPlay, 52, 40f);
            Ui.PinAt(play.GetComponent<RectTransform>(), 0.5f, 0.5f, 0f, -40f, 520f, 118f);
            Pulse pulse = play.gameObject.AddComponent<Pulse>();
            pulse.amount = 0.025f;

            Button settings = Ui.Pill(root, "AJUSTES", 300f, 92f, Pal.Lilac, Pal.Hex("4A2B55"), () =>
            {
                Game.Audio.Click();
                Game.Go(ScreenId.Settings);
            }, 34, 34f);
            Ui.PinAt(settings.GetComponent<RectTransform>(), 0.5f, 0.5f, -170f, -190f, 300f, 92f);

            Button collection = Ui.Pill(root, "COLECCIÓN", 320f, 92f, Pal.Mint, Pal.Hex("2E6B5C"), () =>
            {
                Game.Audio.Click();
                Game.ReturnTarget = ScreenId.Menu;
                Game.Go(ScreenId.Collectibles);
            }, 32, 34f);
            Ui.PinAt(collection.GetComponent<RectTransform>(), 0.5f, 0.5f, 170f, -190f, 320f, 92f);

            int unlockedCount = Game.Save.unlocked.Count;
            Text counter = Ui.LabelWithShadow(root, "Objetos mágicos: " + unlockedCount + " / 8", 26,
                Pal.Hex("5A3155"), Pal.A(Pal.White, 0.8f));
            Ui.PinAt(counter.rectTransform, 0.5f, 0.5f, 0f, -290f, 700f, 40f);

            Text footer = Ui.Label(root, "Dress Magic", 20, Pal.A(Pal.Hex("6B3A63"), 0.6f));
            Ui.PinAt(footer.rectTransform, 0.5f, 0f, 0f, 34f, 400f, 30f);

            Game.Audio.PlayMusic();
        }

        void OnPlay()
        {
            Game.Audio.Click();
            Game.Go(ScreenId.Scenarios);
        }

        static string TitleMarkup()
        {
            string[] cols = { "F062A0", "9C6BE0", "48A8E0", "34C0A8", "F0A93C" };
            string word = "Dress Magic";
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            int ci = 0;
            foreach (char ch in word)
            {
                if (ch == ' ')
                {
                    sb.Append("  ");
                    continue;
                }
                sb.Append("<color=#").Append(cols[ci % cols.Length]).Append(">").Append(ch).Append("</color>");
                ci++;
            }
            return sb.ToString();
        }

        static Sprite MakeMenuBackground()
        {
            const int W = 960;
            const int H = 540;
            Canvas2D c = new Canvas2D(W, H, 2);
            c.SetTransform(W / 1600f);
            c.MultiGradient(Pal.Hex("FDF0FF"), Pal.Hex("E7D5FA"), Pal.Hex("CDB6F2"), 0.6f);

            c.Glow(320f, 700f, 520f, Pal.A(Pal.Hex("FFC7E4"), 0.55f), 2f);
            c.Glow(1300f, 740f, 480f, Pal.A(Pal.Hex("C7E9FF"), 0.55f), 2f);
            c.Glow(800f, 180f, 620f, Pal.A(Pal.Hex("FFF0C0"), 0.45f), 2f);
            c.Glow(760f, 430f, 700f, Pal.A(Pal.White, 0.28f), 1.6f);

            Random.State prev = Random.state;
            Random.InitState(9001);
            for (int i = 0; i < 70; i++)
            {
                float x = Random.Range(0f, 1600f);
                float y = Random.Range(60f, 860f);
                float r = Random.Range(4f, 16f);
                c.Star(x, y, r, r * 0.36f, 4, Random.Range(0f, 90f), Pal.A(Pal.White, Random.Range(0.3f, 0.95f)));
            }
            for (int i = 0; i < 10; i++)
            {
                float x = Random.Range(0f, 1600f);
                float y = Random.Range(120f, 800f);
                c.Glow(x, y, Random.Range(60f, 150f), Pal.A(i % 2 == 0 ? Pal.Pink : Pal.Sky, 0.12f), 2f);
            }
            Random.state = prev;

            // nubes suaves en la base
            c.Ellipse(140f, 30f, 340f, 150f, Pal.A(Pal.White, 0.5f));
            c.Ellipse(560f, 10f, 420f, 170f, Pal.A(Pal.White, 0.42f));
            c.Ellipse(1080f, 20f, 460f, 180f, Pal.A(Pal.White, 0.45f));
            c.Ellipse(1500f, 40f, 320f, 150f, Pal.A(Pal.White, 0.4f));

            return c.ToSprite(true);
        }
    }

    // ========================================================== AJUSTES

    public sealed class SettingsScreen : GameScreen
    {
        public override void Build()
        {
            Image bg = Ui.Img(Root, "Bg", null, Pal.Hex("E6DAF8"));
            Ui.Stretch(bg.rectTransform);
            SparkleField sparkles = SparkleField.Create(Root, 18, Pal.A(Pal.White, 0.9f), 0.8f);
            if (!Game.Save.sparkles) sparkles.SetActive(false);

            RectTransform panel = Ui.Node(Root, "Panel");
            Ui.Pin(panel, 0.5f, 0.5f, 1080f, 780f);
            Image panelBg = panel.gameObject.AddComponent<Image>();
            panelBg.sprite = Ui.Round(44, Pal.Cream, Pal.A(Pal.Hex("8A6AB0"), 0.5f), 6);
            panelBg.type = Image.Type.Sliced;

            Text title = Ui.LabelWithShadow(panel, "Ajustes", 62, Pal.Hex("7A3F72"), Pal.A(Pal.White, 0.9f));
            Ui.PinAt(title.rectTransform, 0.5f, 1f, 0f, -66f, 600f, 80f);

            float y = 232f;
            AddSliderRow(panel, "Música", ref y, Game.Save.music, Pal.Pink, v =>
            {
                Game.Save.music = v;
                Game.Audio.Apply(Game.Save);
            });
            AddSliderRow(panel, "Efectos", ref y, Game.Save.sfx, Pal.Sky, v =>
            {
                Game.Save.sfx = v;
                Game.Audio.Apply(Game.Save);
            });
            AddSliderRow(panel, "Brillo", ref y, Game.Save.brightness, Pal.Butter, v =>
            {
                Game.Save.brightness = v;
                Game.ApplyBrightness();
            });

            Game.Save.brightness = Mathf.Clamp(Game.Save.brightness, 0.35f, 1f);

            AddSegmentedRow(panel, "Tamaño de interfaz", ref y, Game.Save.uiScale,
                new[] { "Pequeña", "Normal", "Grande" }, idx =>
                {
                    Game.Save.uiScale = idx;
                    Game.ApplyUiScale();
                    Game.SaveSettings();
                });

            AddToggleRow(panel, "Destellos mágicos", ref y, Game.Save.sparkles, v =>
            {
                Game.Save.sparkles = v;
                Game.SaveSettings();
            });

            AddToggleRow(panel, "Animaciones", ref y, Game.Save.animations, v =>
            {
                Game.Save.animations = v;
                Game.SaveSettings();
            });

            Button back = Ui.Pill(panel, "VOLVER", 300f, 88f, Pal.Lilac, Pal.Hex("4A2B55"), () =>
            {
                Game.Audio.Click();
                Game.SaveSettings();
                Game.Go(ScreenId.Menu);
            }, 34, 34f);
            Ui.PinAt(back.GetComponent<RectTransform>(), 0.5f, 0f, 0f, 74f, 300f, 88f);

            Button reset = Ui.Pill(panel, "Borrar progreso", 300f, 62f, Pal.A(Pal.Hex("F0A0B8"), 0.95f), Pal.White,
                () => ConfirmWipe(), 24, 26f);
            Ui.PinAt(reset.GetComponent<RectTransform>(), 0.5f, 0f, 380f, 76f, 300f, 62f);
        }

        void AddSliderRow(RectTransform panel, string label, ref float y, float value, Color fill, System.Action<float> onChange)
        {
            RectTransform row = Ui.Node(panel, "row_" + label);
            Ui.PinAt(row, 0.5f, 0.5f, 0f, y, 940f, 92f);

            Text lbl = Ui.Label(row, label, 30, Pal.Hex("5A3155"), TextAnchor.MiddleLeft, true);
            Ui.PinAt(lbl.rectTransform, 0f, 0.5f, 150f, 0f, 280f, 60f);

            Text val = Ui.Label(row, Mathf.RoundToInt(value * 100f) + "%", 26, Pal.TextSoft, TextAnchor.MiddleRight);
            Ui.PinAt(val.rectTransform, 1f, 0.5f, -60f, 0f, 120f, 60f);

            Slider slider = Ui.Slider(row, 520f, 56f, value, fill, Pal.Hex("DCCEF0"), v =>
            {
                onChange(v);
                val.text = Mathf.RoundToInt(v * 100f) + "%";
            });
            Ui.PinAt((RectTransform)slider.transform, 0.5f, 0.5f, 60f, 0f, 520f, 56f);

            y -= 104f;
        }

        void AddSegmentedRow(RectTransform panel, string label, ref float y, int value, string[] options, System.Action<int> onChange)
        {
            RectTransform row = Ui.Node(panel, "row_" + label);
            Ui.PinAt(row, 0.5f, 0.5f, 0f, y, 940f, 92f);

            const float rowW = 940f;
            const float labelW = 300f;
            const float btnW = 168f;
            const float btnH = 62f;
            const float btnGap = 10f;

            Text lbl = Ui.Label(row, label, 30, Pal.Hex("5A3155"), TextAnchor.MiddleLeft, true);
            Ui.PinAt(lbl.rectTransform, 0f, 0.5f, 160f, 0f, labelW, 60f);

            // Centre the button group in the space right of the label.
            float labelRight = -rowW * 0.5f + 160f + labelW * 0.5f;
            float availRight = rowW * 0.5f - 20f;
            float groupW = options.Length * btnW + (options.Length - 1) * btnGap;
            float startX = labelRight + (availRight - labelRight) * 0.5f - groupW * 0.5f + btnW * 0.5f;

            for (int i = 0; i < options.Length; i++)
            {
                int index = i;
                bool selected = value == i;
                Button b = Ui.Pill(row, options[i], btnW, btnH,
                    selected ? Pal.Pink : Pal.Hex("E6DBF6"),
                    selected ? Pal.White : Pal.Hex("6B4A6E"),
                    () =>
                    {
                        Game.Audio.Click();
                        onChange(index);
                        Game.Go(ScreenId.Settings);
                    }, 24, 26f);
                Ui.PinAt(b.GetComponent<RectTransform>(), 0.5f, 0.5f, startX + i * (btnW + btnGap), 0f, btnW, btnH);
            }

            y -= 100f;
        }

        void AddToggleRow(RectTransform panel, string label, ref float y, bool value, System.Action<bool> onChange)
        {
            RectTransform row = Ui.Node(panel, "row_" + label);
            Ui.PinAt(row, 0.5f, 0.5f, 0f, y, 940f, 80f);

            Text lbl = Ui.Label(row, label, 30, Pal.Hex("5A3155"), TextAnchor.MiddleLeft, true);
            Ui.PinAt(lbl.rectTransform, 0f, 0.5f, 170f, 0f, 320f, 60f);

            Toggle t = Ui.Switch(row, 132f, 62f, value, Pal.MintDeep, v => onChange(v));
            Ui.PinAt((RectTransform)t.transform, 0.5f, 0.5f, 300f, 0f, 132f, 62f);

            y -= 88f;
        }

        void ConfirmWipe()
        {
            Game.Audio.Click();

            RectTransform scrim = Ui.Node(Root, "WipeScrim");
            Ui.Stretch(scrim);
            Image scrimImg = scrim.gameObject.AddComponent<Image>();
            scrimImg.color = new Color(0.2f, 0.1f, 0.2f, 0.6f);
            scrimImg.raycastTarget = true;

            RectTransform card = Ui.Node(scrim, "Card");
            Ui.Pin(card, 0.5f, 0.5f, 700f, 380f);
            Image cardBg = card.gameObject.AddComponent<Image>();
            cardBg.sprite = Ui.Round(36, Pal.Cream, Pal.A(Pal.Hex("8A5A8C"), 0.45f), 6);
            cardBg.type = Image.Type.Sliced;
            cardBg.raycastTarget = true;

            Text msg = Ui.LabelWithShadow(card, "¿Borrar todo el progreso?\nSe perderán los objetos mágicos.", 34,
                Pal.Hex("A0345E"), Pal.A(Pal.White, 0.9f));
            Ui.PinAt(msg.rectTransform, 0.5f, 1f, 0f, -110f, 620f, 130f);

            Button no = Ui.Pill(card, "No", 240f, 82f, Pal.Mint, Pal.Hex("2E6B5C"), () =>
            {
                Game.Audio.Click();
                Destroy(scrim.gameObject);
            }, 32, 30f);
            Ui.PinAt(no.GetComponent<RectTransform>(), 0.5f, 0f, -140f, 84f, 240f, 82f);

            Button yes = Ui.Pill(card, "Sí, borrar", 260f, 82f, Pal.Hex("F08AA8"), Pal.White, () =>
            {
                SaveSystem.Wipe();
                Game.ResetEverything();
            }, 30, 30f);
            Ui.PinAt(yes.GetComponent<RectTransform>(), 0.5f, 0f, 140f, 84f, 260f, 82f);
        }
    }

    // ======================================================= ESCENARIOS

    public sealed class ScenarioScreen : GameScreen
    {
        public override void Build()
        {
            Image bg = Ui.Img(Root, "Bg", null, Pal.Hex("EFE4FB"));
            Ui.Stretch(bg.rectTransform);
            SparkleField sparkles = SparkleField.Create(Root, 22, Pal.A(Pal.Butter, 0.9f), 0.9f);
            if (!Game.Save.sparkles) sparkles.SetActive(false);

            Text title = Ui.LabelWithShadow(Root, "Elige tu temática", 64, Pal.Hex("7A3F72"), Pal.A(Pal.White, 0.9f));
            Ui.PinAt(title.rectTransform, 0.5f, 1f, 0f, -78f, 1000f, 84f);

            Text sub = Ui.Label(Root, "Cada escenario tiene su propio mundo mágico", 26, Pal.TextSoft);
            Ui.PinAt(sub.rectTransform, 0.5f, 1f, 0f, -140f, 1000f, 40f);

            int count = SceneArt.All.Length;
            float cardW = 284f;
            float gap = 18f;
            float totalW = count * cardW + (count - 1) * gap;
            float startX = -totalW * 0.5f + cardW * 0.5f;

            for (int i = 0; i < count; i++)
            {
                int index = i;
                ScenarioDef def = SceneArt.All[i];
                float x = startX + i * (cardW + gap);

                RectTransform card = Ui.Node(Root, "card_" + def.id);
                Ui.PinAt(card, 0.5f, 0.5f, x, -20f, cardW, 452f);
                Image cardBg = card.gameObject.AddComponent<Image>();
                cardBg.sprite = Ui.Round(30, Pal.White, Pal.A(Pal.Hex("8A6AB0"), 0.45f), 5);
                cardBg.type = Image.Type.Sliced;
                cardBg.raycastTarget = true;

                Image pic = Ui.Img(card, "Preview", SpriteBank.BackgroundFit(index, 260, 250, 26), Color.white);
                Ui.PinAt(pic.rectTransform, 0.5f, 1f, 0f, -145f, cardW - 24f, 250f);
                pic.raycastTarget = false;

                Text name = Ui.LabelWithShadow(card, def.name, 24, Pal.Hex("5A3155"), Pal.A(Pal.White, 0.8f));
                Ui.PinAt(name.rectTransform, 0.5f, 0f, 0f, 146f, cardW - 20f, 66f);

                Text tag = Ui.Label(card, def.tagline, 19, Pal.TextSoft);
                Ui.PinAt(tag.rectTransform, 0.5f, 0f, 0f, 92f, cardW - 20f, 30f);

                Button choose = Ui.Pill(card, "¡Vamos!", 190f, 64f, def.accent, Pal.Hex("4A2B48"),
                    () => Choose(index, card), 28, 28f);
                Ui.PinAt(choose.GetComponent<RectTransform>(), 0.5f, 0f, 0f, 42f, 190f, 64f);

                Button overlay = card.gameObject.AddComponent<Button>();
                overlay.transition = Selectable.Transition.None;
                overlay.targetGraphic = cardBg;
                overlay.onClick.AddListener(() => Choose(index, card));

                FadeIn f = card.gameObject.AddComponent<FadeIn>();
                f.delay = i * 0.06f;
                f.rise = 30f;

                if (Game.Save.scenario == def.id)
                {
                    Image badge = Ui.Img(card, "Badge", SpriteBank.Star(5, 96), Pal.Butter);
                    Ui.PinAt(badge.rectTransform, 1f, 1f, -34f, -34f, 56f, 56f);
                }
            }

            Button back = Ui.Pill(Root, "← VOLVER", 230f, 80f, Pal.White, Pal.Hex("6B4A6E"), () =>
            {
                Game.Audio.Click();
                Game.Go(ScreenId.Menu);
            }, 28, 30f);
            Ui.PinAt(back.GetComponent<RectTransform>(), 0f, 0f, 150f, 70f, 230f, 80f);
        }

        void Choose(int index, RectTransform card)
        {
            Game.Audio.Click();
            Game.Save.scenario = SceneArt.All[index].id;
            Game.Persist();
            StartCoroutine(ChooseRoutine(card));
        }

        IEnumerator ChooseRoutine(RectTransform card)
        {
            float t = 0f;
            while (t < 0.22f)
            {
                t += Time.unscaledDeltaTime;
                float k = Mathf.Clamp01(t / 0.22f);
                if (card != null) card.localScale = Vector3.one * (1f + k * 0.06f);
                yield return null;
            }
            Game.Go(ScreenId.DressUp);
        }
    }
}
