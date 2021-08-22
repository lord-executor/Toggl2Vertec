﻿using System;
using System.Threading;
using Toggl2Vertec.Logging;
using Toggl2Vertec.Vertec;

namespace Toggl2Vertec.Commands.Check
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
                    _client.Login(false);
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