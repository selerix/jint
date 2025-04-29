using Jint.Runtime.Interop;

namespace Jint.Tests.Runtime
{
    public class JScriptNetTests
    {
        [Fact]
        public void DateTime_Now()
        {
            var engine = new Engine(
                cfg =>
                {
                    cfg.AllowClr();
                }
            );

            var now = engine.Evaluate("return System.DateTime.Now").AsDateTime();
            Assert.True(now >= DateTime.Now);
        }

        [Fact]
        public void DateTime_UtcNow()
        {
            var engine = new Engine(
                cfg =>
                {
                    cfg.AllowClr();
                }
            );

            var utcNow = engine.Evaluate("System.DateTime.Now.UtcNow").AsDateTime();
            Assert.True(utcNow >= DateTime.UtcNow);
        }

        [Fact]
        public void DateTime_ToString()
        {
            var engine = new Engine(
                cfg =>
                {
                    cfg.AllowClr();
                }
            );

            var nowString = engine.Evaluate("System.DateTime.Now.ToString()").AsString();
            Assert.NotNull(nowString);
        }

        [Fact]
        public void DateTime_ToShortDateString()
        {
            var engine = new Engine(
                cfg =>
                {
                    cfg.AllowClr();
                }
            );

            var shortDateString = engine.Evaluate("System.DateTime.Now.ToShortDateString()").AsString();
            Assert.NotNull(shortDateString);
        }

        [Fact]
        public void DateTime_TypeConversion()
        {
            var engine = new Engine(
                cfg =>
                {
                    cfg.AllowClr();
                }
            );

            var date = engine.Evaluate(
                @"
                    var date : DateTime = '4/1/2025';
                    return date;
                "
            ).AsDateTime();

            Assert.Equal(new DateTime(year: 2025, month: 4, day: 1), date);
        }

        [Fact]
        public void TypedVariables()
        {
            var engine = new Engine(
                cfg =>
                {
                    cfg.AllowClr();
                }
            );

            var isoString = engine.Evaluate(
                @"
                    var date : DateTime = DateTime.Now;
                    return date;
                "
            ).AsString();

            Assert.NotNull(isoString);
        }

        [Fact]
        public void Function_TypedParameters()
        {
            var engine = new Engine(
                cfg =>
                {
                    cfg.AllowClr();
                }
            );

            var now = engine.Evaluate(
                @"
                    function DoNothing(date: DateTime) : DateTime {
                        return date;
                    }

                    var d = DoNothing(DateTime.Now);

                    return date;
                "
            ).AsDateTime();

            Assert.True(now >= DateTime.Now);
        }

        [Fact]
        public void Function_TypeParameters_Conversion()
        {
            var engine = new Engine(
                cfg =>
                {
                    cfg.AllowClr();
                }
            );

            var date = engine.Evaluate(
                @"
                    function DoNothing(date: DateTime) : DateTime {
                        return date;
                    }

                    var d = DoNothing('4/1/2025');

                    return date;
                "
            ).AsDateTime();

            Assert.Equal(new DateTime(year: 2025, month: 4, day: 1), date);
        }

        #region Helper Classes

        public class Foo
        {
#pragma warning disable CA1822 // Mark members as static
            public void BarA(out int value)
#pragma warning restore CA1822 // Mark members as static
            {
                value = 42;
            }

#pragma warning disable CA1822 // Mark members as static
            public void BarB(ref int value)
#pragma warning restore CA1822 // Mark members as static
            {
                value += 1;
            }
        }

        #endregion

        [Fact]
        public void Methods_Out_Parameters()
        {
            var engine = new Engine(
                cfg =>
                {
                    cfg.AllowClr();
                }
            );

            engine.SetValue("Foo", TypeReference.CreateTypeReference<Foo>(engine));

            var number = engine.Evaluate(
                @"
                    var foo = new Foo();
    
                    var number;

                    foo.BarA(&number);

                    return number;
                "
            ).AsInteger();

            Assert.Equal(42, number);
        }

        [Fact]
        public void Methods_Ref_Parameters()
        {
            var engine = new Engine(
                cfg =>
                {
                    cfg.AllowClr();
                }
            );

            engine.SetValue("Foo", TypeReference.CreateTypeReference<Foo>(engine));

            var number = engine.Evaluate(
                @"
                    var foo = new Foo();
    
                    var number = 41;

                    foo.BarB(&number);

                    return number;
                "
            ).AsInteger();

            Assert.Equal(42, number);
        }
    }
}
