using Ninject;
using Ninject.Syntax;
using System;

namespace Toggl2Vertec.Configuration
{
    public static class NinjectSettingsExtensions
    {
        public static IBindingInNamedWithOrOnSyntax<T> WhenSettings<T>(this IBindingWhenSyntax<T> binding, Func<Settings, bool> condition)
        {
            return binding.When(req => condition(req.ParentContext.Kernel.Get<Settings>()));
        }
    }
}
