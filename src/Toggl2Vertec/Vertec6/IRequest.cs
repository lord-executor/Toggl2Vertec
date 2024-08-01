using System.Collections.Generic;
using Toggl2Vertec.Vertec6.Api;

namespace Toggl2Vertec.Vertec6;

public interface IRequest<T>
{
    T Execute(XmlApiClient client);
}