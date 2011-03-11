using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Conformance_10_Execution_Contexts_10_2_Entering_An_Execution_Context_10_2_1_Global_Code : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Conformance\10_Execution_Contexts\10.2_Entering_An_Execution_Context\10.2.1_Global_Code"); }
    [TestMethod] public void S10_2_1_A1_T1_js() { RunFile(@"S10.2.1_A1_T1.js"); }
    [TestMethod] public void S10_2_1_A1_T2_js() { RunFile(@"S10.2.1_A1_T2.js"); }
  }
}