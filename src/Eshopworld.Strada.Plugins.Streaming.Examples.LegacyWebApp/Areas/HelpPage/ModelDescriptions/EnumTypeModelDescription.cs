using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Eshopworld.Strada.Plugins.Streaming.Examples.LegacyWebApp.Areas.HelpPage.ModelDescriptions
{
    public class EnumTypeModelDescription : ModelDescription
    {
        public EnumTypeModelDescription()
        {
            Values = new Collection<EnumValueDescription>();
        }

        public Collection<EnumValueDescription> Values { get; private set; }
    }
}