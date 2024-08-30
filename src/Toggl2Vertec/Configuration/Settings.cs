using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Toggl2Vertec.Configuration;

public class Settings
{
    private readonly IConfiguration _config;
    public TogglSettings Toggl { get; }
    public VertecSettings Vertec { get; }
    public CompanySettings Company { get; }

    public Settings(IConfiguration config)
    {
        _config = config;

        Toggl = new TogglSettings();
        _config.GetSection(nameof(Toggl)).Bind(Toggl);

        Vertec = new VertecSettings();
        _config.GetSection(nameof(Vertec)).Bind(Vertec);

        Company = new CompanySettings();
        _config.GetSection(nameof(Company)).Bind(Company);
    }

    public IEnumerable<ProcessorDefinition> GetProcessors()
    {
        foreach (var section in _config.GetSection("Processors").GetChildren())
        {
            yield return new ProcessorDefinition(section);
        }
    }
}