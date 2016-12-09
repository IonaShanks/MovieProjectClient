// <copyright file="UnitTest1Test.cs">Copyright ©  2016</copyright>
using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelTests;

namespace ModelTests.Tests
{
    /// <summary>This class contains parameterized unit tests for UnitTest1</summary>
    [PexClass(typeof(UnitTest1))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class UnitTest1Test
    {
        /// <summary>Test stub for TestMethod1()</summary>
        [PexMethod]
        public void TestMethod1Test([PexAssumeUnderTest]UnitTest1 target)
        {
            target.TestMethod1();
            // TODO: add assertions to method UnitTest1Test.TestMethod1Test(UnitTest1)
        }
    }
}
