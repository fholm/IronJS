using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Regression : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Regression"); }
    [TestMethod] public void S11_13_2_R2_3_T1_js() { RunFile(@"S11.13.2_R2.3_T1.js"); }
    [TestMethod] public void S11_13_2_R2_3_T10_js() { RunFile(@"S11.13.2_R2.3_T10.js"); }
    [TestMethod] public void S11_13_2_R2_3_T11_js() { RunFile(@"S11.13.2_R2.3_T11.js"); }
    [TestMethod] public void S11_13_2_R2_3_T2_js() { RunFile(@"S11.13.2_R2.3_T2.js"); }
    [TestMethod] public void S11_13_2_R2_3_T3_js() { RunFile(@"S11.13.2_R2.3_T3.js"); }
    [TestMethod] public void S11_13_2_R2_3_T4_js() { RunFile(@"S11.13.2_R2.3_T4.js"); }
    [TestMethod] public void S11_13_2_R2_3_T5_js() { RunFile(@"S11.13.2_R2.3_T5.js"); }
    [TestMethod] public void S11_13_2_R2_3_T6_js() { RunFile(@"S11.13.2_R2.3_T6.js"); }
    [TestMethod] public void S11_13_2_R2_3_T7_js() { RunFile(@"S11.13.2_R2.3_T7.js"); }
    [TestMethod] public void S11_13_2_R2_3_T8_js() { RunFile(@"S11.13.2_R2.3_T8.js"); }
    [TestMethod] public void S11_13_2_R2_3_T9_js() { RunFile(@"S11.13.2_R2.3_T9.js"); }
    [TestMethod] public void S12_6_4_R1_js() { RunFile(@"S12.6.4_R1.js"); }
    [TestMethod] public void S12_6_4_R2_js() { RunFile(@"S12.6.4_R2.js"); }
    [TestMethod] public void S15_1_1_1_R1_js() { RunFile(@"S15.1.1.1_R1.js"); }
    [TestMethod] public void S15_1_1_1_R2_1_js() { RunFile(@"S15.1.1.1_R2.1.js"); }
    [TestMethod] public void S15_1_1_1_R2_2_js() { RunFile(@"S15.1.1.1_R2.2.js"); }
    [TestMethod] public void S15_1_1_1_R3_1_js() { RunFile(@"S15.1.1.1_R3.1.js"); }
    [TestMethod] public void S15_1_1_1_R3_2_js() { RunFile(@"S15.1.1.1_R3.2.js"); }
    [TestMethod] public void S15_1_1_1_R4_js() { RunFile(@"S15.1.1.1_R4.js"); }
    [TestMethod] public void S15_1_1_2_R1_js() { RunFile(@"S15.1.1.2_R1.js"); }
    [TestMethod] public void S15_1_1_2_R2_1_js() { RunFile(@"S15.1.1.2_R2.1.js"); }
    [TestMethod] public void S15_1_1_2_R2_2_js() { RunFile(@"S15.1.1.2_R2.2.js"); }
    [TestMethod] public void S15_1_1_2_R3_1_js() { RunFile(@"S15.1.1.2_R3.1.js"); }
    [TestMethod] public void S15_1_1_2_R3_2_js() { RunFile(@"S15.1.1.2_R3.2.js"); }
    [TestMethod] public void S15_1_1_2_R4_js() { RunFile(@"S15.1.1.2_R4.js"); }
  }
}