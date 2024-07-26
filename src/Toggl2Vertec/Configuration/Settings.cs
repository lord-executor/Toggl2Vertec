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

    public Settings(IConfiguration config)
    {
        _config = config;

        Toggl = new TogglSettings();
        _config.GetSection("Toggl").Bind(Toggl);

        Vertec = new VertecSettings();
        _config.GetSection("Vertec").Bind(Vertec);
    }

    public IEnumerable<ProcessorDefinition> GetProcessors()
    {
        foreach (var section in _config.GetSection("Processors").GetChildren())
        {
            yield return new ProcessorDefinition(section);
        }
    }
}