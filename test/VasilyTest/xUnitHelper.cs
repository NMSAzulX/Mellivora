using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vasily.Utils;
using VasilyTest.Model;
using Xunit.Abstractions;
using Xunit.Sdk;
using Vasily;

namespace VasilyTest
{
    public class VasilyInitializeAttribute : BeforeAfterTestAttribute {

        public override void Before(MethodInfo methodUnderTest)
        {
            ModelAnalyser.Initialization(typeof(Test));
            ModelAnalyser.Initialization(typeof(AlTest));
        }
    }

}
