using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProtonPlanCheck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.Types;

namespace PlanCheck.Tests
{
    [TestClass()]
    public class FieldNameCheckerTests
    {
        [TestMethod()]
        public void FieldNameDecoderTest()
        {
            FieldNameChecker fieldNameChecker = new FieldNameChecker("01ANT", 0, 0);
            Assert.IsTrue(fieldNameChecker.IsLegitName);
            Assert.IsTrue(fieldNameChecker.IsAngleMatch);
            fieldNameChecker = new FieldNameChecker("10 RPO", 200, 0);
            Assert.IsTrue(fieldNameChecker.IsLegitName);
            Assert.IsTrue(fieldNameChecker.IsAngleMatch);
            fieldNameChecker = new FieldNameChecker("4 LASO", 30, 300);
            Assert.IsTrue(fieldNameChecker.IsLegitName);
            Assert.IsTrue(fieldNameChecker.IsAngleMatch);
            fieldNameChecker = new FieldNameChecker("11 POST", 180, 0);
            Assert.IsTrue(fieldNameChecker.IsLegitName);
            Assert.IsTrue(fieldNameChecker.IsAngleMatch);
            fieldNameChecker = new FieldNameChecker("11 RSO", 270, 30);
            Assert.IsTrue(fieldNameChecker.IsLegitName);
            Assert.IsTrue(fieldNameChecker.IsAngleMatch);
            fieldNameChecker = new FieldNameChecker("07 VERTEX ", 90, 270);
            Assert.IsTrue(fieldNameChecker.IsLegitName);
            Assert.IsTrue(fieldNameChecker.IsAngleMatch);
            fieldNameChecker = new FieldNameChecker("02 POST SP_MID", 180, 0);
            Assert.IsTrue(fieldNameChecker.IsLegitName);
            Assert.IsTrue(fieldNameChecker.IsAngleMatch);
            fieldNameChecker = new FieldNameChecker("01 SP_MID", 0, 0);
            Assert.IsFalse(fieldNameChecker.IsLegitName);
            fieldNameChecker = new FieldNameChecker("5 ABC", 0, 0);
            Assert.IsFalse(fieldNameChecker.IsLegitName);
            fieldNameChecker = new FieldNameChecker("02  POST", 0, 0);
            Assert.IsFalse(fieldNameChecker.IsLegitName);

        }
    }
}