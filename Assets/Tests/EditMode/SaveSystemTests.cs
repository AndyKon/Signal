using System.IO;
using NUnit.Framework;
using Signal.Core;

namespace Signal.Tests.EditMode
{
    public class SaveSystemTests
    {
        private string _testSaveDir;

        [SetUp]
        public void SetUp()
        {
            _testSaveDir = Path.Combine(Path.GetTempPath(), "SignalTestSaves");
            if (Directory.Exists(_testSaveDir))
                Directory.Delete(_testSaveDir, true);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_testSaveDir))
                Directory.Delete(_testSaveDir, true);
        }

        [Test]
        public void Save_CreatesFile()
        {
            var system = new SaveSystem(_testSaveDir, 5);
            var data = new SaveData { CurrentScene = "test_scene" };
            system.Save(0, data);

            Assert.IsTrue(system.SlotExists(0));
        }

        [Test]
        public void Load_RestoresSavedData()
        {
            var system = new SaveSystem(_testSaveDir, 5);
            var data = new SaveData
            {
                CurrentScene = "Section1_Hub_Room2",
                Flags = new() { "flag_a", "flag_b" },
                PoweredSections = new() { 1 },
                InventoryItems = new() { "keycard_a" }
            };
            system.Save(0, data);
            SaveData loaded = system.Load(0);

            Assert.AreEqual("Section1_Hub_Room2", loaded.CurrentScene);
            Assert.Contains("flag_a", loaded.Flags);
            Assert.Contains("flag_b", loaded.Flags);
            Assert.Contains(1, loaded.PoweredSections);
            Assert.Contains("keycard_a", loaded.InventoryItems);
        }

        [Test]
        public void SlotExists_FalseForEmptySlot()
        {
            var system = new SaveSystem(_testSaveDir, 5);
            Assert.IsFalse(system.SlotExists(0));
        }

        [Test]
        public void Save_InvalidSlot_ReturnsFalse()
        {
            var system = new SaveSystem(_testSaveDir, 5);
            var data = new SaveData();
            Assert.IsFalse(system.Save(5, data));
            Assert.IsFalse(system.Save(-1, data));
        }

        [Test]
        public void Load_EmptySlot_ReturnsNull()
        {
            var system = new SaveSystem(_testSaveDir, 5);
            Assert.IsNull(system.Load(0));
        }

        [Test]
        public void Delete_RemovesSlot()
        {
            var system = new SaveSystem(_testSaveDir, 5);
            system.Save(0, new SaveData { CurrentScene = "test" });
            Assert.IsTrue(system.SlotExists(0));

            system.Delete(0);
            Assert.IsFalse(system.SlotExists(0));
        }

        [Test]
        public void MultipleSlotsWorkIndependently()
        {
            var system = new SaveSystem(_testSaveDir, 5);
            system.Save(0, new SaveData { CurrentScene = "scene_a" });
            system.Save(2, new SaveData { CurrentScene = "scene_b" });

            Assert.AreEqual("scene_a", system.Load(0).CurrentScene);
            Assert.AreEqual("scene_b", system.Load(2).CurrentScene);
            Assert.IsFalse(system.SlotExists(1));
        }
    }
}
