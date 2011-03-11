using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests.Sputnik {
  [TestClass]
  public class Implementation_Diagnostics : BaseTest {
    [TestInitialize]
    public void Init() { SetSputnikDir(@"Implementation_Diagnostics"); }
    [TestMethod] public void S11_4_3_D1_1_js() { RunFile(@"S11.4.3_D1.1.js"); }
    [TestMethod] public void S11_4_3_D1_2_js() { RunFile(@"S11.4.3_D1.2.js"); }
    [TestMethod] public void S12_6_4_D1_js() { RunFile(@"S12.6.4_D1.js"); }
    [TestMethod] public void S13_2_2_D20_T1_js() { RunFile(@"S13.2.2_D20_T1.js"); }
    [TestMethod] public void S13_2_2_D20_T2_js() { RunFile(@"S13.2.2_D20_T2.js"); }
    [TestMethod] public void S13_2_2_D20_T3_js() { RunFile(@"S13.2.2_D20_T3.js"); }
    [TestMethod] public void S13_2_2_D20_T4_js() { RunFile(@"S13.2.2_D20_T4.js"); }
    [TestMethod] public void S13_2_2_D20_T5_js() { RunFile(@"S13.2.2_D20_T5.js"); }
    [TestMethod] public void S13_2_2_D20_T6_js() { RunFile(@"S13.2.2_D20_T6.js"); }
    [TestMethod] public void S13_2_2_D20_T7_js() { RunFile(@"S13.2.2_D20_T7.js"); }
    [TestMethod] public void S13_2_2_D20_T8_js() { RunFile(@"S13.2.2_D20_T8.js"); }
    [TestMethod] public void S13_2_D1_1_js() { RunFile(@"S13.2_D1.1.js"); }
    [TestMethod] public void S13_2_D1_2_js() { RunFile(@"S13.2_D1.2.js"); }
    [TestMethod] public void S13_D1_T1_js() { RunFile(@"S13_D1_T1.js"); }
    [TestMethod] public void S14_D1_T1_js() { RunFile(@"S14_D1_T1.js"); }
    [TestMethod] public void S14_D4_T1_js() { RunFile(@"S14_D4_T1.js"); }
    [TestMethod] public void S14_D4_T2_js() { RunFile(@"S14_D4_T2.js"); }
    [TestMethod] public void S14_D4_T3_js() { RunFile(@"S14_D4_T3.js"); }
    [TestMethod] public void S14_D6_T1_js() { RunFile(@"S14_D6_T1.js"); }
    [TestMethod] public void S14_D6_T2_js() { RunFile(@"S14_D6_T2.js"); }
    [TestMethod] public void S14_D7_js() { RunFile(@"S14_D7.js"); }
    [TestMethod] public void S15_1_2_2_D1_1_js() { RunFile(@"S15.1.2.2_D1.1.js"); }
    [TestMethod] public void S15_1_2_2_D1_2_js() { RunFile(@"S15.1.2.2_D1.2.js"); }
    [TestMethod] public void S15_10_6_3_D1_T1_js() { RunFile(@"S15.10.6.3_D1_T1.js"); }
    [TestMethod] public void S15_4_4_12_D1_5_T1_js() { RunFile(@"S15.4.4.12_D1.5_T1.js"); }
    [TestMethod] public void S15_4_4_12_D1_5_T2_js() { RunFile(@"S15.4.4.12_D1.5_T2.js"); }
    [TestMethod] public void S15_5_2_D1_T1_js() { RunFile(@"S15.5.2_D1_T1.js"); }
    [TestMethod] public void S15_5_2_D1_T2_js() { RunFile(@"S15.5.2_D1_T2.js"); }
    [TestMethod] public void S15_5_2_D2_js() { RunFile(@"S15.5.2_D2.js"); }
    [TestMethod] public void S15_5_4_11_D1_1_T1_js() { RunFile(@"S15.5.4.11_D1.1_T1.js"); }
    [TestMethod] public void S15_5_4_11_D1_1_T2_js() { RunFile(@"S15.5.4.11_D1.1_T2.js"); }
    [TestMethod] public void S15_5_4_11_D1_1_T3_js() { RunFile(@"S15.5.4.11_D1.1_T3.js"); }
    [TestMethod] public void S15_5_4_11_D1_1_T4_js() { RunFile(@"S15.5.4.11_D1.1_T4.js"); }
    [TestMethod] public void S15_7_4_5_A1_2_D02_js() { RunFile(@"S15.7.4.5_A1.2_D02.js"); }
    [TestMethod] public void S15_7_4_5_D1_2_T01_js() { RunFile(@"S15.7.4.5_D1.2_T01.js"); }
    [TestMethod] public void S15_9_1_14_D1_js() { RunFile(@"S15.9.1.14_D1.js"); }
    [TestMethod] public void S8_4_D1_1_js() { RunFile(@"S8.4_D1.1.js"); }
    [TestMethod] public void S8_4_D1_2_js() { RunFile(@"S8.4_D1.2.js"); }
    [TestMethod] public void S8_4_D2_1_js() { RunFile(@"S8.4_D2.1.js"); }
    [TestMethod] public void S8_4_D2_2_js() { RunFile(@"S8.4_D2.2.js"); }
    [TestMethod] public void S8_4_D2_3_js() { RunFile(@"S8.4_D2.3.js"); }
    [TestMethod] public void S8_4_D2_4_js() { RunFile(@"S8.4_D2.4.js"); }
    [TestMethod] public void S8_4_D2_5_js() { RunFile(@"S8.4_D2.5.js"); }
    [TestMethod] public void S8_4_D2_6_js() { RunFile(@"S8.4_D2.6.js"); }
    [TestMethod] public void S8_4_D2_7_js() { RunFile(@"S8.4_D2.7.js"); }
    [TestMethod] public void S8_6_D1_1_js() { RunFile(@"S8.6_D1.1.js"); }
    [TestMethod] public void S8_6_D1_2_js() { RunFile(@"S8.6_D1.2.js"); }
    [TestMethod] public void S8_6_D1_3_js() { RunFile(@"S8.6_D1.3.js"); }
    [TestMethod] public void S8_6_D1_4_js() { RunFile(@"S8.6_D1.4.js"); }
    [TestMethod] public void S8_8_D1_1_js() { RunFile(@"S8.8_D1.1.js"); }
    [TestMethod] public void S8_8_D1_2_js() { RunFile(@"S8.8_D1.2.js"); }
    [TestMethod] public void S8_8_D1_3_js() { RunFile(@"S8.8_D1.3.js"); }
  }
}