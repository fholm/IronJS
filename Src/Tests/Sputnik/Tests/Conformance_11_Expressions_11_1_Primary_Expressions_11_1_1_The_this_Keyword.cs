using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_11_Expressions_11_1_Primary_Expressions_11_1_1_The_this_Keyword : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\11_Expressions\11.1_Primary_Expressions\11.1.1_The_this_Keyword"); }
    [TestMethod] public void S11_1_1_A1_js() { RunFile(@"S11.1.1_A1.js"); }
    [TestMethod] public void S11_1_1_A2_js() { RunFile(@"S11.1.1_A2.js"); }
    [TestMethod] public void S11_1_1_A3_1_js() { RunFile(@"S11.1.1_A3.1.js"); }
    [TestMethod] public void S11_1_1_A3_2_js() { RunFile(@"S11.1.1_A3.2.js"); }
    [TestMethod] public void S11_1_1_A4_1_js() { RunFile(@"S11.1.1_A4.1.js"); }
    [TestMethod] public void S11_1_1_A4_2_js() { RunFile(@"S11.1.1_A4.2.js"); }
  }
}