﻿using Moq;
using Qml.Net.Internal.Qml;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class DoubleTests : BaseQmlTests<DoubleTests.DoubleTestsQml>
    {
        public class DoubleTestsQml
        {
            public virtual double Property { get; set; }

            public virtual void MethodParameter(double value)
            {
            }

            public virtual double MethodReturn()
            {
                return 0;
            }
        }

        [Fact]
        public void Can_read_write_int_min_value()
        {
            Mock.Setup(x => x.Property).Returns(double.MinValue);

            RunQmlTest(
                "test",
                @"
                    test.property = test.property
                ");

            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.VerifySet(x => x.Property = double.MinValue, Times.Once);
        }

        [Fact]
        public void Can_read_write_int_max_value()
        {
            Mock.Setup(x => x.Property).Returns(double.MaxValue);

            RunQmlTest(
                "test",
                @"
                    test.property = test.property
                ");

            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.VerifySet(x => x.Property = double.MaxValue, Times.Once);
        }

        [Fact]
        public void Can_call_method_with_parameter()
        {
            RunQmlTest(
                "test",
                @"
                    test.methodParameter(3)
                ");

            Mock.Verify(x => x.MethodParameter(It.Is<double>(y => y == 3D)), Times.Once);
        }

        [Fact]
        public void Can_call_method_with_return()
        {
            Mock.Setup(x => x.MethodReturn()).Returns(double.MaxValue);

            RunQmlTest(
                "test",
                @"
                    test.methodParameter(test.methodReturn())
                ");

            Mock.Verify(x => x.MethodParameter(It.Is<double>(y => y == double.MaxValue)), Times.Once);
        }
    }
}