namespace Toggl2Vertec.Vertec6.Api;

public class Envelope
{
    public Header Header { get; set; } = new Header();

    public Body Body { get; set; } = new Body();
}