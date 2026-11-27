using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DressMagic.Art;
using DressMagic.Core;
using DressMagic.Data;
using DressMagic.UI;

namespace DressMagic.Screens
{
    /// <summary>Probador: armario a la izquierda, muñeca en el escenario, armario de objetos a la derecha.</summary>
    public sealed class DressUpScreen : GameScreen
    {
        static readonly Slot[] LayerOrder =
        {
            Slot.Back, Slot.Legs, Slot.Skirt, Slot.Dress, Slot.Top, Slot.Feet, Slot.Neck, Slot.Head, Slot.Bag, Slot.Hand
        };

        readonly Dictionary<Slot, Image> layers = new Dictionary<Slot, Image>();
        readonly List<GameObject> categoryButtons = new List<GameObject>();

        RectTransform gridHost;
        RectTransform rightPanel;
        Text categoryTitle;
        Text colorTitle;
        RectTransform paletteHost;
        Text collectionLabel;
        RectTransform collectionButton;
        Text dollNameLabel;

        string activeCategory = "vestidos";
        string lastColorable;

        public override void Build()
        {
            Image bg = Ui.Img(Root, "Backdrop", SpriteBank.Background(SceneArt.IndexOf(Game.Save.scenario)), Color.white);
            Ui.Stretch(bg.rectTransform);
            Image veil = Ui.Img(Root, "Veil", null, Pal.A(Pal.Hex("F6EDFF"), 0.62f));
            Ui.Stretch(veil.rectTransform);

            BuildLeftPanel();
            BuildStage();
            BuildRightPanel();

            Game.MagicUnlocked += OnMagicUnlocked;

            RefreshDoll();
            RefreshCategoryButtons();
            RefreshGrid();
            RefreshPalette();
            RefreshCollectionButton();
        }

        public override void OnHide()
        {
            Game.MagicUnlocked -= OnMagicUnlocked;
        }

        void OnMagicUnlocked(ItemDef item)
        {
            RefreshGrid();
            RefreshCollectionButton();
        }

        // ------------------------------------------------------------- panel izq

        void BuildLeftPanel()
        {
            RectTransform panel = Panel(Root, "LeftPanel", 0.1344f, 0.5f, 390f, 852f);
            Ui.PinAt(panel, 0.1344f, 0.5f, 0f, 0f, 390f, 852f);

            Text header = Ui.LabelWithShadow(panel, "Vestuario", 40, Pal.Hex("7A3F72"), Pal.A(Pal.White, 0.9f));
            Ui.PinAt(header.rectTransform, 0.5f, 1f, 0f, -40f, 340f, 50f);

            for (int i = 0; i < ItemDatabase.Categories.Length; i++)
            {
                CategoryDef def = ItemDatabase.Categories[i];
                int index = i;
                string id = def.id;
                float oy = -92f - i * 49f;

                Button b = Ui.Pill(panel, def.label, 340f, 44f, Pal.White, Pal.Hex("5A3155"),
                    () => SelectCategory(index), 24, 22f);
                Ui.PinAt(b.GetComponent<RectTransform>(), 0.5f, 1f, 0f, oy, 340f, 44f);
                categoryButtons.Add(b.gameObject);
            }

            Button hint = Ui.Pill(panel, "✨ Pista mágica", 320f, 62f, Pal.Butter, Pal.Hex("6B4A2E"), ShowHint, 24, 28f);
            Ui.PinAt(hint.GetComponent<RectTransform>(), 0.5f, 0f, 0f, 84f, 320f, 62f);

            Button back = Ui.Pill(panel, "← Temáticas", 200f, 52f, Pal.White, Pal.Hex("6B4A6E"), () =>
            {
                Game.Audio.Click();
                Game.Go(ScreenId.Scenarios);
            }, 22, 24f);
            Ui.PinAt(back.GetComponent<RectTransform>(), 0.5f, 0f, 0f, 26f, 200f, 52f);
        }

        // --------------------------------------------------------------- stage

