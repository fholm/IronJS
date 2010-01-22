using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronJS.Tests
{
    [TestClass]
    public class SwitchTests
    {
        [TestMethod]
        public void TestSwitchWillFallThroughAndStopOnBreak()
        {
            Assert.AreEqual(
                "345",
                ScriptRunner.Run(
                    @"
                        switch (3) {
                            case 1:
                                emit(1);

                            case 2:
                                emit(2);
                                break;
                                
                            case 3:
                                emit(3);
                                
                            case 4:
                                emit(4);

                            case 5:
                                emit(5);
                                break;
                                
                            case 6:
                                emit(6);

                            default:
                                emit('default');
                        }
                    "
                )
            );
        }

        [TestMethod]
        public void TestSwitchWillFallThroughAndContinueToDefault()
        {
            Assert.AreEqual(
                "6default",
                ScriptRunner.Run(
                    @"
                        switch (6) {
                            case 1:
                                emit(1);

                            case 2:
                                emit(2);
                                break;
                                
                            case 3:
                                emit(3);
                                
                            case 4:
                                emit(4);

                            case 5:
                                emit(5);
                                break;
                                
                            case 6:
                                emit(6);

                            default:
                                emit('default');
                        }
                    "
                )
            );
        }

        [TestMethod]
        public void TestSwitchDefaultWillAlwaysRunIfNoMatch()
        {
            Assert.AreEqual(
                "default",
                ScriptRunner.Run(
                    @"
                        switch (8) {
                            case 1:
                                emit(1);

                            case 2:
                                emit(2);
                                break;
                                
                            case 3:
                                emit(3);
                                
                            case 4:
                                emit(4);

                            case 5:
                                emit(5);
                                break;
                                
                            case 6:
                                emit(6);

                            default:
                                emit('default');
                        }
                    "
                )
            );
        }
    }
}
