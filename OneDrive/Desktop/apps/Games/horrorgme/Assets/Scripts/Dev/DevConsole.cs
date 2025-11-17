using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Runtime developer overlay and console.
/// - Toggle with BackQuote (~) or on-screen toggle button (dev builds).
/// - Displays scene, time, choice flags, and command registry.
/// - Parses and executes commands via [DevCommand] methods.
/// </summary>
public class DevConsole : MonoBehaviour
{
    private static DevConsole _instance;
    public static DevConsole Instance => _instance;

    [Header("UI References")]
    public GameObject rootPanel;
    public Text sceneLabel;
    public Text timeLabel;
    public InputField commandInput;
    public Text outputText;
    public InputField commandFilterInput;
    public RectTransform commandListContent;
    public RectTransform flagsListContent;
    public GameObject listEntryPrefab;
    public Button toggleButton;

    [Header("Settings")]
    public KeyCode toggleKey = KeyCode.BackQuote;
    public int maxOutputLines = 256;
    public int maxListEntries = 100;

    private readonly Dictionary<string, object> _choiceFlags = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

    private class RegisteredCommand
    {
        public string name;
        public string description;
        public MethodInfo method;
        public UnityEngine.Object target; // MonoBehaviour instance or null for static
    }

    private readonly Dictionary<string, RegisteredCommand> _commands = new Dictionary<string, RegisteredCommand>(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> _outputLines = new List<string>(128);

    // Cache for static commands to avoid repeated reflection
    private static Dictionary<string, RegisteredCommand> _staticCommandsCache;
    private static bool _staticCommandsCached = false;

    // Object pooling for list entries
    private readonly Queue<GameObject> _entryPool = new Queue<GameObject>();
    private readonly List<GameObject> _activeFlagEntries = new List<GameObject>();
    private readonly List<GameObject> _activeCommandEntries = new List<GameObject>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        if (rootPanel == null)
        {
            BuildMinimalUi();
        }
        BuildRegistry();
        WireUi();
        HideOverlay();
        PrintLine("DevConsole ready. Type 'help' to list commands.");
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleOverlay();
        }