        void BuildStage()
        {
            RectTransform stage = Ui.Node(Root, "Stage");
            Ui.PinAt(stage, 0.475f, 0.5f, 0f, 0f, 664f, 852f);

            Image frame = stage.gameObject.AddComponent<Image>();
            frame.sprite = Ui.Round(40, Pal.White, Pal.A(Pal.Hex("8A6AB0"), 0.45f), 6);
            frame.type = Image.Type.Sliced;

            int scenario = SceneArt.IndexOf(Game.Save.scenario);
            Image bgImage = Ui.Img(stage, "ScenarioBg", SpriteBank.BackgroundFit(scenario, 650, 838, 44), Color.white);
            Ui.Stretch(bgImage.rectTransform, 7f, 7f, 7f, 7f);

            RectTransform fx = Ui.Node(stage, "StageFx");
            Ui.Stretch(fx, 24f, 24f, 24f, 24f);
            SparkleField sparkles = SparkleField.Create(fx, 16, Pal.A(Pal.White, 0.95f), 0.7f);
            if (!Game.Save.sparkles) sparkles.SetActive(false);

            // pedestal
            Image pedestal = Ui.Img(stage, "Pedestal", SpriteBank.Ellipse("pedestal", 440, 100,
                Pal.A(Pal.White, 0.85f), Pal.A(Pal.Hex("B9A2E4"), 0.9f), 5), Color.white);
            Ui.PinAt(pedestal.rectTransform, 0.5f, 0.5f, 0f, -326f, 420f, 92f);

            // capas de la muñeca
            RectTransform dollHost = Ui.Node(stage, "DollHost");
            Ui.PinAt(dollHost, 0.5f, 0.5f, 0f, 0f, 320f, 640f);

            foreach (Slot slot in LayerOrder)
            {
                Image layer = Ui.Img(dollHost, "Layer_" + slot, null, Color.white);
                Ui.Stretch(layer.rectTransform);
                layer.preserveAspect = true;
                layer.enabled = false;
                layers[slot] = layer;
            }

            Image dollImage = Ui.Img(dollHost, "Doll", SpriteBank.Doll(Game.Save.doll), Color.white);
            Ui.Stretch(dollImage.rectTransform);
            dollImage.preserveAspect = true;
            dollImage.transform.SetSiblingIndex(1);

            // selector de muñeca
            RectTransform selector = Ui.Node(stage, "DollSelector");
            Ui.PinAt(selector, 0.5f, 1f, 0f, -54f, 372f, 78f);
            Image selBg = selector.gameObject.AddComponent<Image>();
            selBg.sprite = Ui.Round(28, Pal.A(Pal.White, 0.88f), Pal.A(Pal.Hex("B9A2E4"), 0.7f), 4);
            selBg.type = Image.Type.Sliced;

            for (int i = 0; i < DollArt.Dolls.Length; i++)
            {
                int index = i;
                RectTransform holder = Ui.Node(selector, "face" + i);
                Ui.PinAt(holder, 0.5f, 0.5f, -138f + i * 92f, 0f, 74f, 74f);

                Image ring = Ui.Img(holder, "Ring", SpriteBank.Ellipse("fring" + (Game.Save.doll == i), 80, 80,
                    Game.Save.doll == i ? Pal.Pink : Pal.A(Pal.White, 0f), Pal.PinkDeep, Game.Save.doll == i ? 5 : 0), Color.white);
                Ui.Stretch(ring.rectTransform);

                Image face = Ui.Img(holder, "Face", SpriteBank.DollFace(i), Color.white, true);
                Ui.Stretch(face.rectTransform, 6f, 6f, 6f, 6f);

                Button b = holder.gameObject.AddComponent<Button>();
                b.transition = Selectable.Transition.None;
                b.targetGraphic = face;
                b.onClick.AddListener(() => SelectDoll(index));

                ClickPunch punch = holder.gameObject.AddComponent<ClickPunch>();
                punch.target = holder;
            }

            dollNameLabel = Ui.LabelWithShadow(stage, DollArt.Dolls[Game.Save.doll].name, 26,
                Pal.Hex("5A3155"), Pal.A(Pal.White, 0.85f));
            Ui.PinAt(dollNameLabel.rectTransform, 0.5f, 1f, 0f, -106f, 300f, 32f);
        }

        // --------------------------------------------------------- panel derecho

