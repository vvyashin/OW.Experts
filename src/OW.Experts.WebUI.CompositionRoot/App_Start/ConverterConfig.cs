using System;
using System.Linq;
using System.Reflection;
using OW.Experts.WebUI.Infrastructure.AutoConverter.Rules;

namespace OW.Experts.WebUI.CompositionRoot
{
    public static class ConverterConfig
    {
        public static void RegisterAll()
        {
            var registerInterface = typeof(IConvertRegister);
            var convertRegisters = Assembly.GetAssembly(typeof(SessionConvertRegister))
                .GetTypes()
                .Where(p => !p.IsInterface && !p.IsAbstract && registerInterface.IsAssignableFrom(p));

            foreach (var convertRegister in convertRegisters)
                ((IConvertRegister)Activator.CreateInstance(convertRegister)).Register();
        }
    }
}