using UnityEngine;
using UnityEditor;
using Signal.Core;
using Signal.Scene;
using Signal.Interaction;
using Signal.Inventory;
using Signal.Narrative;
using Signal.Audio;
using Signal.UI;

namespace Signal.Editor
{
    public static class RebuildManagersPrefab
    {
        [MenuItem("Signal/Rebuild Managers Prefab")]
        public static void Rebuild()
        {
            // Delete old prefab
            string prefabPath = "Assets/Resources/Managers.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
                AssetDatabase.DeleteAsset(prefabPath);

            // Root
            var root = new GameObject("Managers");

            // GameManager on root
            root.AddComponent<GameManager>();

            // InteractionManager on root
            root.AddComponent<InteractionManager>();

            // CursorManager on root
            root.AddComponent<CursorManager>();

            // InventoryManager on root
            root.AddComponent<InventoryManager>();

            // --- AudioManager child ---
            var audioObj = new GameObject("AudioManager");
            audioObj.transform.SetParent(root.transform);
            var audioMgr = audioObj.AddComponent<AudioManager>();

            var musicSource = audioObj.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;

            var ambienceSource = audioObj.AddComponent<AudioSource>();
            ambienceSource.loop = true;
            ambienceSource.playOnAwake = false;

            var sfxSource = audioObj.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;

            // Wire audio sources via SerializedObject
            var audioSO = new SerializedObject(audioMgr);
            audioSO.FindProperty("_musicSource").objectReferenceValue = musicSource;
            audioSO.FindProperty("_ambienceSource").objectReferenceValue = ambienceSource;
            audioSO.FindProperty("_sfxSource").objectReferenceValue = sfxSource;
            audioSO.ApplyModifiedPropertiesWithoutUndo();

            // --- TransitionCanvas child ---
            var transCanvas = CreateCanvas("TransitionCanvas", root.transform, 999);

            var overlay = new GameObject("Overlay");
            overlay.transform.SetParent(transCanvas.transform, false);
            var overlayRect = overlay.AddComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;
            var overlayImage = overlay.AddComponent<UnityEngine.UI.Image>();
            overlayImage.color = new Color(0, 0, 0, 0); // Transparent black

            var transOverlay = transCanvas.AddComponent<TransitionOverlay>();
            var transSO = new SerializedObject(transOverlay);
            transSO.FindProperty("_overlay").objectReferenceValue = overlayImage;
            transSO.ApplyModifiedPropertiesWithoutUndo();

            var sceneLoader = transCanvas.AddComponent<SceneLoader>();
            var slSO = new SerializedObject(sceneLoader);
            slSO.FindProperty("_overlay").objectReferenceValue = transOverlay;
            slSO.ApplyModifiedPropertiesWithoutUndo();

            // --- NarrativeCanvas child ---
            var narCanvas = CreateCanvas("NarrativeCanvas", root.transform, 100);

            var narPanel = new GameObject("NarrativePanel");
            narPanel.transform.SetParent(narCanvas.transform, false);
            var narPanelRect = narPanel.AddComponent<RectTransform>();
            narPanelRect.anchorMin = new Vector2(0, 0);
            narPanelRect.anchorMax = new Vector2(1, 0);
            narPanelRect.pivot = new Vector2(0.5f, 0);
            narPanelRect.sizeDelta = new Vector2(0, 120);
            narPanelRect.anchoredPosition = Vector2.zero;
            var narPanelImage = narPanel.AddComponent<UnityEngine.UI.Image>();
            narPanelImage.color = new Color(0, 0, 0, 0.78f);
            narPanel.SetActive(false); // Inactive by default

            var narTextObj = new GameObject("NarrativeText");
            narTextObj.transform.SetParent(narPanel.transform, false);
            var narTextRect = narTextObj.AddComponent<RectTransform>();
            narTextRect.anchorMin = Vector2.zero;
            narTextRect.anchorMax = Vector2.one;
            narTextRect.offsetMin = new Vector2(10, 10);
            narTextRect.offsetMax = new Vector2(-10, -10);
            var narTMP = narTextObj.AddComponent<TMPro.TextMeshProUGUI>();
            narTMP.fontSize = 14;
            narTMP.color = Color.white;
            narTMP.alignment = TMPro.TextAlignmentOptions.TopLeft;

            var voiceObj = new GameObject("VoiceSource");
            voiceObj.transform.SetParent(narCanvas.transform, false);
            var voiceSource = voiceObj.AddComponent<AudioSource>();
            voiceSource.playOnAwake = false;

            var narUI = narCanvas.AddComponent<NarrativeUI>();
            var narUISO = new SerializedObject(narUI);
            narUISO.FindProperty("_panel").objectReferenceValue = narPanel;
            narUISO.FindProperty("_textDisplay").objectReferenceValue = narTMP;
            narUISO.ApplyModifiedPropertiesWithoutUndo();

            var narMgr = narCanvas.AddComponent<NarrativeManager>();
            var narMgrSO = new SerializedObject(narMgr);
            narMgrSO.FindProperty("_narrativeUI").objectReferenceValue = narUI;
            narMgrSO.FindProperty("_voiceSource").objectReferenceValue = voiceSource;
            narMgrSO.ApplyModifiedPropertiesWithoutUndo();

            // --- PauseCanvas child ---
            var pauseCanvas = CreateCanvas("PauseCanvas", root.transform, 200);

            // Pause panel
            var pausePanel = CreatePanel("PausePanel", pauseCanvas.transform, new Vector2(300, 250));
            pausePanel.SetActive(false);
            var pauseLayout = pausePanel.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
            pauseLayout.spacing = 10;
            pauseLayout.childAlignment = TextAnchor.MiddleCenter;
            pauseLayout.childForceExpandWidth = true;
            pauseLayout.childForceExpandHeight = false;
            pauseLayout.padding = new RectOffset(20, 20, 30, 30);

            var resumeBtn = CreateUIButton("ResumeButton", "Resume", pausePanel.transform);
            var saveBtn = CreateUIButton("SaveButton", "Save", pausePanel.transform);
            var quitBtn = CreateUIButton("QuitToMenuButton", "Quit to Menu", pausePanel.transform);

            // Save slot panel
            var saveSlotPanel = CreatePanel("SaveSlotPanel", pauseCanvas.transform, new Vector2(350, 300));
            saveSlotPanel.SetActive(false);
            var slotContainer = new GameObject("SlotContainer");
            slotContainer.transform.SetParent(saveSlotPanel.transform, false);
            var scRect = slotContainer.AddComponent<RectTransform>();
            scRect.anchorMin = Vector2.zero;
            scRect.anchorMax = Vector2.one;
            scRect.offsetMin = new Vector2(10, 10);
            scRect.offsetMax = new Vector2(-10, -10);
            var scLayout = slotContainer.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
            scLayout.spacing = 5;
            scLayout.childAlignment = TextAnchor.UpperCenter;

            var saveSlotUI = saveSlotPanel.AddComponent<SaveSlotUI>();
            var ssuiSO = new SerializedObject(saveSlotUI);
            ssuiSO.FindProperty("_slotContainer").objectReferenceValue = slotContainer.transform;
            var slotBtnPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/SlotButton.prefab");
            ssuiSO.FindProperty("_slotButtonPrefab").objectReferenceValue = slotBtnPrefab;
            ssuiSO.ApplyModifiedPropertiesWithoutUndo();

            var pauseMenuUI = pauseCanvas.AddComponent<PauseMenuUI>();
            var pmuiSO = new SerializedObject(pauseMenuUI);
            pmuiSO.FindProperty("_pausePanel").objectReferenceValue = pausePanel;
            pmuiSO.FindProperty("_resumeButton").objectReferenceValue = resumeBtn.GetComponent<UnityEngine.UI.Button>();
            pmuiSO.FindProperty("_saveButton").objectReferenceValue = saveBtn.GetComponent<UnityEngine.UI.Button>();
            pmuiSO.FindProperty("_quitToMenuButton").objectReferenceValue = quitBtn.GetComponent<UnityEngine.UI.Button>();
            pmuiSO.FindProperty("_saveSlotUI").objectReferenceValue = saveSlotUI;
            pmuiSO.ApplyModifiedPropertiesWithoutUndo();

            // --- InventoryCanvas child ---
            var invCanvas = CreateCanvas("InventoryCanvas", root.transform, 50);

            var invBar = new GameObject("InventoryBar");
            invBar.transform.SetParent(invCanvas.transform, false);
            var invBarRect = invBar.AddComponent<RectTransform>();
            invBarRect.anchorMin = new Vector2(0, 1);
            invBarRect.anchorMax = new Vector2(1, 1);
            invBarRect.pivot = new Vector2(0.5f, 1);
            invBarRect.sizeDelta = new Vector2(0, 60);
            invBarRect.anchoredPosition = Vector2.zero;
            var invBarImage = invBar.AddComponent<UnityEngine.UI.Image>();
            invBarImage.color = new Color(0, 0, 0, 0.6f);
            invBar.SetActive(false);

            var invSlotContainer = new GameObject("SlotContainer");
            invSlotContainer.transform.SetParent(invBar.transform, false);
            var iscRect = invSlotContainer.AddComponent<RectTransform>();
            iscRect.anchorMin = Vector2.zero;
            iscRect.anchorMax = Vector2.one;
            iscRect.offsetMin = new Vector2(8, 4);
            iscRect.offsetMax = new Vector2(-8, -4);
            var iscLayout = invSlotContainer.AddComponent<UnityEngine.UI.HorizontalLayoutGroup>();
            iscLayout.spacing = 8;
            iscLayout.childAlignment = TextAnchor.MiddleLeft;

            var invUI = invCanvas.AddComponent<InventoryUI>();
            var invUISO = new SerializedObject(invUI);
            invUISO.FindProperty("_inventoryBar").objectReferenceValue = invBar;
            invUISO.FindProperty("_slotContainer").objectReferenceValue = invSlotContainer.transform;
            var itemSlotPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/ItemSlot.prefab");
            invUISO.FindProperty("_itemSlotPrefab").objectReferenceValue = itemSlotPrefab;
            invUISO.ApplyModifiedPropertiesWithoutUndo();

            // Save as prefab
            System.IO.Directory.CreateDirectory(Application.dataPath + "/Resources");
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            Object.DestroyImmediate(root);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Done", "Managers prefab rebuilt at Assets/Resources/Managers.prefab\n\nRemember to add NarrativeEntry and ItemDefinition assets to the prefab's lists.", "OK");
        }

        private static GameObject CreateCanvas(string name, Transform parent, int sortOrder)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var canvas = obj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortOrder;
            var scaler = obj.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
            obj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            return obj;
        }

        private static GameObject CreatePanel(string name, Transform parent, Vector2 size)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
            var image = obj.AddComponent<UnityEngine.UI.Image>();
            image.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);
            return obj;
        }

        private static GameObject CreateUIButton(string name, string label, Transform parent)
        {
            var btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);
            var rect = btnObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(200, 40);
            var image = btnObj.AddComponent<UnityEngine.UI.Image>();
            image.color = new Color(0.2f, 0.2f, 0.25f, 1f);
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
            tmp.fontSize = 16;
            tmp.alignment = TMPro.TextAlignmentOptions.Center;
            tmp.color = Color.white;

            return btnObj;
        }
    }
}