        void BuildRightPanel()
        {
            RectTransform panel = Ui.Node(Root, "RightPanel");
            Ui.PinAt(panel, 0.8416f, 0.5f, 0f, 0f, 470f, 852f);
            Image panelBg = panel.gameObject.AddComponent<Image>();
            panelBg.sprite = Ui.Round(38, Pal.Cream, Pal.A(Pal.Hex("8A6AB0"), 0.45f), 6);
            panelBg.type = Image.Type.Sliced;

            rightPanel = panel;

            categoryTitle = Ui.LabelWithShadow(panel, "Vestidos", 36, Pal.Hex("7A3F72"), Pal.A(Pal.White, 0.9f));
            Ui.PinAt(categoryTitle.rectTransform, 0.5f, 1f, 0f, -42f, 400f, 50f);

            gridHost = Ui.Node(panel, "Grid");
            Ui.PinAt(gridHost, 0.5f, 1f, 0f, -285f, 430f, 470f);

            colorTitle = Ui.Label(panel, "Color de la prenda", 22, Pal.TextSoft);
            Ui.PinAt(colorTitle.rectTransform, 0.5f, 1f, 0f, -542f, 400f, 26f);

            paletteHost = Ui.Node(panel, "Palette");
            Ui.PinAt(paletteHost, 0.5f, 1f, 0f, -586f, 430f, 46f);

            Button cont = Ui.Pill(panel, "CONTINUAR", 380f, 82f, Pal.Pink, Pal.White, () =>
            {
                Game.Audio.Click();
                Game.Go(ScreenId.Polaroid);
            }, 34, 32f);
            Ui.PinAt(cont.GetComponent<RectTransform>(), 0.5f, 1f, 0f, -666f, 380f, 82f);
            Pulse pulse = cont.gameObject.AddComponent<Pulse>();
            pulse.amount = 0.02f;

            Button remove = Ui.Pill(panel, "Quitar", 186f, 62f, Pal.White, Pal.Hex("6B4A6E"), () =>
            {
                Game.Audio.Click();
                ItemDef def = ItemDatabase.Get(FirstInCategory(activeCategory));
                if (def != null) Game.ClearSlot(def.slot);
                RefreshAll();
            }, 24, 26f);
            Ui.PinAt(remove.GetComponent<RectTransform>(), 0.5f, 1f, -98f, -748f, 186f, 62f);

            Button reset = Ui.Pill(panel, "Reiniciar", 186f, 62f, Pal.White, Pal.Hex("6B4A6E"), () =>
            {
                Game.Audio.Click();
                Game.ClearOutfit();
                lastColorable = null;
                RefreshAll();
                Game.ShowToast("Nuevo look desde cero ✨");
            }, 24, 26f);
            Ui.PinAt(reset.GetComponent<RectTransform>(), 0.5f, 1f, 98f, -748f, 186f, 62f);

            collectionButton = Ui.Node(panel, "CollectionBtn");
            Ui.PinAt(collectionButton, 0.5f, 1f, 0f, -816f, 380f, 54f);
            Image cb = collectionButton.gameObject.AddComponent<Image>();
            cb.sprite = Ui.Round(26, Pal.Lilac, Pal.A(Pal.Hex("6B3A66"), 0.4f), 4);
            cb.type = Image.Type.Sliced;
            Button cbBtn = collectionButton.gameObject.AddComponent<Button>();
            cbBtn.targetGraphic = cb;
            cbBtn.onClick.AddListener(() =>
            {
                Game.Audio.Click();
                Game.ReturnTarget = ScreenId.DressUp;
                Game.Go(ScreenId.Collectibles);
            });
            collectionLabel = Ui.Label(collectionButton, "Ver colección", 24, Pal.Hex("4A2B55"));
            Ui.Stretch(collectionLabel.rectTransform);
        }

        static RectTransform Panel(Transform parent, string name, float fx, float fy, float w, float h)
        {
            return Ui.Node(parent, name);
        }

        // ------------------------------------------------------------ acciones

        string FirstInCategory(string category)
        {
            foreach (ItemDef d in ItemDatabase.InCategory(category)) return d.id;
            return null;
        }

