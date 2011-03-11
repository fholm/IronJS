using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_10_Execution_Contexts_10_2_Entering_An_Execution_Context : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\10_Execution_Contexts\10.2_Entering_An_Execution_Context"); }
    [TestMethod] public void S10_2_A1_1_T1_js() { RunFile(@"S10.2_A1.1_T1.js"); }
    [TestMethod] public void S10_2_A1_1_T2_js() { RunFile(@"S10.2_A1.1_T2.js"); }
  }
}