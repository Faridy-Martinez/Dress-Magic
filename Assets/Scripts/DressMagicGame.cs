using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controlador inicial de Dress & Magic.
/// Coloca los sprites en Assets/Resources/DressMagic/ siguiendo la guía del README.
/// </summary>
public class DressMagicGame : MonoBehaviour
{
    [Serializable] public class ClothingCategory
    {
        public string folder;
        public string label;
    }

    [Serializable] public class MagicCombination
    {
        public string id;
        public string firstItem;
        public string secondItem;
        public string rewardItem;
        public string message;
    }

    [Header("Recursos (dentro de Resources/DressMagic)")]
    public string dollResource = "Doll";
    public string backgroundsFolder = "Backgrounds";
    public ClothingCategory[] categories = {
        new ClothingCategory { folder = "Clothes", label = "Ropa" },
        new ClothingCategory { folder = "Shoes", label = "Zapatos" },
        new ClothingCategory { folder = "Hair", label = "Peinados" },
        new ClothingCategory { folder = "Accessories", label = "Accesorios" }
    };

    [Header("Combinaciones mágicas (nombres exactos de archivo, sin .png)")]
    public MagicCombination[] combinations = new MagicCombination[0];

    private readonly string[] sceneNames = {
        "Pista de hielo", "Parque de juegos", "Cine de estrellas",
        "Dulce pastelería", "Pista de baile fiesta", "Escenario de concierto"
    };
    private readonly string[] sceneIds = { "Hielo", "Parque", "Cine", "Pasteleria", "Baile", "Concierto" };
    private readonly Color[] sceneColors = {
        new Color(0.78f,0.91f,1f), new Color(0.82f,0.95f,0.82f),
        new Color(0.83f,0.77f,0.94f), new Color(1f,0.86f,0.88f),
        new Color(0.84f,0.80f,0.98f), new Color(0.97f,0.80f,0.88f)
    };

    private Canvas canvas;
    private Image backgroundImage;
    private Image dollImage;
    private readonly Dictionary<string, Image> clothingLayers = new Dictionary<string, Image>();
    private readonly Dictionary<string, Sprite[]> itemCache = new Dictionary<string, Sprite[]>();
    private readonly Dictionary<string, Sprite> selectedItems = new Dictionary<string, Sprite>();
    private readonly HashSet<string> unlocked = new HashSet<string>();
    private string currentScene = "";
    private string currentCategory = "Clothes";
    private Text statusText;
    private Text titleText;
    private RectTransform itemGrid;
    private Sprite[] dollSprites;

    private void Start()
    {
        LoadProgress();
        BuildInterface();
        ShowHome();
    }