        void SelectCategory(int index)
        {
            Game.Audio.Click();
            activeCategory = ItemDatabase.Categories[index].id;
            RefreshCategoryButtons();
            RefreshGrid();
            RefreshPalette();
        }

        void SelectDoll(int index)
        {
            Game.Audio.Click();
            Game.Save.doll = index;
            Game.SaveSettings();

            Transform host = Root.Find("Stage/DollHost");
            if (host != null)
            {
                Image doll = host.Find("Doll")?.GetComponent<Image>();
                if (doll != null) doll.sprite = SpriteBank.Doll(index);
            }
            if (dollNameLabel != null) dollNameLabel.text = DollArt.Dolls[index].name;

            RectTransform selector = Root.Find("Stage/DollSelector") as RectTransform;
            if (selector != null)
            {
                for (int i = 0; i < selector.childCount; i++)
                {
                    Transform child = selector.GetChild(i);
                    Image ring = child.Find("Ring")?.GetComponent<Image>();
                    if (ring == null) continue;
                    bool sel = i == index;
                    ring.sprite = SpriteBank.Ellipse("fring" + (sel ? "on" : "off"), 80, 80,
                        sel ? Pal.Pink : Pal.A(Pal.White, 0f), Pal.PinkDeep, sel ? 5 : 0);
                    ring.color = Color.white;
                }
            }

            Game.ShowToast("Muñeca: " + DollArt.Dolls[index].name);
        }

        void ShowHint()
        {
            Game.Audio.Click();
            foreach (MagicCombo combo in MagicCombos.All)
            {
                if (Game.Save.unlocked.Contains(combo.reward)) continue;
                ItemDef reward = ItemDatabase.Get(combo.reward);
                if (reward == null) continue;
                Game.ShowToast("Pista: " + reward.hint, 3.4f);
                return;
            }
            Game.ShowToast("¡Ya descubriste todas las combinaciones! 🎉", 3f);
        }

        // ------------------------------------------------------------- refresco

        void RefreshAll()
        {
            RefreshDoll();
            RefreshGrid();
            RefreshPalette();
            RefreshCollectionButton();
        }

        void RefreshDoll()
        {
            Transform host = Root.Find("Stage/DollHost");
            if (host != null)
            {
                Image doll = host.Find("Doll")?.GetComponent<Image>();
                if (doll != null) doll.sprite = SpriteBank.Doll(Game.Save.doll);
            }

            foreach (Slot slot in LayerOrder)
            {
                Image layer;
                if (!layers.TryGetValue(slot, out layer) || layer == null) continue;

                ItemDef def = Game.EquippedIn(slot);
                if (def == null)
                {
                    layer.enabled = false;
                    layer.sprite = null;
                    continue;
                }
                layer.sprite = SpriteBank.Wearable(def.id, Game.ColorOf(def.id));
                layer.enabled = true;
            }

            Game.PinOutfit();

            ItemDef dress = Game.EquippedIn(Slot.Dress);
            if (dress != null && dress.colorable) lastColorable = dress.id;
        }

        void RefreshCategoryButtons()
        {
            for (int i = 0; i < categoryButtons.Count; i++)
            {
                GameObject go = categoryButtons[i];
                if (go == null) continue;
                Image body = go.transform.Find("Body")?.GetComponent<Image>();
                if (body == null) continue;
                bool active = ItemDatabase.Categories[i].id == activeCategory;
                body.sprite = Ui.Round(22, active ? Pal.Pink : Pal.White, Pal.A(Pal.Hex("6B3A66"), 0.35f), 4);
                body.color = Color.white;
                Text label = go.transform.Find("Label")?.GetComponent<Text>();
                if (label != null)
                {
                    label.color = active ? Pal.White : Pal.Hex("5A3155");
                    label.fontStyle = active ? FontStyle.Bold : FontStyle.Normal;
                }
            }
        }