        if (rootPanel != null && rootPanel.activeSelf)
        {
            if (sceneLabel != null)
                sceneLabel.text = SceneManager.GetActiveScene().name;
            if (timeLabel != null)
                timeLabel.text = DateTime.Now.ToString("HH:mm:ss");
        }
    }

    private void WireUi()
    {
        if (commandInput != null)
        {
            commandInput.onEndEdit.AddListener(OnSubmit);
        }
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleOverlay);
        }
        if (commandFilterInput != null)
        {
            commandFilterInput.onValueChanged.AddListener(_ => RefreshCommandList());
        }

        RegisterBuiltIns();
        RefreshCommandList();
        RefreshFlagsList();
        RefreshOutput();
    }

    public void ToggleOverlay()
    {
        if (rootPanel == null) return;
        rootPanel.SetActive(!rootPanel.activeSelf);
    }

    public void ShowOverlay()
    {
        if (rootPanel == null) return;
        rootPanel.SetActive(true);
    }

    public void HideOverlay()
    {
        if (rootPanel == null) return;
        rootPanel.SetActive(false);
    }

    private void OnSubmit(string text)
    {
        if (!rootPanel.activeSelf) return;
        if (string.IsNullOrWhiteSpace(text)) return;
        Execute(text);
        commandInput.text = string.Empty;
        commandInput.ActivateInputField();
    }

    public void Execute(string line)
    {
        PrintLine($"> {line}");
        var parts = Tokenize(line);
        if (parts.Length == 0) return;
        var cmd = parts[0];
        var args = parts.Skip(1).ToArray();

        if (string.Equals(cmd, "help", StringComparison.OrdinalIgnoreCase))
        {
            PrintLine("Commands:");
            foreach (var c in _commands.Values.OrderBy(c => c.name))
                PrintLine($" - {c.name}: {c.description}");
            RefreshOutput();
            return;
        }

        if (!_commands.TryGetValue(cmd, out var entry))
        {
            PrintLine($"Unknown command: {cmd}");
            RefreshOutput();
            return;
        }

        try
        {
            var parameters = entry.method.GetParameters();
            object result = null;
            if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string[]))
            {
                result = entry.method.Invoke(entry.target, new object[] { args });
            }
            else if (parameters.Length == 0)
            {
                result = entry.method.Invoke(entry.target, null);
            }
            else
            {
                PrintLine("Handler signature not supported. Use void/string Handler(string[] args)");
            }

            if (result is string s && !string.IsNullOrEmpty(s))
            {
                PrintLine(s);
            }
        }
        catch (TargetInvocationException ex)
        {
            PrintLine($"Error: {ex.InnerException?.Message ?? ex.Message}");
        }
        catch (Exception ex)
        {
            PrintLine($"Error: {ex.Message}");
        }

        RefreshOutput();
    }

    private static string[] Tokenize(string input)
    {
        // Simple split supporting quoted strings
        var tokens = new List<string>();
        bool inQuotes = false;
        var current = new System.Text.StringBuilder();
        foreach (var ch in input)
        {
            if (ch == '"')
            {
                inQuotes = !inQuotes;
                continue;
            }
            if (!inQuotes && char.IsWhiteSpace(ch))
            {
                if (current.Length > 0)
                {
                    tokens.Add(current.ToString());
                    current.Length = 0;
                }
            }
            else
            {
                current.Append(ch);
            }
        }
        if (current.Length > 0) tokens.Add(current.ToString());
        return tokens.ToArray();
    }

    private void PrintLine(string line)
    {
        _outputLines.Add(line);
        if (_outputLines.Count > maxOutputLines)
        {
            var removed = _outputLines.Count - maxOutputLines;
            _outputLines.RemoveRange(0, removed);
            Debug.LogWarning($"DevConsole: Output buffer exceeded {maxOutputLines} lines, oldest {removed} lines dropped");
        }
    }

    private void RefreshOutput()
    {
        if (outputText != null)
            outputText.text = string.Join("\n", _outputLines);
    }

    public IReadOnlyDictionary<string, object> ChoiceFlags => _choiceFlags;

    public void SetFlag(string key, object value)
    {
        _choiceFlags[key] = value;
        RefreshFlagsList();
    }

    public bool TryGetFlag<T>(string key, out T value)
    {
        if (_choiceFlags.TryGetValue(key, out var obj) && obj is T t)
        {
            value = t;
            return true;
        }
        value = default(T);
        return false;
    }

    private void RefreshFlagsList()
    {
        if (flagsListContent == null) return;

        // Return active entries to pool
        foreach (var entry in _activeFlagEntries)
        {
            if (entry != null)
            {
                entry.SetActive(false);
                _entryPool.Enqueue(entry);
            }
        }
        _activeFlagEntries.Clear();

        int count = 0;
        foreach (var kv in _choiceFlags.OrderBy(k => k.Key))
        {
            if (count >= maxListEntries)
            {
                var overflow = GetOrCreateEntry(flagsListContent);
                overflow.GetComponentInChildren<Text>().text = $"... and {_choiceFlags.Count - count} more";
                _activeFlagEntries.Add(overflow);
                break;
            }

            var go = GetOrCreateEntry(flagsListContent);
            var txt = go.GetComponentInChildren<Text>();
            if (txt != null) txt.text = kv.Key + ": " + (kv.Value == null ? "<null>" : kv.Value.ToString());
            _activeFlagEntries.Add(go);
            count++;
        }
    }

    private void RefreshCommandList()
    {
        if (commandListContent == null) return;

        // Return active entries to pool
        foreach (var entry in _activeCommandEntries)
        {
            if (entry != null)
            {
                entry.SetActive(false);
                _entryPool.Enqueue(entry);
            }
        }
        _activeCommandEntries.Clear();

        string filter = commandFilterInput != null ? commandFilterInput.text : string.Empty;
        IEnumerable<RegisteredCommand> items = _commands.Values;
        if (!string.IsNullOrWhiteSpace(filter))
        {
            items = items.Where(c => c.name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                     (!string.IsNullOrEmpty(c.description) && c.description.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0));
        }

        int count = 0;
        foreach (var c in items.OrderBy(c => c.name))
        {
            if (count >= maxListEntries)
            {
                var overflow = GetOrCreateEntry(commandListContent);
                overflow.GetComponentInChildren<Text>().text = $"... and more (use filter)";
                _activeCommandEntries.Add(overflow);
                break;
            }

            var go = GetOrCreateEntry(commandListContent);
            var txt = go.GetComponentInChildren<Text>();
            if (txt != null) txt.text = c.name + " — " + c.description;
            _activeCommandEntries.Add(go);
            count++;
        }
    }

    private GameObject GetOrCreateEntry(Transform parent)
    {
        if (_entryPool.Count > 0)
        {
            var pooled = _entryPool.Dequeue();
            pooled.transform.SetParent(parent, false);
            pooled.SetActive(true);
            return pooled;
        }

        return CreateListEntry(parent, "");
    }

    private GameObject CreateListEntry(Transform parent, string text)
    {
        if (listEntryPrefab != null)
        {
            var go = Instantiate(listEntryPrefab, parent);
            var txt = go.GetComponentInChildren<Text>();
            if (txt != null) txt.text = text;
            return go;
        }

        var entry = new GameObject("Entry");
        entry.transform.SetParent(parent, false);
        var t = entry.AddComponent<Text>();
        t.text = text;
        t.fontSize = 14;
        t.color = Color.white;
        t.alignment = TextAnchor.MiddleLeft;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        var rt = entry.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 20);
        return entry;
    }

    private void BuildMinimalUi()
    {
        // Create a simple overlay Canvas and wiring so the console is usable without a prefab
        var canvasGo = new GameObject("DevOverlayCanvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGo.AddComponent<CanvasScaler>();
        canvasGo.AddComponent<GraphicRaycaster>();

        var panel = new GameObject("DevOverlayPanel");
        panel.transform.SetParent(canvasGo.transform, false);
        var panelRt = panel.AddComponent<RectTransform>();
        panelRt.anchorMin = new Vector2(0f, 0f);
        panelRt.anchorMax = new Vector2(0.5f, 1f);
        panelRt.offsetMin = new Vector2(10, 10);
        panelRt.offsetMax = new Vector2(10, -10);
        var panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0f, 0f, 0f, 0.6f);

        rootPanel = panel;

        sceneLabel = CreateLabel(panel.transform, "Scene", new Vector2(10, -10));
        timeLabel = CreateLabel(panel.transform, "00:00:00", new Vector2(120, -10));

        outputText = CreateMultiline(panel.transform, new Vector2(10, -40), new Vector2(-10, -160));
        commandInput = CreateInput(panel.transform, new Vector2(10, -140), new Vector2(-10, -110));
        commandFilterInput = CreateInput(panel.transform, new Vector2(10, -170), new Vector2(-10, -140));

        // Simple vertical list containers
        var commandsRt = new GameObject("Commands").AddComponent<RectTransform>();
        commandsRt.SetParent(panel.transform, false);
        commandsRt.anchorMin = new Vector2(0f, 0f);
        commandsRt.anchorMax = new Vector2(1f, 0f);
        commandsRt.offsetMin = new Vector2(10, 10);
        commandsRt.offsetMax = new Vector2(-10, 100);
        commandListContent = commandsRt;

        var flagsRt = new GameObject("Flags").AddComponent<RectTransform>();
        flagsRt.SetParent(panel.transform, false);
        flagsRt.anchorMin = new Vector2(0.5f, 0f);
        flagsRt.anchorMax = new Vector2(1f, 0f);
        flagsRt.offsetMin = new Vector2(10, 110);
        flagsRt.offsetMax = new Vector2(-10, 200);
        flagsListContent = flagsRt;

        // Toggle button
        var btnGo = new GameObject("ToggleButton");
        btnGo.transform.SetParent(canvasGo.transform, false);
        var btnImg = btnGo.AddComponent<Image>();
        btnImg.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        toggleButton = btnGo.AddComponent<Button>();
        var btnRt = btnGo.GetComponent<RectTransform>();
        btnRt.anchorMin = new Vector2(1f, 1f);
        btnRt.anchorMax = new Vector2(1f, 1f);
        btnRt.anchoredPosition = new Vector2(-40, -40);
        btnRt.sizeDelta = new Vector2(24, 24);
        var btnLabel = CreateLabel(btnGo.transform, "~", Vector2.zero);
        btnLabel.alignment = TextAnchor.MiddleCenter;
    }

    private Text CreateLabel(Transform parent, string text, Vector2 anchoredPos)
    {
        var go = new GameObject("Label");
        go.transform.SetParent(parent, false);
        var t = go.AddComponent<Text>();
        t.text = text;
        t.fontSize = 14;
        t.color = Color.white;
        t.alignment = TextAnchor.UpperLeft;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(100, 20);
        return t;
    }

    private Text CreateMultiline(Transform parent, Vector2 offsetMin, Vector2 offsetMax)
    {
        var go = new GameObject("Output");
        go.transform.SetParent(parent, false);
        var t = go.AddComponent<Text>();
        t.text = string.Empty;
        t.fontSize = 12;
        t.color = Color.white;
        t.alignment = TextAnchor.UpperLeft;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;
        return t;
    }

    private InputField CreateInput(Transform parent, Vector2 offsetMin, Vector2 offsetMax)
    {
        var go = new GameObject("Input");
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        var field = go.AddComponent<InputField>();
        var textGo = new GameObject("Text");
        textGo.transform.SetParent(go.transform, false);
        var txt = textGo.AddComponent<Text>();
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.color = Color.white;
        txt.fontSize = 14;
        field.textComponent = txt;
        var phGo = new GameObject("Placeholder");
        phGo.transform.SetParent(go.transform, false);
        var phTxt = phGo.AddComponent<Text>();
        phTxt.text = "type command...";
        phTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        phTxt.color = new Color(1f, 1f, 1f, 0.4f);
        field.placeholder = phTxt;
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;
        var textRt = textGo.GetComponent<RectTransform>();
        textRt.anchorMin = new Vector2(0f, 0f);
        textRt.anchorMax = new Vector2(1f, 1f);
        textRt.offsetMin = new Vector2(6, 6);
        textRt.offsetMax = new Vector2(-6, -6);
        var phRt = phGo.GetComponent<RectTransform>();
        phRt.anchorMin = new Vector2(0f, 0f);
        phRt.anchorMax = new Vector2(1f, 1f);
        phRt.offsetMin = new Vector2(6, 6);
        phRt.offsetMax = new Vector2(-6, -6);
        return field;
    }

    private void BuildRegistry()
    {
        _commands.Clear();

        // Scan loaded scene objects for instance methods
        foreach (var mb in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            RegisterCommandsOn(mb);
        }

        // Use cached static commands to avoid repeated expensive reflection
        if (!_staticCommandsCached)
        {
            _staticCommandsCache = new Dictionary<string, RegisteredCommand>(StringComparer.OrdinalIgnoreCase);
            ScanStaticCommands();
            _staticCommandsCached = true;
        }

        // Add cached static commands
        foreach (var kvp in _staticCommandsCache)
        {
            if (!_commands.ContainsKey(kvp.Key))
            {
                _commands[kvp.Key] = kvp.Value;
            }
        }
    }

    private void ScanStaticCommands()
    {
        // Whitelist only project assemblies for security
        var allowedAssemblyPrefixes = new[] { "Assembly-CSharp", "Assembly-CSharp-firstpass" };

        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            // Skip Unity and System assemblies
            var asmName = asm.GetName().Name;
            if (!allowedAssemblyPrefixes.Any(prefix => asmName.StartsWith(prefix)))
            {
                continue;
            }

            try
            {
                foreach (var type in asm.GetTypes())
                {
                    // Skip compiler-generated and internal types
                    if (type.IsNotPublic || type.IsNested || type.Name.StartsWith("<"))
                    {
                        continue;
                    }

                    foreach (var mi in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                    {
                        var attr = mi.GetCustomAttribute<DevCommandAttribute>();
                        if (attr != null)
                        {
                            AddStaticRegistryEntry(attr, mi);
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                Debug.LogWarning($"DevConsole: Failed to load some types from {asmName}: {ex.Message}");
            }
        }
    }

    private void RegisterCommandsOn(MonoBehaviour target)
    {
        if (target == null) return;

        var type = target.GetType();
        foreach (var mi in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            var attr = mi.GetCustomAttribute<DevCommandAttribute>();
            if (attr != null)
            {
                AddRegistryEntry(attr, mi, target);
            }
        }
    }

    private void AddRegistryEntry(DevCommandAttribute attr, MethodInfo mi, UnityEngine.Object target)
    {
        if (string.IsNullOrWhiteSpace(attr.Name)) return;
        if (_commands.ContainsKey(attr.Name)) return;
        _commands[attr.Name] = new RegisteredCommand
        {
            name = attr.Name,
            description = attr.Description ?? "",
            method = mi,
            target = target
        };
    }

    private void AddStaticRegistryEntry(DevCommandAttribute attr, MethodInfo mi)
    {
        if (string.IsNullOrWhiteSpace(attr.Name)) return;
        if (_staticCommandsCache.ContainsKey(attr.Name)) return;
        _staticCommandsCache[attr.Name] = new RegisteredCommand
        {
            name = attr.Name,
            description = attr.Description ?? "",
            method = mi,
            target = null
        };
    }

    private void RegisterBuiltIns()
    {
        // Built-ins implemented as local lambdas via hidden receiver
        RegisterLocal("trigger", "trigger <eventId> — stub event trigger", args =>
        {
            PrintLine("Trigger: " + string.Join(" ", args));
            return "ok";
        });

        RegisterLocal("spawn", "spawn <prefabName> — Resources.Load then Instantiate", args =>
        {
            if (args.Length < 1) return "usage: spawn <prefabName>";
            var prefab = Resources.Load<GameObject>(args[0]);
            if (prefab == null) return "prefab not found in Resources: " + args[0];
            var pos = Vector3.zero;
            var cam = Camera.main;
            if (cam != null) pos = cam.transform.position + cam.transform.forward * 2f;
            Instantiate(prefab, pos, Quaternion.identity);
            return "spawned " + args[0];
        });

        RegisterLocal("setflag", "setflag <key> <value> — sets choice flag", args =>
        {
            if (args.Length < 2) return "usage: setflag <key> <value>";
            var key = args[0];
            object value = ParseScalar(args[1]);
            SetFlag(key, value);
            return key + "=" + value;
        });

        RegisterLocal("teleport", "teleport <x> <y> <z> — moves Player or Camera", args =>
        {
            if (args.Length < 3) return "usage: teleport <x> <y> <z>";
            if (!float.TryParse(args[0], out var x) || !float.TryParse(args[1], out var y) || !float.TryParse(args[2], out var z))
                return "invalid coordinates";
            var target = GameObject.FindGameObjectWithTag("Player");
            if (target == null && Camera.main != null) target = Camera.main.gameObject;
            if (target == null) return "no target found";
            target.transform.position = new Vector3(x, y, z);
            return "teleported to (" + x + "," + y + "," + z + ")";
        });

        RegisterLocal("clear", "clear — clears console output", args =>
        {
            _outputLines.Clear();
            RefreshOutput();
            return "";
        });

        RegisterLocal("shownav", "shownav — toggle NavMesh visualization (Editor)", args =>
        {
#if UNITY_EDITOR
            var t = Type.GetType("UnityEditor.AI.NavMeshVisualizationSettings, UnityEditor");
            if (t != null)
            {
                var prop = t.GetProperty("showNavigation", BindingFlags.Public | BindingFlags.Static);
                if (prop != null)
                {
                    bool current = (bool)prop.GetValue(null, null);
                    prop.SetValue(null, !current, null);
                    return "NavMesh visualization: " + (!current);
                }
            }
            return "NavMesh visualization API not found";
#else
            return "shownav is editor-only";
#endif
        });
    }

    private static object ParseScalar(string raw)
    {
        if (bool.TryParse(raw, out var b)) return b;
        if (int.TryParse(raw, out var i)) return i;
        if (float.TryParse(raw, out var f)) return f;
        return raw;
    }

    private void RegisterLocal(string name, string description, Func<string[], string> handler)
    {
        // Wrap as pseudo command
        var local = new RegisteredCommand
        {
            name = name,
            description = description,
            method = handler.Method,
            target = handler.Target as UnityEngine.Object
        };
        _commands[name] = local;
    }
}
