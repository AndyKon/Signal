using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Signal.Interaction;
using Signal.Narrative;
using Signal.Inventory;
using Signal.Audio;
using Signal.UI;

namespace Signal.Editor
{
    public static class VerticalSliceBuilder
    {
        [MenuItem("Signal/Build Vertical Slice")]
        public static void BuildVerticalSlice()
        {
            if (!EditorUtility.DisplayDialog(
                "Build Vertical Slice",
                "This will create:\n" +
                "- 3 Hub room scenes\n" +
                "- Placeholder background textures\n" +
                "- NarrativeEntry assets\n" +
                "- ItemDefinition assets\n" +
                "- MainMenu scene\n\n" +
                "Existing files will be overwritten. Continue?",
                "Build", "Cancel"))
            {
                return;
            }

            CreatePlaceholderBackgrounds();
            CreateItemDefinitions();
            CreateNarrativeEntries();
            CreateMainMenuScene();
            CreateRoom1();
            CreateRoom2();
            CreateRoom3();
            UpdateBuildSettings();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Done", "Vertical slice built successfully!\n\nScenes added to Build Settings.\nOpen MainMenu scene and press Play to test.", "OK");
        }

        private static Texture2D CreateSolidTexture(string name, Color color, string path)
        {
            var tex = new Texture2D(320, 180, TextureFormat.RGBA32, false);
            var pixels = new Color[320 * 180];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = color;
            tex.SetPixels(pixels);
            tex.Apply();

            byte[] png = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, png);
            AssetDatabase.ImportAsset(GetRelativePath(path));

            // Configure as sprite
            var importer = AssetImporter.GetAtPath(GetRelativePath(path)) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = 16;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Texture2D>(GetRelativePath(path));
        }

        private static void CreatePlaceholderBackgrounds()
        {
            string dir = Application.dataPath + "/Art/Backgrounds";
            System.IO.Directory.CreateDirectory(dir);

            CreateSolidTexture("hub_room1", new Color(0.08f, 0.1f, 0.18f), dir + "/hub_room1_placeholder.png");
            CreateSolidTexture("hub_room2", new Color(0.1f, 0.12f, 0.2f), dir + "/hub_room2_placeholder.png");
            CreateSolidTexture("hub_room3", new Color(0.12f, 0.08f, 0.06f), dir + "/hub_room3_placeholder.png");
        }

        private static void CreateItemDefinitions()
        {
            string dir = "Assets/Data/Items";
            System.IO.Directory.CreateDirectory(Application.dataPath + "/Data/Items");

            var keycard = ScriptableObject.CreateInstance<ItemDefinition>();
            keycard.ItemId = "keycard_hub";
            keycard.DisplayName = "Hub Keycard";
            keycard.Description = "A standard issue keycard for the Hub section.";
            AssetDatabase.CreateAsset(keycard, dir + "/keycard_hub.asset");
        }

        private static void CreateNarrativeEntries()
        {
            string dir = "Assets/Data/Narrative";
            System.IO.Directory.CreateDirectory(Application.dataPath + "/Data/Narrative");

            var reboot = ScriptableObject.CreateInstance<NarrativeEntry>();
            reboot.EntryId = "hub_reboot_01";
            reboot.Text = "Systems initializing... ARIA unit 7 online. Directive received: Restore station power. Proceed to main corridor.";
            reboot.FlagToSet = "";
            AssetDatabase.CreateAsset(reboot, dir + "/hub_reboot_01.asset");

            var optional = ScriptableObject.CreateInstance<NarrativeEntry>();
            optional.EntryId = "hub_optional_terminal";
            optional.Text = "Maintenance log 4471: Routine diagnostic. All crew accounted for. No anomalies.\n\n...but the timestamp is three years after the last crew rotation was scheduled.";
            optional.FlagToSet = "found_hub_log_01";
            AssetDatabase.CreateAsset(optional, dir + "/hub_optional_terminal.asset");

            var powerOn = ScriptableObject.CreateInstance<NarrativeEntry>();
            powerOn.EntryId = "hub_power_console";
            powerOn.Text = "Hub power restored. Life support systems coming online. Section 2 access unlocked.\n\nSomething hums in the walls. It sounds almost... familiar.";
            powerOn.FlagToSet = "hub_power_restored";
            AssetDatabase.CreateAsset(powerOn, dir + "/hub_power_console.asset");
        }

        private static void CreateMainMenuScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Camera
            var camObj = new GameObject("Main Camera");
            var cam = camObj.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5.625f; // 180 / 32
            cam.backgroundColor = Color.black;
            cam.clearFlags = CameraClearFlags.SolidColor;
            camObj.tag = "MainCamera";

            // Canvas
            var canvas = CreateCanvas("MenuCanvas", 0);