        void RefreshGrid()
        {
            foreach (Transform child in gridHost) Destroy(child.gameObject);

            CategoryDef cat = null;
            foreach (CategoryDef cd in ItemDatabase.Categories) if (cd.id == activeCategory) cat = cd;
            categoryTitle.text = cat != null ? cat.label : "";

            if (cat != null && cat.isPalette)
            {
                BuildPaletteGrid(gridHost);
                return;
            }

            List<ItemDef> list = new List<ItemDef>();
            if (activeCategory == "especial")
            {
                list.AddRange(ItemDatabase.AllMagic());
            }
            else
            {
                list.AddRange(ItemDatabase.InCategory(activeCategory));
            }

            const float cellW = 132f;
            const float cellH = 138f;
            const float gapX = 12f;
            const float gapY = 10f;

            for (int i = 0; i < list.Count && i < 9; i++)
            {
                ItemDef def = list[i];
                int col = i % 3;
                int row = i / 3;
                float x = (col - 1) * (cellW + gapX);
                float y = 148f - row * (cellH + gapY);

                RectTransform cell = Ui.Node(gridHost, "cell_" + def.id);
                Ui.PinAt(cell, 0.5f, 0.5f, x, y, cellW, cellH);

                bool unlocked = Game.IsUnlocked(def);
                bool equipped = false;
                ItemDef eq = Game.EquippedIn(def.slot);
                if (eq != null && eq.id == def.id) equipped = true;

                Color fill = equipped ? Pal.PinkLight : Pal.White;
                Color border = equipped ? Pal.PinkDeep : Pal.A(Pal.Hex("8A6AB0"), 0.35f);

                Image cellBg = cell.gameObject.AddComponent<Image>();
                cellBg.sprite = Ui.Round(24, fill, border, equipped ? 5 : 3);
                cellBg.type = Image.Type.Sliced;
                cellBg.raycastTarget = true;

                Sprite iconSprite = unlocked
                    ? SpriteBank.Icon(def.id, Game.ColorOf(def.id))
                    : SpriteBank.LockedIcon();
                Image icon = Ui.Img(cell, "Icon", iconSprite, Color.white);
                Ui.PinAt(icon.rectTransform, 0.5f, 1f, 0f, -56f, cellW - 18f, 100f);
                icon.preserveAspect = true;

                Text name = Ui.Label(cell, unlocked ? def.name : "???", 15,
                    unlocked ? Pal.Hex("5A3155") : Pal.A(Pal.InkSoft, 0.7f));
                Ui.PinAt(name.rectTransform, 0.5f, 0f, 0f, 18f, cellW - 8f, 32f);

                if (equipped)
                {
                    Image check = Ui.Img(cell, "Check", SpriteBank.Star(5, 64), Pal.PinkDeep);
                    Ui.PinAt(check.rectTransform, 1f, 1f, -20f, -20f, 30f, 30f);
                }

                Button b = cell.gameObject.AddComponent<Button>();
                b.transition = Selectable.Transition.None;
                b.targetGraphic = cellBg;
                ItemDef captured = def;
                b.onClick.AddListener(() =>
                {
                    if (!Game.IsUnlocked(captured))
                    {
                        Game.Audio.Click();
                        Game.ShowToast("Combina prendas mágicas para descubrirlo ✨", 2.6f);
                        return;
                    }
                    Game.ToggleItem(captured.id);
                    if (captured.colorable) lastColorable = captured.id;
                    RefreshAll();
                });

                ClickPunch punch = cell.gameObject.AddComponent<ClickPunch>();
                punch.target = cell;
            }
        }

        void RefreshPalette()
        {
            foreach (Transform child in paletteHost) Destroy(child.gameObject);

            if (activeCategory == "colores")
            {
                paletteHost.gameObject.SetActive(false);
                return;
            }
            paletteHost.gameObject.SetActive(true);

            ItemDef target = ColorTarget();
            bool enabled = target != null;

            colorTitle.text = enabled ? "Color de " + target.name : "Elige una prenda para darle color";
            paletteHost.gameObject.SetActive(true);

            const float size = 46f;
            const float gap = 6f;
            for (int i = 0; i < ItemDatabase.ColorValues.Length; i++)
            {
                int index = i;
                RectTransform sw = Ui.Node(paletteHost, "sw" + i);
                Ui.PinAt(sw, 0.5f, 0.5f, (i - 3.5f) * (size + gap), 0f, size, size);

                bool selected = enabled && Game.ColorOf(target.id) == i;
                Image img = sw.gameObject.AddComponent<Image>();
                img.sprite = SpriteBank.Ellipse("sw" + i, 56, 56, ItemDatabase.ColorValues[i],
                    selected ? Pal.Hex("4A2B48") : Pal.A(Pal.Hex("6B3A66"), 0.35f), selected ? 5 : 2);
                img.color = enabled ? Color.white : Pal.A(Pal.White, 0.5f);
                img.raycastTarget = true;

                Button b = sw.gameObject.AddComponent<Button>();
                b.transition = Selectable.Transition.None;
                b.targetGraphic = img;
                b.onClick.AddListener(() =>
                {
                    ItemDef t = ColorTarget();
                    if (t == null)
                    {
                        Game.ShowToast("Primero elige una prenda 🎀");
                        return;
                    }
                    Game.Audio.Click();
                    Game.SetColor(t.id, index);
                    RefreshDoll();
                    RefreshGrid();
                    RefreshPalette();
                });

                ClickPunch punch = sw.gameObject.AddComponent<ClickPunch>();
                punch.target = sw;
            }
        }

