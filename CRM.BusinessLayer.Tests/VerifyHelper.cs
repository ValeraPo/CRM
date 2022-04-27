using System;
using Microsoft.Extensions.Logging;
using Moq;
using static Moq.It;

namespace CRM.BusinessLayer.Tests
{
    public static class VerifyHelper
    {
        internal static void VerifyLogger<T>(Mock<ILogger<T>> logger, LogLevel level, string message)
        {
            logger.Verify(v => v.Log(level,
                    IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => string.Equals(message, o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                    IsAny<Exception>(),
                    IsAny<Func<IsAnyType, Exception, string>>()!), Times.Once);
        }

        internal static void VerifyLogger<T>(Mock<ILogger<T>> logger, LogLevel level, int times)
        {
            logger.Verify(v => v.Log(level,
                    IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => true),
                    IsAny<Exception>(),
                    IsAny<Func<IsAnyType, Exception, string>>()!), Times.Exactly(times));
        }
    }
}