    private void BuildInterface()
    {
        GameObject canvasObject = new GameObject("Dress & Magic UI", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.matchWidthOrHeight = 0.5f;

        RectTransform root = Panel("Root", canvas.transform, new Color(1f,0.94f,0.97f));
        Stretch(root);

        Text brand = MakeText("Dress & Magic  ✦", root, 34, new Color(0.55f,0.25f,0.45f), TextAnchor.MiddleCenter);
        Place(brand.rectTransform, 0.5f, 1f, 0.5f, 1f, new Vector2(0, -48), new Vector2(600, 70));

        backgroundImage = MakeImage("SceneBackground", root, Color.white);
        Stretch(backgroundImage.rectTransform);
        backgroundImage.transform.SetAsFirstSibling();
        dollImage = MakeImage("OriginalDoll", root, Color.white);
        dollImage.preserveAspect = true;
        Place(dollImage.rectTransform, 0.5f, 0.08f, 0.5f, 0.08f, Vector2.zero, new Vector2(360, 540));

        foreach (ClothingCategory category in categories)
        {
            Image layer = MakeImage(category.folder + "Layer", root, Color.white);
            layer.preserveAspect = true;
            Place(layer.rectTransform, 0.5f, 0.08f, 0.5f, 0.08f, Vector2.zero, new Vector2(360, 540));
            clothingLayers[category.folder] = layer;
            layer.gameObject.SetActive(false);
        }

        titleText = MakeText("", root, 27, new Color(0.4f,0.2f,0.38f), TextAnchor.MiddleCenter);
        Place(titleText.rectTransform, 0.5f, 0.89f, 0.5f, 0.89f, Vector2.zero, new Vector2(760, 52));

        statusText = MakeText("", root, 18, new Color(0.42f,0.25f,0.38f), TextAnchor.MiddleCenter);
        Place(statusText.rectTransform, 0.5f, 0.025f, 0.5f, 0.025f, Vector2.zero, new Vector2(900, 42));
    }

    private void ShowHome()
    {
        ClearDynamicButtons();
        SetDollVisible(false);
        titleText.text = "¡Tu estilo, tu magia!";
        statusText.text = "Elige un escenario y crea tu propio outfit.";
        AddButton("¡Jugar ahora!", new Vector2(0.5f,0.48f), new Vector2(260,70), ShowScenes);
        AddButton("Mi colección  ♡", new Vector2(0.5f,0.35f), new Vector2(260,60), ShowCollection);
    }

    private void ShowScenes()
    {
        ClearDynamicButtons();
        SetDollVisible(false);
        titleText.text = "¿A dónde vamos hoy?";
        statusText.text = "Selecciona uno de los seis escenarios.";
        for (int i = 0; i < sceneNames.Length; i++)
        {
            int index = i;
            float x = 0.27f + (i % 3) * 0.23f;
            float y = 0.60f - (i / 3) * 0.23f;
            AddButton(sceneNames[i], new Vector2(x,y), new Vector2(245,85), () => StartScene(index));
        }
        AddButton("Volver", new Vector2(0.5f,0.12f), new Vector2(180,48), ShowHome);
    }

    private void StartScene(int index)
    {
        currentScene = sceneIds[index];
        Sprite[] bg = Resources.LoadAll<Sprite>(backgroundsFolder + "/" + currentScene);
        Sprite background = Resources.Load<Sprite>("DressMagic/" + backgroundsFolder + "/" + currentScene);
        if (background == null && bg.Length > 0) background = bg[0];
        backgroundImage.sprite = background;
        backgroundImage.color = background == null ? sceneColors[index] : Color.white;
        foreach (Image layer in clothingLayers.Values) layer.sprite = null;
        selectedItems.Clear();
        currentCategory = categories.Length > 0 ? categories[0].folder : "Clothes";
        ShowWardrobe();
    }

    private void ShowWardrobe()
    {
        ClearDynamicButtons();
        SetDollVisible(true);
        Sprite doll = Resources.Load<Sprite>("DressMagic/" + dollResource);
        if (doll == null)
        {
            Sprite[] all = Resources.LoadAll<Sprite>("DressMagic");
            if (all.Length > 0) doll = all[0];
        }
        dollImage.sprite = doll;
        titleText.text = currentScene + "  ✦  Crea tu outfit";
        statusText.text = doll == null
            ? "Falta importar la muñeca como Sprite en Resources/DressMagic/Doll."
            : "Selecciona prendas. La ilustración base no se modifica.";

        float leftX = 0.16f;
        AddButton("← Escenarios", new Vector2(0.12f,0.13f), new Vector2(180,48), ShowScenes);
        AddButton("¡Listo!", new Vector2(0.87f,0.13f), new Vector2(170,52), ShowFinal);
        AddButton("Quitar prendas", new Vector2(0.87f,0.22f), new Vector2(170,45), ClearOutfit);

        for (int i = 0; i < categories.Length; i++)
        {
            int index = i;
            AddButton(categories[i].label, new Vector2(leftX,0.72f - i*0.105f), new Vector2(190,52),
                () => ShowCategory(categories[index].folder));
        }
        ShowCategory(currentCategory);
    }

    private void ShowCategory(string folder)
    {
        currentCategory = folder;
        ClearItemButtons();
        Sprite[] sprites = GetItems(folder);
        if (sprites.Length == 0)
        {
            statusText.text = "Aún no hay sprites en la categoría " + folder + ". Puedes agregarlos después.";
            return;
        }
        for (int i = 0; i < sprites.Length; i++)
        {
            Sprite item = sprites[i];
            float x = 0.36f + (i % 4) * 0.12f;
            float y = 0.78f - (i / 4) * 0.18f;
            AddSpriteButton(item, new Vector2(x,y), () => SelectItem(folder,item));
        }
    }

    private Sprite[] GetItems(string folder)
    {
        if (!itemCache.ContainsKey(folder))
            itemCache[folder] = Resources.LoadAll<Sprite>("DressMagic/" + folder);
        return itemCache[folder];
    }

    private void SelectItem(string folder, Sprite item)
    {
        selectedItems[folder] = item;
        if (clothingLayers.ContainsKey(folder))
        {
            Image layer = clothingLayers[folder];
            layer.sprite = item;
            layer.gameObject.SetActive(true);
        }
        CheckMagic();
        statusText.text = "Elegiste: " + item.name;
    }

    private void ClearOutfit()
    {
        selectedItems.Clear();
        foreach (Image layer in clothingLayers.Values) { layer.sprite = null; layer.gameObject.SetActive(false); }
        statusText.text = "¡Puedes crear un nuevo look!";
    }

    private void CheckMagic()
    {
        foreach (MagicCombination combo in combinations)
        {
            if (unlocked.Contains(combo.id)) continue;
            bool hasFirst = HasSelected(combo.firstItem);
            bool hasSecond = HasSelected(combo.secondItem);
            if (hasFirst && hasSecond)
            {
                unlocked.Add(combo.id);
                if (!string.IsNullOrWhiteSpace(combo.rewardItem)) unlocked.Add(combo.rewardItem);
                SaveProgress();
                statusText.text = string.IsNullOrEmpty(combo.message) ? "¡Combinación mágica descubierta!" : combo.message;
            }
        }
    }

    private bool HasSelected(string itemName)
    {
        foreach (Sprite sprite in selectedItems.Values)
            if (sprite != null && sprite.name == itemName) return true;
        return false;
    }

    private void ShowCollection()
    {
        ClearDynamicButtons();
        SetDollVisible(false);
        titleText.text = "Mi colección  ♡";
        statusText.text = unlocked.Count == 0 ? "Todavía no has desbloqueado piezas mágicas." : "Elementos desbloqueados: " + string.Join(", ", unlocked);
        AddButton("Volver", new Vector2(0.5f,0.2f), new Vector2(180,52), ShowHome);
    }

    private void ShowFinal()
    {
        ClearDynamicButtons();
        titleText.text = "¡Tu look está listo! ✨";
        statusText.text = "Escenario: " + currentScene + "   |   Prendas seleccionadas: " + selectedItems.Count;
        AddButton("Volver al vestidor", new Vector2(0.5f,0.35f), new Vector2(230,55), ShowWardrobe);
        AddButton("Elegir otro escenario", new Vector2(0.5f,0.24f), new Vector2(230,55), ShowScenes);
        AddButton("Inicio", new Vector2(0.5f,0.13f), new Vector2(180,48), ShowHome);
    }

    private void LoadProgress()
    {
        string saved = PlayerPrefs.GetString("DressMagicUnlocked", "");
        foreach (string entry in saved.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries)) unlocked.Add(entry);
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetString("DressMagicUnlocked", string.Join("|", unlocked));
        PlayerPrefs.Save();
    }