        void BuildPaletteGrid(RectTransform host)
        {
            colorTitle.text = "Toca un color para pintar tu prenda";

            Color[] cols = ItemDatabase.ColorValues;
            for (int i = 0; i < cols.Length; i++)
            {
                int index = i;
                int col = i % 2;
                int row = i / 2;
                RectTransform cell = Ui.Node(host, "pcell" + i);
                Ui.PinAt(cell, 0.5f, 0.5f, (col - 0.5f) * 200f, 180f - row * 120f, 190f, 112f);

                Image bg = cell.gameObject.AddComponent<Image>();
                bg.sprite = Ui.Round(28, Pal.White, Pal.A(Pal.Hex("8A6AB0"), 0.35f), 3);
                bg.type = Image.Type.Sliced;
                bg.raycastTarget = true;

                Image dot = Ui.Img(cell, "Dot", SpriteBank.Ellipse("bigsw" + i, 84, 84, cols[i], Pal.A(Pal.Hex("6B3A66"), 0.35f), 3), Color.white);
                Ui.PinAt(dot.rectTransform, 0.5f, 1f, 0f, -46f, 74f, 74f);

                Text label = Ui.Label(cell, ItemDatabase.ColorNames[i], 18, Pal.Hex("5A3155"));
                Ui.PinAt(label.rectTransform, 0.5f, 0f, 0f, 18f, 170f, 26f);

                Button b = cell.gameObject.AddComponent<Button>();
                b.transition = Selectable.Transition.None;
                b.targetGraphic = bg;
                b.onClick.AddListener(() =>
                {
                    ItemDef t = ColorTarget();
                    if (t == null)
                    {
                        Game.ShowToast("Primero elige una prenda 🎀");
                        return;
                    }
                    Game.Audio.Click();
                    Game.SetColor(t.id, index);
                    RefreshDoll();
                    RefreshPalette();
                });

                ClickPunch punch = cell.gameObject.AddComponent<ClickPunch>();
                punch.target = cell;
            }
        }

        ItemDef ColorTarget()
        {
            ItemDef inCategory = null;
            int count = 0;
            foreach (ItemDef d in Game.Equipped.Values) count++;

            ItemDef dress = Game.EquippedIn(Slot.Dress);
            if (dress != null && dress.colorable) inCategory = dress;

            if (inCategory == null && !string.IsNullOrEmpty(lastColorable))
            {
                ItemDef def = ItemDatabase.Get(lastColorable);
                if (def != null && def.colorable && IsWorn(def.id)) inCategory = def;
            }

            if (inCategory == null)
            {
                foreach (ItemDef d in Game.Equipped.Values)
                {
                    if (d.colorable) { inCategory = d; break; }
                }
            }
            return inCategory;
        }

        bool IsWorn(string id)
        {
            foreach (ItemDef d in Game.Equipped.Values) if (d.id == id) return true;
            return false;
        }

        void RefreshCollectionButton()
        {
            if (collectionButton == null) return;
            int count = Game.Save.unlocked.Count;
            collectionButton.gameObject.SetActive(count > 0);
            if (collectionLabel != null) collectionLabel.text = "Ver colección (" + count + "/8)";
        }
    }
}
