using UnityEngine;

namespace Signal.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState State { get; private set; }
        public SaveSystem SaveSystem { get; private set; }

        private const int MaxSaveSlots = 5;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            State = new GameState();
            string saveDir = System.IO.Path.Combine(Application.persistentDataPath, "saves");
            SaveSystem = new SaveSystem(saveDir, MaxSaveSlots);
        }

        public void SaveToSlot(int slot)
        {
            SaveData data = State.ToSaveData();
            SaveSystem.Save(slot, data);
        }

        public bool LoadFromSlot(int slot)
        {
            SaveData data = SaveSystem.Load(slot);
            if (data == null) return false;
            State.LoadFromSaveData(data);
            return true;
        }

        public void NewGame()
        {
            State.Reset();
        }
    }
}
