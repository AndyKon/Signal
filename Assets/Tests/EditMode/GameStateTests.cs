using NUnit.Framework;
using Signal.Core;

namespace Signal.Tests.EditMode
{
    public class GameStateTests
    {
        private GameState _state;

        [SetUp]
        public void SetUp()
        {
            _state = new GameState();
        }

        [Test]
        public void NewState_HasNoFlags()
        {
            Assert.AreEqual(0, _state.FlagCount);
        }

        [Test]
        public void SetFlag_FlagIsPresent()
        {
            _state.SetFlag("found_crew_log_01");
            Assert.IsTrue(_state.HasFlag("found_crew_log_01"));
        }

        [Test]
        public void HasFlag_ReturnsFalseForUnsetFlag()
        {
            Assert.IsFalse(_state.HasFlag("nonexistent"));
        }

        [Test]
        public void FlagCount_ReturnsCorrectCount()
        {
            _state.SetFlag("flag_a");
            _state.SetFlag("flag_b");
            _state.SetFlag("flag_c");
            Assert.AreEqual(3, _state.FlagCount);
        }

        [Test]
        public void SetFlag_Duplicate_DoesNotIncrementCount()
        {
            _state.SetFlag("flag_a");
            _state.SetFlag("flag_a");
            Assert.AreEqual(1, _state.FlagCount);
        }

        [Test]
        public void FlagRatio_ReturnsCorrectPercentage()
        {
            _state.RegisterTotalOptionalFlags(10);
            _state.SetFlag("a");
            _state.SetFlag("b");
            _state.SetFlag("c");
            Assert.AreEqual(0.3f, _state.FlagRatio, 0.001f);
        }

        [Test]
        public void SetSectionPowered_SectionIsPowered()
        {
            _state.SetSectionPowered(1);
            Assert.IsTrue(_state.IsSectionPowered(1));
            Assert.IsFalse(_state.IsSectionPowered(2));
        }

        [Test]
        public void CurrentScene_DefaultsToEmpty()
        {
            Assert.AreEqual("", _state.CurrentScene);
        }

        [Test]
        public void SetCurrentScene_UpdatesValue()
        {
            _state.CurrentScene = "Section1_Hub_Room1";
            Assert.AreEqual("Section1_Hub_Room1", _state.CurrentScene);
        }

        [Test]
        public void ToSaveData_RoundTrips()
        {
            _state.SetFlag("flag_a");
            _state.SetFlag("flag_b");
            _state.SetSectionPowered(1);
            _state.CurrentScene = "Section1_Hub_Room2";
            _state.RegisterTotalOptionalFlags(20);

            SaveData data = _state.ToSaveData();
            var restored = new GameState();
            restored.LoadFromSaveData(data);

            Assert.IsTrue(restored.HasFlag("flag_a"));
            Assert.IsTrue(restored.HasFlag("flag_b"));
            Assert.IsFalse(restored.HasFlag("flag_c"));
            Assert.IsTrue(restored.IsSectionPowered(1));
            Assert.IsFalse(restored.IsSectionPowered(2));
            Assert.AreEqual("Section1_Hub_Room2", restored.CurrentScene);
            Assert.AreEqual(2, restored.FlagCount);
        }

        [Test]
        public void Reset_ClearsAllState()
        {
            _state.SetFlag("flag_a");
            _state.SetSectionPowered(1);
            _state.CurrentScene = "somewhere";
            _state.Reset();

            Assert.AreEqual(0, _state.FlagCount);
            Assert.IsFalse(_state.IsSectionPowered(1));
            Assert.AreEqual("", _state.CurrentScene);
        }
    }
}
