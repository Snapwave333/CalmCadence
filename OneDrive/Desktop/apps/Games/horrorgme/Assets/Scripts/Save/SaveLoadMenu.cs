using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Minimal pause menu with Save/Load buttons for proof-of-concept.
/// Toggle with Escape; supports multiple save slots.
/// </summary>
public class SaveLoadMenu : MonoBehaviour
{
    [Header("Settings")]
    public KeyCode toggleKey = KeyCode.Escape;
    public int numberOfSlots = 3;

    private GameObject _root;
    private List<Button> _saveButtons = new List<Button>();
    private List<Button> _loadButtons = new List<Button>();
    private List<Button> _deleteButtons = new List<Button>();
    private List<Text> _slotLabels = new List<Text>();

    private void Start()
    {
        BuildUi();
        _root.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            _root.SetActive(!_root.activeSelf);
            if (_root.activeSelf)
            {
                RefreshSlotInfo();
            }
        }
    }

    private void RefreshSlotInfo()
    {
        for (int i = 0; i < numberOfSlots; i++)
        {
            int slot = i + 1;
            if (SaveManager.SaveExists(slot))
            {
                if (SaveManager.TryLoad(slot, out var data))
                {
                    _slotLabels[i].text = $"Slot {slot}: {data.sceneName} ({data.timestampUtc})";
                    _loadButtons[i].interactable = true;
                    _deleteButtons[i].interactable = true;
                }
                else
                {
                    _slotLabels[i].text = $"Slot {slot}: (corrupted)";
                    _loadButtons[i].interactable = false;
                    _deleteButtons[i].interactable = true;
                }
            }
            else
            {
                _slotLabels[i].text = $"Slot {slot}: Empty";
                _loadButtons[i].interactable = false;
                _deleteButtons[i].interactable = false;
            }
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
        rt.anchorMin = new Vector2(0.2f, 0.15f);
        rt.anchorMax = new Vector2(0.8f, 0.85f);
        var img = panel.AddComponent<Image>();
        img.color = new Color(0f, 0f, 0f, 0.85f);

        // Title
        var titleGo = new GameObject("Title");
        titleGo.transform.SetParent(panel.transform, false);
        var titleTxt = titleGo.AddComponent<Text>();
        titleTxt.text = "Save / Load Game";
        titleTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleTxt.fontSize = 24;
        titleTxt.color = Color.white;
        titleTxt.alignment = TextAnchor.MiddleCenter;
        var titleRt = titleGo.GetComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0f, 0.9f);
        titleRt.anchorMax = new Vector2(1f, 1f);
        titleRt.offsetMin = Vector2.zero;
        titleRt.offsetMax = Vector2.zero;

        // Create slots
        float slotHeight = 0.8f / numberOfSlots;
        for (int i = 0; i < numberOfSlots; i++)
        {
            int slot = i + 1;
            float yMax = 0.85f - (i * slotHeight);
            float yMin = yMax - slotHeight + 0.02f;

            // Slot container
            var slotGo = new GameObject($"Slot{slot}");
            slotGo.transform.SetParent(panel.transform, false);
            var slotRt = slotGo.AddComponent<RectTransform>();
            slotRt.anchorMin = new Vector2(0.05f, yMin);
            slotRt.anchorMax = new Vector2(0.95f, yMax);
            slotRt.offsetMin = Vector2.zero;
            slotRt.offsetMax = Vector2.zero;

            // Slot label
            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(slotGo.transform, false);
            var labelTxt = labelGo.AddComponent<Text>();
            labelTxt.text = $"Slot {slot}: Empty";
            labelTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            labelTxt.fontSize = 16;
            labelTxt.color = Color.white;
            labelTxt.alignment = TextAnchor.MiddleLeft;
            var labelRt = labelGo.GetComponent<RectTransform>();
            labelRt.anchorMin = new Vector2(0f, 0.5f);
            labelRt.anchorMax = new Vector2(0.5f, 1f);
            labelRt.offsetMin = Vector2.zero;
            labelRt.offsetMax = Vector2.zero;
            _slotLabels.Add(labelTxt);

            // Save button
            var saveBtn = CreateButton(slotGo.transform, "Save", new Vector2(0.55f, 0.5f), new Vector2(0.7f, 1f));
            int capturedSlot = slot;
            saveBtn.onClick.AddListener(() => OnSave(capturedSlot));
            _saveButtons.Add(saveBtn);

            // Load button
            var loadBtn = CreateButton(slotGo.transform, "Load", new Vector2(0.72f, 0.5f), new Vector2(0.87f, 1f));
            loadBtn.onClick.AddListener(() => OnLoad(capturedSlot));
            loadBtn.interactable = false;
            _loadButtons.Add(loadBtn);

            // Delete button
            var deleteBtn = CreateButton(slotGo.transform, "X", new Vector2(0.89f, 0.5f), new Vector2(0.98f, 1f));
            deleteBtn.onClick.AddListener(() => OnDelete(capturedSlot));
            deleteBtn.interactable = false;
            _deleteButtons.Add(deleteBtn);
        }

        // Close button
        var closeBtn = CreateButton(panel.transform, "Close", new Vector2(0.4f, 0.02f), new Vector2(0.6f, 0.08f));
        closeBtn.onClick.AddListener(() => _root.SetActive(false));

        _root = canvasGo;
    }

    private Button CreateButton(Transform parent, string label, Vector2 anchorMin, Vector2 anchorMax)
    {
        var go = new GameObject(label + "Button");
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        var img = go.AddComponent<Image>();
        img.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        var btn = go.AddComponent<Button>();
        var txtGo = new GameObject("Text");
        txtGo.transform.SetParent(go.transform, false);
        var txt = txtGo.AddComponent<Text>();
        txt.text = label;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.fontSize = 14;
        var txtRt = txtGo.GetComponent<RectTransform>();
        txtRt.anchorMin = Vector2.zero;
        txtRt.anchorMax = Vector2.one;
        txtRt.offsetMin = new Vector2(4, 4);
        txtRt.offsetMax = new Vector2(-4, -4);
        return btn;
    }

    private void OnSave(int slot)
    {
        try
        {
            var data = new SaveData(SaveManager.CurrentSaveVersion);
            var scene = SceneManager.GetActiveScene();
            data.sceneName = scene.name;

            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                data.playerPosition = player.transform.position;
            }
            else
            {
                var cam = Camera.main;
                data.playerPosition = cam != null ? cam.transform.position : Vector3.zero;
            }

            // Copy choice flags from ChoiceManager if available
            var choiceManager = ChoiceManager.Instance;
            // ChoiceManager.Instance already handles lazy creation

            SaveManager.Save(slot, data);
            Debug.Log($"SaveLoadMenu: Saved to slot {slot}");
            RefreshSlotInfo();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"SaveLoadMenu: Failed to save slot {slot}: {ex.Message}");
        }
    }

    private void OnLoad(int slot)
    {
        if (!SaveManager.TryLoad(slot, out var data))
        {
            Debug.LogWarning($"SaveLoadMenu: Failed to load slot {slot}");
            return;
        }

        if (string.IsNullOrEmpty(data.sceneName))
        {
            Debug.LogWarning($"SaveLoadMenu: Save data has no scene name");
            return;
        }

        // Validate scene exists before loading
        if (!IsSceneValid(data.sceneName))
        {
            Debug.LogError($"SaveLoadMenu: Scene '{data.sceneName}' does not exist in build settings");
            return;
        }

        try
        {
            // Store position for after scene load
            var targetPosition = data.playerPosition;
            var targetScene = data.sceneName;

            // Subscribe to scene loaded event
            SceneManager.sceneLoaded += OnSceneLoadedHandler;

            void OnSceneLoadedHandler(Scene loadedScene, LoadSceneMode mode)
            {
                SceneManager.sceneLoaded -= OnSceneLoadedHandler;

                if (loadedScene.name == targetScene)
                {
                    // Apply player position
                    var player = GameObject.FindGameObjectWithTag("Player");
                    if (player != null)
                    {
                        player.transform.position = targetPosition;
                    }
                    else
                    {
                        var cam = Camera.main;
                        if (cam != null)
                        {
                            cam.transform.position = targetPosition;
                        }
                    }

                    Debug.Log($"SaveLoadMenu: Loaded from slot {slot}, scene: {targetScene}");
                }
            }

            SceneManager.LoadScene(data.sceneName);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"SaveLoadMenu: Failed to load scene '{data.sceneName}': {ex.Message}");
        }
    }

    private void OnDelete(int slot)
    {
        if (SaveManager.DeleteSave(slot))
        {
            Debug.Log($"SaveLoadMenu: Deleted slot {slot}");
            RefreshSlotInfo();
        }
        else
        {
            Debug.LogWarning($"SaveLoadMenu: Failed to delete slot {slot}");
        }
    }

    private bool IsSceneValid(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
            return false;

        // Check if scene is in build settings
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < sceneCount; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameInBuild = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneNameInBuild == sceneName)
            {
                return true;
            }
        }

        return false;
    }
}
