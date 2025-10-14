using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public SettingsManager settings;
    private GameObject _root;

    private Slider _volume;
    private Toggle _subtitles;
    private Dropdown _graphics;

    private void Start()
    {
        BuildUi();
        _root.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9)) _root.SetActive(!_root.activeSelf);
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
        img.color = new Color(0f, 0f, 0f, 0.8f);

        _volume = CreateLabeledSlider(panel.transform, "Master Volume", new Vector2(0.5f, 0.7f));
        _volume.value = settings != null ? settings.masterVolume : 1f;
        _volume.onValueChanged.AddListener(v => { if (settings != null) settings.masterVolume = v; AudioListener.volume = v; });

        _subtitles = CreateLabeledToggle(panel.transform, "Subtitles", new Vector2(0.5f, 0.5f));
        _subtitles.isOn = settings != null && settings.subtitlesOn;
        _subtitles.onValueChanged.AddListener(v => { if (settings != null) settings.subtitlesOn = v; });

        _graphics = CreateLabeledDropdown(panel.transform, "Graphics", new Vector2(0.5f, 0.3f), new string[] { "Low", "Medium", "High" });
        _graphics.value = settings != null ? (int)settings.graphicsPreset : 1;
        _graphics.onValueChanged.AddListener(i => { if (settings != null) settings.graphicsPreset = (GraphicsPreset)i; ApplyGraphicsPreset((GraphicsPreset)i); });

        _root = canvasGo;
    }

    private void ApplyGraphicsPreset(GraphicsPreset preset)
    {
        switch (preset)
        {
            case GraphicsPreset.Low:
                QualitySettings.SetQualityLevel(0, true);
                break;
            case GraphicsPreset.Medium:
                QualitySettings.SetQualityLevel(Mathf.Min(2, QualitySettings.names.Length - 1), true);
                break;
            case GraphicsPreset.High:
                QualitySettings.SetQualityLevel(QualitySettings.names.Length - 1, true);
                break;
        }
    }

    private Slider CreateLabeledSlider(Transform parent, string label, Vector2 anchor)
    {
        var go = new GameObject(label.Replace(" ", "") + "Slider");
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchor; rt.anchorMax = anchor; rt.sizeDelta = new Vector2(300, 30);
        var slider = go.AddComponent<Slider>();
        var txtGo = new GameObject("Label");
        txtGo.transform.SetParent(go.transform, false);
        var txt = txtGo.AddComponent<Text>();
        txt.text = label;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.color = Color.white;
        var txtRt = txtGo.GetComponent<RectTransform>(); txtRt.anchorMin = new Vector2(0f, 1f); txtRt.anchorMax = new Vector2(0f, 1f); txtRt.anchoredPosition = new Vector2(-120, 10);
        return slider;
    }

    private Toggle CreateLabeledToggle(Transform parent, string label, Vector2 anchor)
    {
        var go = new GameObject(label.Replace(" ", "") + "Toggle");
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchor; rt.anchorMax = anchor; rt.sizeDelta = new Vector2(200, 30);
        var toggle = go.AddComponent<Toggle>();
        var txtGo = new GameObject("Label");
        txtGo.transform.SetParent(go.transform, false);
        var txt = txtGo.AddComponent<Text>();
        txt.text = label;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.color = Color.white;
        var txtRt = txtGo.GetComponent<RectTransform>(); txtRt.anchorMin = new Vector2(0f, 1f); txtRt.anchorMax = new Vector2(0f, 1f); txtRt.anchoredPosition = new Vector2(20, 10);
        return toggle;
    }

    private Dropdown CreateLabeledDropdown(Transform parent, string label, Vector2 anchor, string[] options)
    {
        var go = new GameObject(label.Replace(" ", "") + "Dropdown");
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchor; rt.anchorMax = anchor; rt.sizeDelta = new Vector2(240, 30);
        var dd = go.AddComponent<Dropdown>();
        dd.AddOptions(new System.Collections.Generic.List<string>(options));
        var txtGo = new GameObject("Label");
        txtGo.transform.SetParent(go.transform, false);
        var txt = txtGo.AddComponent<Text>();
        txt.text = label;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.color = Color.white;
        var txtRt = txtGo.GetComponent<RectTransform>(); txtRt.anchorMin = new Vector2(0f, 1f); txtRt.anchorMax = new Vector2(0f, 1f); txtRt.anchoredPosition = new Vector2(-120, 10);
        return dd;
    }
}


