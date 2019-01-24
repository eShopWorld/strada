using System;
using System.Reflection;

namespace Eshopworld.Strada.Plugins.Streaming.Examples.LegacyWebApp.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}