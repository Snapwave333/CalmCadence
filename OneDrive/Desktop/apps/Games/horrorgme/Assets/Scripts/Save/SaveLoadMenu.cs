using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Minimal pause menu with Save/Load buttons for proof-of-concept.
/// Toggle with Escape; writes to slot 1.
/// </summary>
public class SaveLoadMenu : MonoBehaviour
{
    private GameObject _root;

    private void Start()
    {
        BuildUi();
        _root.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _root.SetActive(!_root.activeSelf);
        }
    }

    private void BuildUi()
    {
        var canvasGo = new GameObject("SaveLoadCanvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGo.AddComponent<CanvasScaler>();
        canvasGo.AddComponent<GraphicRaycaster>();

        var panel = new GameObject("Panel");
        panel.transform.SetParent(canvasGo.transform, false);
        var rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.35f, 0.35f);
        rt.anchorMax = new Vector2(0.65f, 0.65f);
        var img = panel.AddComponent<Image>();
        img.color = new Color(0f, 0f, 0f, 0.8f);

        // Save button
        var saveBtn = CreateButton(panel.transform, "Save", new Vector2(0.5f, 0.65f));
        saveBtn.onClick.AddListener(OnSave);
        // Load button
        var loadBtn = CreateButton(panel.transform, "Load", new Vector2(0.5f, 0.45f));
        loadBtn.onClick.AddListener(OnLoad);
        // Close button
        var closeBtn = CreateButton(panel.transform, "Close", new Vector2(0.5f, 0.25f));
        closeBtn.onClick.AddListener(() => _root.SetActive(false));

        _root = canvasGo;
    }

    private Button CreateButton(Transform parent, string label, Vector2 anchor)
    {
        var go = new GameObject(label + "Button");
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.sizeDelta = new Vector2(220, 48);
        var img = go.AddComponent<Image>();
        img.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        var btn = go.AddComponent<Button>();
        var txtGo = new GameObject("Text");
        txtGo.transform.SetParent(go.transform, false);
        var txt = txtGo.AddComponent<Text>();
        txt.text = label;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        var txtRt = txtGo.GetComponent<RectTransform>();
        txtRt.anchorMin = Vector2.zero;
        txtRt.anchorMax = Vector2.one;
        txtRt.offsetMin = new Vector2(6, 6);
        txtRt.offsetMax = new Vector2(-6, -6);
        return btn;
    }

    private void OnSave()
    {
        var data = new SaveData(SaveManager.CurrentSaveVersion);
        var scene = SceneManager.GetActiveScene();
        data.sceneName = scene.name;
        var cam = Camera.main;
        data.playerPosition = cam != null ? cam.transform.position : Vector3.zero;
        SaveManager.Save(1, data);
    }

    private void OnLoad()
    {
        if (SaveManager.TryLoad(1, out var data))
        {
            if (!string.IsNullOrEmpty(data.sceneName))
            {
                SceneManager.LoadScene(data.sceneName);
            }
            var cam = Camera.main;
            if (cam != null)
            {
                cam.transform.position = data.playerPosition;
            }
        }
    }
}