            // Title
            var titleObj = CreateTMPText(canvas.transform, "TitleText", "SIGNAL",
                new Vector2(0, 100), new Vector2(400, 80), 48);

            // Buttons
            var newGameBtn = CreateButton(canvas.transform, "NewGameButton", "New Game", new Vector2(0, 0));
            var loadGameBtn = CreateButton(canvas.transform, "LoadGameButton", "Load Game", new Vector2(0, -60));
            var quitBtn = CreateButton(canvas.transform, "QuitButton", "Quit", new Vector2(0, -120));

            // Save slot panel (inactive)
            var slotPanel = CreatePanel(canvas.transform, "SaveSlotPanel", new Vector2(0, 0), new Vector2(350, 300));
            slotPanel.SetActive(false);
            var slotContainer = new GameObject("SlotContainer");
            slotContainer.transform.SetParent(slotPanel.transform, false);
            var slotRect = slotContainer.AddComponent<RectTransform>();
            slotRect.anchorMin = Vector2.zero;
            slotRect.anchorMax = Vector2.one;
            slotRect.offsetMin = new Vector2(10, 10);
            slotRect.offsetMax = new Vector2(-10, -10);
            var slotLayout = slotContainer.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
            slotLayout.spacing = 5;
            slotLayout.childAlignment = TextAnchor.UpperCenter;

            // Wire up SaveSlotUI
            var saveSlotUI = slotPanel.AddComponent<SaveSlotUI>();
            SetSerializedField(saveSlotUI, "_slotContainer", slotContainer.transform);
            var slotButtonPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/SlotButton.prefab");
            SetSerializedField(saveSlotUI, "_slotButtonPrefab", slotButtonPrefab);

            // Wire up MainMenuUI
            var menuUI = canvas.AddComponent<UI.MainMenuUI>();
            SetSerializedField(menuUI, "_newGameButton", newGameBtn.GetComponent<UnityEngine.UI.Button>());
            SetSerializedField(menuUI, "_loadGameButton", loadGameBtn.GetComponent<UnityEngine.UI.Button>());
            SetSerializedField(menuUI, "_quitButton", quitBtn.GetComponent<UnityEngine.UI.Button>());
            SetSerializedField(menuUI, "_saveSlotUI", saveSlotUI);
            SetSerializedField(menuUI, "_firstSceneName", "Section1_Hub_Room1");

            EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainMenu.unity");
        }

        private static void CreateRoom1()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            SetupRoomCamera();

            // Background
            CreateBackground("Assets/Art/Backgrounds/hub_room1_placeholder.png");

            // Intro terminal hotspot (center)
            var introTerminal = CreateHotspot("IntroTerminal", new Vector3(0, 0, 0), new Vector2(3, 2));
            var introHotspot = introTerminal.GetComponent<Hotspot>();
            var introAction = new HotspotAction
            {
                Type = HotspotType.Narration,
                NarrativeEntryId = "hub_reboot_01"
            };
            SetSerializedField(introHotspot, "_action", introAction);

            // Optional terminal (left side)
            var optTerminal = CreateHotspot("OptionalTerminal", new Vector3(-7, 0, 0), new Vector2(2, 2));
            var optHotspot = optTerminal.GetComponent<Hotspot>();
            var optAction = new HotspotAction
            {
                Type = HotspotType.Narration,
                NarrativeEntryId = "hub_optional_terminal"
            };
            SetSerializedField(optHotspot, "_action", optAction);

            // Door to Room 2 (right side)
            var door = CreateHotspot("DoorToRoom2", new Vector3(9, 0, 0), new Vector2(1, 3));
            var doorHotspot = door.GetComponent<Hotspot>();
            var doorAction = new HotspotAction
            {
                Type = HotspotType.Door,
                TargetScene = "Section1_Hub_Room2",
                ExamineText = ""
            };
            SetSerializedField(doorHotspot, "_action", doorAction);

            // Label objects for clarity in editor
            CreateLabel("< Optional Terminal", new Vector3(-7, -2, 0));
            CreateLabel("[ Intro Terminal ]", new Vector3(0, -2, 0));
            CreateLabel("Door >", new Vector3(9, -2, 0));

            // Scene audio placeholder
            var audioObj = new GameObject("SceneAudio");
            audioObj.AddComponent<SceneAudio>();

            EditorSceneManager.SaveScene(scene, "Assets/Scenes/Section1_Hub_Room1.unity");
        }

        private static void CreateRoom2()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            SetupRoomCamera();

            CreateBackground("Assets/Art/Backgrounds/hub_room2_placeholder.png");

            // Door back to Room 1
            var doorBack = CreateHotspot("DoorToRoom1", new Vector3(-9, 0, 0), new Vector2(1, 3));
            var doorBackHotspot = doorBack.GetComponent<Hotspot>();
            var doorBackAction = new HotspotAction
            {
                Type = HotspotType.Door,
                TargetScene = "Section1_Hub_Room1"
            };
            SetSerializedField(doorBackHotspot, "_action", doorBackAction);

            // Keycard pickup
            var keycard = CreateHotspot("KeycardPickup", new Vector3(0, -1, 0), new Vector2(1.5f, 1));
            var keycardHotspot = keycard.GetComponent<Hotspot>();
            var keycardAction = new HotspotAction
            {
                Type = HotspotType.PickUp,
                ExamineText = "A keycard. Might open the power room.",
                ItemToGrant = "keycard_hub",
                FlagToSet = "picked_up_hub_keycard"
            };
            SetSerializedField(keycardHotspot, "_action", keycardAction);
            var keycardCondition = new HotspotCondition
            {
                BlockedByFlag = "picked_up_hub_keycard"
            };
            SetSerializedField(keycardHotspot, "_condition", keycardCondition);

            // Door to Room 3 (locked, needs keycard)
            var doorForward = CreateHotspot("DoorToRoom3", new Vector3(9, 0, 0), new Vector2(1, 3));
            var doorForwardHotspot = doorForward.GetComponent<Hotspot>();
            var doorForwardAction = new HotspotAction
            {
                Type = HotspotType.Door,
                TargetScene = "Section1_Hub_Room3"
            };
            SetSerializedField(doorForwardHotspot, "_action", doorForwardAction);
            var doorForwardCondition = new HotspotCondition
            {
                RequiredItem = "keycard_hub"
            };
            SetSerializedField(doorForwardHotspot, "_condition", doorForwardCondition);

            // Labels
            CreateLabel("< Door Back", new Vector3(-9, -2, 0));
            CreateLabel("[ Keycard ]", new Vector3(0, -3, 0));
            CreateLabel("Door (Locked) >", new Vector3(9, -2, 0));

            var audioObj = new GameObject("SceneAudio");
            audioObj.AddComponent<SceneAudio>();

            EditorSceneManager.SaveScene(scene, "Assets/Scenes/Section1_Hub_Room2.unity");
        }

        private static void CreateRoom3()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            SetupRoomCamera();

            CreateBackground("Assets/Art/Backgrounds/hub_room3_placeholder.png");

            // Door back to Room 2
            var doorBack = CreateHotspot("DoorToRoom2", new Vector3(-9, 0, 0), new Vector2(1, 3));
            var doorBackHotspot = doorBack.GetComponent<Hotspot>();
            var doorBackAction = new HotspotAction
            {
                Type = HotspotType.Door,
                TargetScene = "Section1_Hub_Room2"
            };
            SetSerializedField(doorBackHotspot, "_action", doorBackAction);

            // Power console
            var console = CreateHotspot("PowerConsole", new Vector3(3, 0, 0), new Vector2(3, 2));
            var consoleHotspot = console.GetComponent<Hotspot>();
            var consoleAction = new HotspotAction
            {
                Type = HotspotType.Narration,
                NarrativeEntryId = "hub_power_console"
            };
            SetSerializedField(consoleHotspot, "_action", consoleAction);

            // Labels
            CreateLabel("< Door Back", new Vector3(-9, -2, 0));
            CreateLabel("[ Power Console ]", new Vector3(3, -2, 0));

            var audioObj = new GameObject("SceneAudio");
            audioObj.AddComponent<SceneAudio>();

            EditorSceneManager.SaveScene(scene, "Assets/Scenes/Section1_Hub_Room3.unity");
        }

        private static void UpdateBuildSettings()
        {
            var scenes = new[]
            {
                new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
                new EditorBuildSettingsScene("Assets/Scenes/Section1_Hub_Room1.unity", true),
                new EditorBuildSettingsScene("Assets/Scenes/Section1_Hub_Room2.unity", true),
                new EditorBuildSettingsScene("Assets/Scenes/Section1_Hub_Room3.unity", true),
            };
            EditorBuildSettings.scenes = scenes;
        }

        // --- Helpers ---

        private static void SetupRoomCamera()
        {
            var camObj = new GameObject("Main Camera");
            var cam = camObj.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5.625f;
            cam.backgroundColor = new Color(0.05f, 0.05f, 0.08f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            camObj.tag = "MainCamera";
        }

        private static void CreateBackground(string spritePath)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            if (sprite == null)
            {
                Debug.LogWarning($"Background sprite not found at {spritePath}");
                return;
            }

            var bgObj = new GameObject("Background");
            var sr = bgObj.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = -10;
        }

        private static GameObject CreateHotspot(string name, Vector3 position, Vector2 size)
        {
            var obj = new GameObject(name);
            obj.transform.position = position;
            obj.AddComponent<Hotspot>();
            var col = obj.AddComponent<BoxCollider2D>();
            col.size = size;

            // Visual indicator (white semi-transparent square)
            var visual = new GameObject("Highlight");
            visual.transform.SetParent(obj.transform, false);
            var sr = visual.AddComponent<SpriteRenderer>();
            sr.color = new Color(1, 1, 1, 0.15f);
            sr.sortingOrder = 5;
            // Use a white square sprite — create a tiny 4x4 texture
            var tex = new Texture2D(4, 4);
            var pixels = new Color[16];
            for (int i = 0; i < 16; i++) pixels[i] = Color.white;
            tex.SetPixels(pixels);
            tex.Apply();
            sr.sprite = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4);
            visual.transform.localScale = new Vector3(size.x, size.y, 1);
            sr.enabled = true; // Visible in editor for debugging; Hotspot.SetHighlight controls at runtime

            return obj;
        }

        private static void CreateLabel(string text, Vector3 position)
        {
            var obj = new GameObject($"Label: {text}");
            obj.transform.position = position;
            // Labels are just for editor clarity — no runtime component
        }

        private static GameObject CreateCanvas(string name, int sortOrder)
        {
            var canvasObj = new GameObject(name);
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortOrder;
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            return canvasObj;
        }

        private static GameObject CreateButton(Transform parent, string name, string label, Vector2 position)
        {
            var btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);
            var rect = btnObj.AddComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(200, 40);
            var image = btnObj.AddComponent<UnityEngine.UI.Image>();
            image.color = new Color(0.15f, 0.15f, 0.2f, 0.9f);
            btnObj.AddComponent<UnityEngine.UI.Button>();

            var textObj = new GameObject("Text");
            textObj.transform.SetParent(btnObj.transform, false);
            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            var tmp = textObj.AddComponent<TMPro.TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 18;
            tmp.alignment = TMPro.TextAlignmentOptions.Center;
            tmp.color = Color.white;

            return btnObj;
        }

        private static GameObject CreatePanel(Transform parent, string name, Vector2 position, Vector2 size)
        {
            var panelObj = new GameObject(name);
            panelObj.transform.SetParent(parent, false);
            var rect = panelObj.AddComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            var image = panelObj.AddComponent<UnityEngine.UI.Image>();
            image.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);
            return panelObj;
        }

        private static GameObject CreateTMPText(Transform parent, string name, string text,
            Vector2 position, Vector2 size, float fontSize)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            var tmp = obj.AddComponent<TMPro.TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.alignment = TMPro.TextAlignmentOptions.Center;
            tmp.color = Color.white;
            return obj;
        }

        private static string GetRelativePath(string absolutePath)
        {
            return "Assets" + absolutePath.Substring(Application.dataPath.Length);
        }

        private static void SetSerializedField(Object target, string fieldName, object value)
        {
            var so = new SerializedObject(target);
            var prop = so.FindProperty(fieldName);
            if (prop == null)
            {
                Debug.LogWarning($"Could not find field '{fieldName}' on {target.GetType().Name}");
                return;
            }

            switch (value)
            {
                case string s:
                    prop.stringValue = s;
                    break;
                case Object obj:
                    prop.objectReferenceValue = obj;
                    break;
                case HotspotAction action:
                    SetHotspotAction(prop, action);
                    break;
                case HotspotCondition condition:
                    SetHotspotCondition(prop, condition);
                    break;
            }
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetHotspotAction(SerializedProperty prop, HotspotAction action)
        {
            prop.FindPropertyRelative("Type").enumValueIndex = (int)action.Type;
            prop.FindPropertyRelative("ExamineText").stringValue = action.ExamineText ?? "";
            prop.FindPropertyRelative("ItemToGrant").stringValue = action.ItemToGrant ?? "";
            prop.FindPropertyRelative("ItemToConsume").stringValue = action.ItemToConsume ?? "";
            prop.FindPropertyRelative("FlagToSet").stringValue = action.FlagToSet ?? "";
            prop.FindPropertyRelative("TargetScene").stringValue = action.TargetScene ?? "";
            prop.FindPropertyRelative("IsNewSection").boolValue = action.IsNewSection;
            prop.FindPropertyRelative("NarrativeEntryId").stringValue = action.NarrativeEntryId ?? "";
        }

        private static void SetHotspotCondition(SerializedProperty prop, HotspotCondition condition)
        {
            prop.FindPropertyRelative("RequiredFlag").stringValue = condition.RequiredFlag ?? "";
            prop.FindPropertyRelative("RequiredItem").stringValue = condition.RequiredItem ?? "";
            prop.FindPropertyRelative("BlockedByFlag").stringValue = condition.BlockedByFlag ?? "";
        }
    }
}
