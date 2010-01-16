using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace IronJS.Tests
{
    [TestClass]
    public class MathObjectTests
    {
        //TODO: Unit tests for all math functions with both integral and decimal return value

        [TestMethod]
        public void TestMathE()
        {
            Assert.AreEqual(
                Math.E.ToString(CultureInfo.InvariantCulture),
                ScriptRunner.Run("emit(Math.E.toString())")
            );
        }

        [TestMethod]
        public void TestMathPI()
        {
            Assert.AreEqual(
                Math.PI.ToString(CultureInfo.InvariantCulture),
                ScriptRunner.Run("emit(Math.PI.toString())")
            );
        }

        [TestMethod]
        public void TestMathMax()
        {
            Assert.AreEqual(
                "20",
                ScriptRunner.Run("emit(Math.max(3,5,20,10))")
            );
        }

        [TestMethod]
        public void TestMathMin()
        {
            Assert.AreEqual(
                "3",
                ScriptRunner.Run("emit(Math.min(3,5,20,10))")
            );
        }

        [TestMethod]
        public void TestMathRound()
        {
            Assert.AreEqual(
                "3",
                ScriptRunner.Run("emit(Math.round(3.14))")
            );
        }

        [TestMethod]
        public void TestMathFloor()
        {
            Assert.AreEqual(
                "3",
                ScriptRunner.Run("emit(Math.floor(3.14))")
            );
        }

        [TestMethod]
        public void TestMathCeil()
        {
            Assert.AreEqual(
                "4",
                ScriptRunner.Run("emit(Math.ceil(3.14))")
            );
        }

        [TestMethod]
        public void TestMathRandom()
        {
            Assert.AreEqual(
                "number",
                ScriptRunner.Run("emit(typeof Math.random())")
            );
        }

        [TestMethod]
        public void TestMathPow()
        {
            Assert.AreEqual(
                "8",
                ScriptRunner.Run("emit(Math.pow(2, 3))")
            );
        }

        [TestMethod]
        public void TestMathSqrt()
        {
            Assert.AreEqual(
                "3",
                ScriptRunner.Run("emit(Math.sqrt(9))")
            );
        }
    }
}
