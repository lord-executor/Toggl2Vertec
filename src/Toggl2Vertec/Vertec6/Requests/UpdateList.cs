using System;
using System.Collections.Generic;
using Toggl2Vertec.Vertec6.Api;

namespace Toggl2Vertec.Vertec6.Requests;

public class UpdateList<T>
    where T : Entity
{
    private readonly Create _creates = new Create();
    private readonly Update _updates = new Update();

    public void Register(T entity)
    {
            if (entity.ObjIdSpecified)
            {
                entity.IdToRef();
            }

            if (entity.ObjRefSpecified)
            {
                _updates.Entities.Add(entity);
            }
            else
            {
                _creates.Entities.Add(entity);
            }
        }

    public void Apply(XmlApiClient client)
    {
            if (_updates.Entities.Count > 0)
            {
                var updateResponse = client.Request(_updates).Result.GetDocument().SelectSingleNode("//UpdateResponse/text").InnerText;
                if (!updateResponse.Contains($" {_updates.Entities.Count} "))
                {
                    throw new Exception($"Expected update to return {_updates.Entities.Count} objects, but response is: {updateResponse}");
                }
            }
            if (_creates.Entities.Count > 0)
            {
                var createCount = client.Request(_creates).Result.GetDocument().SelectNodes("//objid").Count;
                if (createCount != _creates.Entities.Count)
                {
                    throw new Exception($"Expected {_creates.Entities.Count} to be created, but response only contains {createCount} object IDs");
                }
            }
        }
}