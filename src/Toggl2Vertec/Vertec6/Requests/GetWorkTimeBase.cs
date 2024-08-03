using System;

namespace Toggl2Vertec.Vertec6.Requests;

public abstract class GetWorkTimeBase : IRequest<long>
{
    protected DateTime Von { get; set; }
    protected DateTime Bis { get; set; }
    protected string DateStr { get; set; }

    protected GetWorkTimeBase(DateTime von, DateTime bis)
    {
        DateStr = $"encodeDate({von.Year},{von.Month},{von.Day}), " + 
                  $"encodeDate({bis.Year},{bis.Month},{bis.Day})";
    }
    
    public abstract long Execute(XmlApiClient client);
}