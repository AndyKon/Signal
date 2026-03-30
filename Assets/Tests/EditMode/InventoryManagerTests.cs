using NUnit.Framework;
using Signal.Core;

namespace Signal.Tests.EditMode
{
    public class InventoryManagerTests
    {
        private GameState _state;

        [SetUp]
        public void SetUp()
        {
            _state = new GameState();
        }

        [Test]
        public void AddItem_ItemIsPresent()
        {
            _state.AddItem("keycard_a");
            Assert.IsTrue(_state.HasItem("keycard_a"));
        }

        [Test]
        public void RemoveItem_ItemIsGone()
        {
            _state.AddItem("keycard_a");
            _state.RemoveItem("keycard_a");
            Assert.IsFalse(_state.HasItem("keycard_a"));
        }

        [Test]
        public void AddItem_Duplicate_NoDoubleEntry()
        {
            _state.AddItem("keycard_a");
            _state.AddItem("keycard_a");
            Assert.AreEqual(1, _state.Inventory.Count);
        }

        [Test]
        public void HasItem_FalseForMissingItem()
        {
            Assert.IsFalse(_state.HasItem("nonexistent"));
        }

        [Test]
        public void Inventory_SurvivesRoundTrip()
        {
            _state.AddItem("keycard_a");
            _state.AddItem("data_chip_1");
            SaveData data = _state.ToSaveData();

            var restored = new GameState();
            restored.LoadFromSaveData(data);

            Assert.IsTrue(restored.HasItem("keycard_a"));
            Assert.IsTrue(restored.HasItem("data_chip_1"));
            Assert.AreEqual(2, restored.Inventory.Count);
        }
    }
}
