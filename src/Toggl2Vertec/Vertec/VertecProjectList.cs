using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace Toggl2Vertec.Vertec
{
    public class VertecProjectList
    {
        public IDictionary<string, VertecProject> _map = new Dictionary<string, VertecProject>();

        public VertecProjectList()
        {
            var vertecProjectJson = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "vertec-projects.json"));
            var projects = (JsonElement)JsonSerializer.Deserialize(vertecProjectJson, typeof(object));

            foreach (var row in projects.GetProperty("rows").EnumerateArray())
            {
                var proj = new VertecProject
                {
                    Id = row.GetProperty("phase").GetString()!,
                    ProjectId = row.GetProperty("projekt_id").GetInt32(),
                    PhaseId = row.GetProperty("phase_id").GetInt32(),
                };

                _map.Add(proj.Id, proj);
            }
        }

        public VertecProject GetProject(string vertecId)
        {
            return _map.ContainsKey(vertecId) ? _map[vertecId] : null;
        }
    }
}
