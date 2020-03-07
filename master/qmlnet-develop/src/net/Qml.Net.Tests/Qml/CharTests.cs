﻿using Moq;
using Qml.Net.Internal.Qml;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class CharTests : BaseQmlTests<CharTests.CharTestsQml>
    {
        public class CharTestsQml
        {
            public virtual char Property { get; set; }

            public virtual char? Nullable { get; set; }

            public virtual void MethodParameter(char value)
            {
            }

            public virtual void MethodParameterNullable(char? value)
            {
            }

            public virtual char MethodReturn()
            {
                return char.MinValue;
            }
        }

        [Fact]
        public void Can_read_write_char_null()
        {
            Mock.Setup(x => x.Property).Returns((char)0);

            RunQmlTest(
                "test",
                @"
                    test.property = test.property
                ");

            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.VerifySet(x => x.Property = (char)0, Times.Once);
        }

        [Fact]
        public void Can_read_write_char_max_value()
        {
            Mock.Setup(x => x.Property).Returns('T');

            RunQmlTest(
                "test",
                @"
                    test.property = test.property
                ");

            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.VerifySet(x => x.Property = 'T', Times.Once);
        }

        [Fact]
        public void Can_read_write_char_unicode()
        {
            Mock.Setup(x => x.Property).Returns('Ώ');

            RunQmlTest(
                "test",
                @"
                    test.property = test.property
                ");

            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.VerifySet(x => x.Property = 'Ώ', Times.Once);
        }

        [Fact]
        public void Can_set_method_parameter()
        {
            RunQmlTest(
                "test",
                @"
                    test.methodParameter(""Ώ"")
                ");

            Mock.Verify(x => x.MethodParameter(It.IsIn('Ώ')), Times.Once);
        }

        [Fact]
        public void Can_use_as_return_type()
        {
            Mock.Setup(x => x.MethodReturn()).Returns('Ώ');

            RunQmlTest(
                "test",
                @"
                    test.methodParameter(test.methodReturn())
                ");

            Mock.Verify(x => x.MethodParameter(It.IsIn('Ώ')), Times.Once);
        }

        [Fact]
        public void Can_read_nullable_char_no_value()
        {
            Mock.Setup(x => x.Nullable).Returns((char?)null);
            Mock.Setup(x => x.MethodParameterNullable(It.Is<char?>(y => y == null)));

            RunQmlTest(
                "test",
                @"
                    test.methodParameterNullable(test.nullable)
                ");

            Mock.VerifyGet(x => x.Nullable, Times.Once);
            Mock.Verify(x => x.MethodParameterNullable(It.Is<char?>(y => y == null)), Times.Once);
        }

        [Fact]
        public void Can_read_nullable_char_with_value()
        {
            Mock.Setup(x => x.Nullable).Returns('t');
            Mock.Setup(x => x.MethodParameterNullable(It.Is<char>(y => y == 't')));

            RunQmlTest(
                "test",
                @"
                    test.methodParameterNullable(test.nullable)
                ");

            Mock.VerifyGet(x => x.Nullable, Times.Once);
            Mock.Verify(x => x.MethodParameterNullable(It.Is<char?>(y => y == 't')), Times.Once);
        }
    }
}