using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Settings Reference")]
    public SettingsManager settings;

    [Header("Configuration")]
    public KeyCode toggleKey = KeyCode.F9;

    private GameObject _root;
    private Slider _volume;
    private Toggle _subtitles;
    private Dropdown _graphics;

    private void Start()
    {
        if (settings == null)
        {
            Debug.LogWarning("SettingsMenu: No SettingsManager assigned, attempting to load from Resources");
            settings = Resources.Load<SettingsManager>("SettingsManager");
        }

        BuildUi();
        _root.SetActive(false);

        // Apply initial settings
        ApplyCurrentSettings();
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            _root.SetActive(!_root.activeSelf);
        }
    }

    private void ApplyCurrentSettings()
    {
        if (settings == null) return;

        AudioListener.volume = settings.masterVolume;
        ApplyGraphicsPreset(settings.graphicsPreset);
    }

    private void BuildUi()
    {
        var canvasGo = new GameObject("SettingsCanvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGo.AddComponent<CanvasScaler>();
        canvasGo.AddComponent<GraphicRaycaster>();

        var panel = new GameObject("Panel");
        panel.transform.SetParent(canvasGo.transform, false);
        var rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.3f, 0.3f);
        rt.anchorMax = new Vector2(0.7f, 0.7f);
        var img = panel.AddComponent<Image>();
        img.color = new Color(0f, 0f, 0f, 0.85f);

        // Title
        CreateLabel(panel.transform, "Settings", new Vector2(0.5f, 0.9f), TextAnchor.MiddleCenter, 24);

        // Volume slider
        _volume = CreateLabeledControl<Slider>(panel.transform, "Master Volume", new Vector2(0.5f, 0.7f));
        if (_volume != null)
        {
            _volume.minValue = 0f;
            _volume.maxValue = 1f;
            _volume.value = settings != null ? settings.masterVolume : 1f;
            _volume.onValueChanged.AddListener(OnVolumeChanged);
        }

        // Subtitles toggle
        _subtitles = CreateLabeledControl<Toggle>(panel.transform, "Subtitles", new Vector2(0.5f, 0.5f));
        if (_subtitles != null)
        {
            _subtitles.isOn = settings != null && settings.subtitlesOn;
            _subtitles.onValueChanged.AddListener(OnSubtitlesChanged);
        }

        // Graphics dropdown
        _graphics = CreateLabeledControl<Dropdown>(panel.transform, "Graphics", new Vector2(0.5f, 0.3f));
        if (_graphics != null)
        {
            _graphics.ClearOptions();
            _graphics.AddOptions(new System.Collections.Generic.List<string> { "Low", "Medium", "High" });

            int graphicsIndex = settings != null ? (int)settings.graphicsPreset : 1;
            if (graphicsIndex >= 0 && graphicsIndex < _graphics.options.Count)
            {
                _graphics.value = graphicsIndex;
            }
            else
            {
                Debug.LogWarning($"SettingsMenu: Invalid graphics preset index {graphicsIndex}");
                _graphics.value = 1; // Default to Medium
            }

            _graphics.onValueChanged.AddListener(OnGraphicsChanged);
        }

        // Close button
        var closeBtn = CreateButton(panel.transform, "Close", new Vector2(0.5f, 0.1f));
        closeBtn.onClick.AddListener(() => _root.SetActive(false));

        _root = canvasGo;
    }

    private void OnVolumeChanged(float value)
    {
        if (settings != null)
        {
            settings.masterVolume = value;
        }
        AudioListener.volume = value;
    }

    private void OnSubtitlesChanged(bool value)
    {
        if (settings != null)
        {
            settings.subtitlesOn = value;
        }
    }

    private void OnGraphicsChanged(int index)
    {
        if (index < 0 || index > 2)
        {
            Debug.LogWarning($"SettingsMenu: Invalid graphics index {index}");
            return;
        }

        var preset = (GraphicsPreset)index;
        if (settings != null)
        {
            settings.graphicsPreset = preset;
        }
        ApplyGraphicsPreset(preset);
    }

    private void ApplyGraphicsPreset(GraphicsPreset preset)
    {
        int qualityLevel;
        switch (preset)
        {
            case GraphicsPreset.Low:
                qualityLevel = 0;
                break;
            case GraphicsPreset.Medium:
                qualityLevel = Mathf.Min(2, QualitySettings.names.Length - 1);
                break;
            case GraphicsPreset.High:
                qualityLevel = QualitySettings.names.Length - 1;
                break;
            default:
                Debug.LogWarning($"SettingsMenu: Unknown graphics preset {preset}");
                return;
        }

        if (qualityLevel >= 0 && qualityLevel < QualitySettings.names.Length)
        {
            QualitySettings.SetQualityLevel(qualityLevel, true);
        }
    }

    private T CreateLabeledControl<T>(Transform parent, string label, Vector2 anchor) where T : Component
    {
        var container = new GameObject(label.Replace(" ", "") + "Container");
        container.transform.SetParent(parent, false);
        var containerRt = container.AddComponent<RectTransform>();
        containerRt.anchorMin = anchor;
        containerRt.anchorMax = anchor;
        containerRt.sizeDelta = new Vector2(350, 40);

        // Label
        var labelGo = new GameObject("Label");
        labelGo.transform.SetParent(container.transform, false);
        var labelTxt = labelGo.AddComponent<Text>();
        labelTxt.text = label;
        labelTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        labelTxt.color = Color.white;
        labelTxt.fontSize = 16;
        labelTxt.alignment = TextAnchor.MiddleLeft;
        var labelRt = labelGo.GetComponent<RectTransform>();
        labelRt.anchorMin = new Vector2(0f, 0f);
        labelRt.anchorMax = new Vector2(0.4f, 1f);
        labelRt.offsetMin = Vector2.zero;
        labelRt.offsetMax = Vector2.zero;

        // Control
        var controlGo = new GameObject("Control");
        controlGo.transform.SetParent(container.transform, false);
        var controlRt = controlGo.AddComponent<RectTransform>();
        controlRt.anchorMin = new Vector2(0.45f, 0f);
        controlRt.anchorMax = new Vector2(1f, 1f);
        controlRt.offsetMin = Vector2.zero;
        controlRt.offsetMax = Vector2.zero;

        T control = null;
        if (typeof(T) == typeof(Slider))
        {
            control = CreateSliderComponent(controlGo) as T;
        }
        else if (typeof(T) == typeof(Toggle))
        {
            control = CreateToggleComponent(controlGo) as T;
        }
        else if (typeof(T) == typeof(Dropdown))
        {
            control = CreateDropdownComponent(controlGo) as T;
        }

        return control;
    }

    private Slider CreateSliderComponent(GameObject parent)
    {
        var bg = parent.AddComponent<Image>();
        bg.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        var slider = parent.AddComponent<Slider>();

        var fillArea = new GameObject("FillArea");
        fillArea.transform.SetParent(parent.transform, false);
        var fillRt = fillArea.AddComponent<RectTransform>();
        fillRt.anchorMin = new Vector2(0.05f, 0.25f);
        fillRt.anchorMax = new Vector2(0.95f, 0.75f);
        fillRt.offsetMin = Vector2.zero;
        fillRt.offsetMax = Vector2.zero;

        var fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        var fillImg = fill.AddComponent<Image>();
        fillImg.color = new Color(0.5f, 0.8f, 0.5f, 1f);
        var fillImgRt = fill.GetComponent<RectTransform>();
        fillImgRt.anchorMin = Vector2.zero;
        fillImgRt.anchorMax = Vector2.one;
        fillImgRt.offsetMin = Vector2.zero;
        fillImgRt.offsetMax = Vector2.zero;

        slider.fillRect = fillImgRt;

        return slider;
    }

    private Toggle CreateToggleComponent(GameObject parent)
    {
        var toggle = parent.AddComponent<Toggle>();

        var bg = new GameObject("Background");
        bg.transform.SetParent(parent.transform, false);
        var bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        var bgRt = bg.GetComponent<RectTransform>();
        bgRt.anchorMin = new Vector2(0f, 0.1f);
        bgRt.anchorMax = new Vector2(0.3f, 0.9f);
        bgRt.offsetMin = Vector2.zero;
        bgRt.offsetMax = Vector2.zero;

        var checkmark = new GameObject("Checkmark");
        checkmark.transform.SetParent(bg.transform, false);
        var checkImg = checkmark.AddComponent<Image>();
        checkImg.color = new Color(0.5f, 0.8f, 0.5f, 1f);
        var checkRt = checkmark.GetComponent<RectTransform>();
        checkRt.anchorMin = new Vector2(0.1f, 0.1f);
        checkRt.anchorMax = new Vector2(0.9f, 0.9f);
        checkRt.offsetMin = Vector2.zero;
        checkRt.offsetMax = Vector2.zero;

        toggle.targetGraphic = bgImg;
        toggle.graphic = checkImg;

        return toggle;
    }

    private Dropdown CreateDropdownComponent(GameObject parent)
    {
        var bg = parent.AddComponent<Image>();
        bg.color = new Color(0.3f, 0.3f, 0.3f, 1f);

        var dropdown = parent.AddComponent<Dropdown>();

        var label = new GameObject("Label");
        label.transform.SetParent(parent.transform, false);
        var labelTxt = label.AddComponent<Text>();
        labelTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        labelTxt.color = Color.white;
        labelTxt.alignment = TextAnchor.MiddleLeft;
        var labelRt = label.GetComponent<RectTransform>();
        labelRt.anchorMin = new Vector2(0.05f, 0f);
        labelRt.anchorMax = new Vector2(0.9f, 1f);
        labelRt.offsetMin = Vector2.zero;
        labelRt.offsetMax = Vector2.zero;

        dropdown.captionText = labelTxt;

        // Create template (simplified)
        var template = new GameObject("Template");
        template.transform.SetParent(parent.transform, false);
        var templateRt = template.AddComponent<RectTransform>();
        templateRt.anchorMin = new Vector2(0f, 0f);
        templateRt.anchorMax = new Vector2(1f, 0f);
        templateRt.pivot = new Vector2(0.5f, 1f);
        templateRt.sizeDelta = new Vector2(0, 150);

        var templateImg = template.AddComponent<Image>();
        templateImg.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        var viewport = new GameObject("Viewport");
        viewport.transform.SetParent(template.transform, false);
        var viewportRt = viewport.AddComponent<RectTransform>();
        viewportRt.anchorMin = Vector2.zero;
        viewportRt.anchorMax = Vector2.one;
        viewportRt.offsetMin = Vector2.zero;
        viewportRt.offsetMax = Vector2.zero;
        viewport.AddComponent<Mask>().showMaskGraphic = false;
        viewport.AddComponent<Image>().color = Color.white;

        var content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        var contentRt = content.AddComponent<RectTransform>();
        contentRt.anchorMin = new Vector2(0f, 1f);
        contentRt.anchorMax = new Vector2(1f, 1f);
        contentRt.pivot = new Vector2(0.5f, 1f);
        contentRt.sizeDelta = new Vector2(0, 28);

        var item = new GameObject("Item");
        item.transform.SetParent(content.transform, false);
        var itemRt = item.AddComponent<RectTransform>();
        itemRt.anchorMin = new Vector2(0f, 0.5f);
        itemRt.anchorMax = new Vector2(1f, 0.5f);
        itemRt.sizeDelta = new Vector2(0, 28);

        var itemToggle = item.AddComponent<Toggle>();
        var itemBg = item.AddComponent<Image>();
        itemBg.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        itemToggle.targetGraphic = itemBg;

        var itemLabel = new GameObject("Item Label");
        itemLabel.transform.SetParent(item.transform, false);
        var itemLabelTxt = itemLabel.AddComponent<Text>();
        itemLabelTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        itemLabelTxt.color = Color.white;
        itemLabelTxt.alignment = TextAnchor.MiddleLeft;
        var itemLabelRt = itemLabel.GetComponent<RectTransform>();
        itemLabelRt.anchorMin = new Vector2(0.05f, 0f);
        itemLabelRt.anchorMax = new Vector2(0.95f, 1f);
        itemLabelRt.offsetMin = Vector2.zero;
        itemLabelRt.offsetMax = Vector2.zero;

        dropdown.template = templateRt;
        dropdown.itemText = itemLabelTxt;

        template.SetActive(false);

        return dropdown;
    }

    private Text CreateLabel(Transform parent, string text, Vector2 anchor, TextAnchor alignment = TextAnchor.MiddleLeft, int fontSize = 16)
    {
        var go = new GameObject("Label");
        go.transform.SetParent(parent, false);
        var txt = go.AddComponent<Text>();
        txt.text = text;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.color = Color.white;
        txt.fontSize = fontSize;
        txt.alignment = alignment;
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.sizeDelta = new Vector2(200, 30);
        return txt;
    }

    private Button CreateButton(Transform parent, string label, Vector2 anchor)
    {
        var go = new GameObject(label + "Button");
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.sizeDelta = new Vector2(120, 40);
        var img = go.AddComponent<Image>();
        img.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        var btn = go.AddComponent<Button>();

        var txtGo = new GameObject("Text");
        txtGo.transform.SetParent(go.transform, false);
        var txt = txtGo.AddComponent<Text>();
        txt.text = label;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.color = Color.white;
        txt.alignment = TextAnchor.MiddleCenter;
        var txtRt = txtGo.GetComponent<RectTransform>();
        txtRt.anchorMin = Vector2.zero;
        txtRt.anchorMax = Vector2.one;
        txtRt.offsetMin = Vector2.zero;
        txtRt.offsetMax = Vector2.zero;

        return btn;
    }
}
