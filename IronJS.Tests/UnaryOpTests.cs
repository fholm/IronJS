using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronJS.Tests
{
    [TestClass]
    public class UnaryOpTests
    {
        [TestMethod]
        public void TestOperatorNot()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(!false)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorOnesComplement()
        {
            Assert.AreEqual(
                "-2",
                ScriptRunner.Run(
                    "emit(~1)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorOnesComplementFractions()
        {
            Assert.AreEqual(
                "-2",
                ScriptRunner.Run(
                    "emit(~1.25)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorNegate()
        {
            Assert.AreEqual(
                "-3",
                ScriptRunner.Run(
                    "emit(-+3)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorNegateFractions()
        {
            Assert.AreEqual(
                "-3.25",
                ScriptRunner.Run(
                    "emit(-+3.25)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorUnaryPlus()
        {
            Assert.AreEqual(
                "3",
                ScriptRunner.Run(
                    "emit(+'3')"
                )
            );
        }

        [TestMethod]
        public void TestOperatorUnaryPlusFractions()
        {
            Assert.AreEqual(
                "3.25",
                ScriptRunner.Run(
                    "emit(+'3.25')"
                )
            );
        }

        [TestMethod]
        public void TestOperatorPreIncrement()
        {
            Assert.AreEqual(
                "2",
                ScriptRunner.Run(
                    "foo = 1; emit(++foo)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorPreIncrementFractions()
        {
            Assert.AreEqual(
                "2.25",
                ScriptRunner.Run(
                    "foo = 1.25; emit(++foo)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorPreDecrement()
        {
            Assert.AreEqual(
                "1",
                ScriptRunner.Run(
                    "foo = 2; emit(--foo)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorPreDecrementFractions()
        {
            Assert.AreEqual(
                "1.25",
                ScriptRunner.Run(
                    "foo = 2.25; emit(--foo)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorPostIncrement()
        {
            Assert.AreEqual(
                "12",
                ScriptRunner.Run(
                    "foo = 1; emit(foo++); emit(foo)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorPostIncrementFractions()
        {
            Assert.AreEqual(
                "1.252.25",
                ScriptRunner.Run(
                    "foo = 1.25; emit(foo++); emit(foo)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorPostDecrement()
        {
            Assert.AreEqual(
                "10",
                ScriptRunner.Run(
                    "foo = 1; emit(foo--); emit(foo)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorPostDecrementFractions()
        {
            Assert.AreEqual(
                "1.250.25",
                ScriptRunner.Run(
                    "foo = 1.25; emit(foo--); emit(foo)"
                )
            );
        }

        [TestMethod]
        public void TestOperatorVoid()
        {
            Assert.AreEqual(
                "undefined",
                ScriptRunner.Run(
                    "emit(void '3.25')"
                )
            );
        }
    }
}
