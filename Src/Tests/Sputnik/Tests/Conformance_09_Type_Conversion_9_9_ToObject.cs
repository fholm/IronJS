using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_09_Type_Conversion_9_9_ToObject : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\09_Type_Conversion\9.9_ToObject"); }
    [TestMethod] public void S9_9_A1_js() { RunFile(@"S9.9_A1.js"); }
    [TestMethod] public void S9_9_A2_js() { RunFile(@"S9.9_A2.js"); }
    [TestMethod] public void S9_9_A3_js() { RunFile(@"S9.9_A3.js"); }
    [TestMethod] public void S9_9_A4_js() { RunFile(@"S9.9_A4.js"); }
    [TestMethod] public void S9_9_A5_js() { RunFile(@"S9.9_A5.js"); }
    [TestMethod] public void S9_9_A6_js() { RunFile(@"S9.9_A6.js"); }
  }
}