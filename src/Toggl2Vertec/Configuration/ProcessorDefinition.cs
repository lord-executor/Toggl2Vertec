using Microsoft.Extensions.Configuration;

namespace Toggl2Vertec.Configuration;

public class ProcessorDefinition
{
    public IConfigurationSection Section { get; }
    public string Name => Section["Name"];

    public ProcessorDefinition(IConfigurationSection section)
    {
        Section = section;
    }
}