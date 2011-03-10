using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests {
  [TestClass]
  public class MozillaECMA3 {
    IronJS.Hosting.Context ctx;

    static MozillaECMA3() {
      System.IO.Directory.SetCurrentDirectory(@"..\..\..\Tests");
    }

    [TestInitialize]
    public void Init() {
      ctx = IronJS.Hosting.Context.Create();
      ctx.ExecuteFile("MozillaECMA3-shell.js");
    }

    [TestMethod]
    public void TestMethod1() {

    }

    [TestMethod]
    public void TestMethod2() {

    }
  }
}