    private readonly List<GameObject> dynamicButtons = new List<GameObject>();
    private readonly List<GameObject> itemButtons = new List<GameObject>();

    private void ClearDynamicButtons()
    {
        foreach (GameObject button in dynamicButtons) if (button != null) Destroy(button);
        dynamicButtons.Clear();
        ClearItemButtons();
    }

    private void ClearItemButtons()
    {
        foreach (GameObject button in itemButtons) if (button != null) Destroy(button);
        itemButtons.Clear();
    }

    private void AddButton(string label, Vector2 anchor, Vector2 size, Action action)
    {
        GameObject go = new GameObject("Button_" + label, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(canvas.transform, false);
        Place(go.GetComponent<RectTransform>(), anchor.x, anchor.y, anchor.x, anchor.y, Vector2.zero, size);
        Image image = go.GetComponent<Image>();
        image.color = new Color(1f,0.78f,0.88f);
        Button button = go.GetComponent<Button>();
        button.onClick.AddListener(() => action());
        Text text = MakeText(label, go.transform, 18, new Color(0.36f,0.2f,0.32f), TextAnchor.MiddleCenter);
        Stretch(text.rectTransform);
        dynamicButtons.Add(go);
    }

    private void AddSpriteButton(Sprite sprite, Vector2 anchor, Action action)
    {
        GameObject go = new GameObject("Item_" + sprite.name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(canvas.transform, false);
        Place(go.GetComponent<RectTransform>(), anchor.x, anchor.y, anchor.x, anchor.y, Vector2.zero, new Vector2(105,115));
        Image image = go.GetComponent<Image>();
        image.sprite = sprite; image.preserveAspect = true; image.color = Color.white;
        Button button = go.GetComponent<Button>();
        button.onClick.AddListener(() => action());
        itemButtons.Add(go);
    }

    private void SetDollVisible(bool visible)
    {
        dollImage.gameObject.SetActive(visible);
        foreach (Image layer in clothingLayers.Values) layer.gameObject.SetActive(visible && layer.sprite != null);
        backgroundImage.gameObject.SetActive(visible);
    }

    private RectTransform Panel(string name, Transform parent, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = color;
        return go.GetComponent<RectTransform>();
    }

    private Image MakeImage(string name, Transform parent, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        Image image = go.GetComponent<Image>(); image.color = color;
        return image;
    }

    private Text MakeText(string value, Transform parent, int size, Color color, TextAnchor alignment)
    {
        GameObject go = new GameObject("Text_" + value, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);
        Text text = go.GetComponent<Text>();
        text.text = value; text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = size; text.color = color; text.alignment = alignment;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        return text;
    }

    private void Place(RectTransform rect, float minX, float minY, float maxX, float maxY, Vector2 offset, Vector2 size)
    {
        rect.anchorMin = new Vector2(minX,minY); rect.anchorMax = new Vector2(maxX,maxY);
        rect.anchoredPosition = offset; rect.sizeDelta = size;
    }

    private void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero; rect.offsetMax = Vector2.zero;
    }
}