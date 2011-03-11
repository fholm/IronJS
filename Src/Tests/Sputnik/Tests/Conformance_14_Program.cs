using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_14_Program : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\14_Program"); }
    [TestMethod] public void S14_A1_js() { RunFile(@"S14_A1.js"); }
    [TestMethod] public void S14_A2_js() { RunFile(@"S14_A2.js"); }
    [TestMethod] public void S14_A3_js() { RunFile(@"S14_A3.js"); }
    [TestMethod] public void S14_A5_T1_js() { RunFile(@"S14_A5_T1.js"); }
    [TestMethod] public void S14_A5_T2_js() { RunFile(@"S14_A5_T2.js"); }
  }
}