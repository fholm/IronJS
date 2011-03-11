using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_11_Expressions_11_1_Primary_Expressions_11_1_4_Array_Initialiser : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\11_Expressions\11.1_Primary_Expressions\11.1.4_Array_Initialiser"); }
    [TestMethod] public void S11_1_4_A1_1_js() { RunFile(@"S11.1.4_A1.1.js"); }
    [TestMethod] public void S11_1_4_A1_2_js() { RunFile(@"S11.1.4_A1.2.js"); }
    [TestMethod] public void S11_1_4_A1_3_js() { RunFile(@"S11.1.4_A1.3.js"); }
    [TestMethod] public void S11_1_4_A1_4_js() { RunFile(@"S11.1.4_A1.4.js"); }
    [TestMethod] public void S11_1_4_A1_5_js() { RunFile(@"S11.1.4_A1.5.js"); }
    [TestMethod] public void S11_1_4_A1_6_js() { RunFile(@"S11.1.4_A1.6.js"); }
    [TestMethod] public void S11_1_4_A1_7_js() { RunFile(@"S11.1.4_A1.7.js"); }
    [TestMethod] public void S11_1_4_A2_js() { RunFile(@"S11.1.4_A2.js"); }
  }
}