using System;
using System.Threading;
using Toggl2Vertec.Commands.Check;
using Toggl2Vertec.Logging;

namespace Toggl2Vertec.Vertec
{
    public class VertecAccessCheck : BaseCheckStep
    {
        private readonly VertecClient _client;

        public VertecAccessCheck(VertecClient client)
        {
            _client = client;
        }

        public override bool Check(ICliLogger logger)
        {
            var attempt = 0;
            do
            {
                logger.LogPartial(logger.CreateText($"Checking Vertec Login (attempt {++attempt}): "));

                try
                {
                    _client.Login();
                    return Ok(logger);
                }
                catch (Exception e)
                {
                    Fail(logger, e.Message);
                }

                Thread.Sleep(2000);
            } while (attempt < 6);

            return attempt != 6;
        }
    }
}
