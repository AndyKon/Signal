using NUnit.Framework;
using Signal.Core;

namespace Signal.Tests.EditMode
{
    public class EndingEvaluatorTests
    {
        private GameState _state;

        [SetUp]
        public void SetUp()
        {
            _state = new GameState();
            _state.RegisterTotalOptionalFlags(10);
        }

        [Test]
        public void NoFlags_ReturnsEndingA()
        {
            Assert.AreEqual(Ending.Escape, EndingEvaluator.Evaluate(_state));
        }

        [Test]
        public void SixtyPercentFlags_ReturnsEndingB()
        {
            for (int i = 0; i < 6; i++)
                _state.SetFlag($"flag_{i}");
            Assert.AreEqual(Ending.Truth, EndingEvaluator.Evaluate(_state));
        }

        [Test]
        public void NinetyPercentFlags_WithoutMirror_ReturnsEndingB()
        {
            for (int i = 0; i < 9; i++)
                _state.SetFlag($"flag_{i}");
            Assert.AreEqual(Ending.Truth, EndingEvaluator.Evaluate(_state));
        }

        [Test]
        public void NinetyPercentFlags_WithMirror_ReturnsEndingC()
        {
            for (int i = 0; i < 9; i++)
                _state.SetFlag($"flag_{i}");
            _state.SetFlag("saw_reflection");
            Assert.AreEqual(Ending.Confrontation, EndingEvaluator.Evaluate(_state));
        }

        [Test]
        public void FiftyPercentFlags_ReturnsEndingA()
        {
            for (int i = 0; i < 5; i++)
                _state.SetFlag($"flag_{i}");
            Assert.AreEqual(Ending.Escape, EndingEvaluator.Evaluate(_state));
        }
    }
}
