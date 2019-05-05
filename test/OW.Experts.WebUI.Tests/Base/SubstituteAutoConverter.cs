using System;
using ExpressMapper;
using NSubstitute;
using OW.Experts.WebUI.Infrastructure.AutoConverter;

namespace OW.Experts.WebUI.UnitTests.Base
{
    public static class SubstituteAutoConverter
    {
        public static void Substitute()
        {
            var autoConverter = NSubstitute.Substitute.For<IMappingServiceProvider>();
            autoConverter.Map(Arg.Any<Type>(), Arg.Any<Type>(), Arg.Any<object>())
                .Returns(x => Activator.CreateInstance((Type)x.Args()[1]));

            AutoConverter.CurrentConverter = autoConverter;
        }

        public static void Reset()
        {
            AutoConverter.Reset();
        }
    }
}