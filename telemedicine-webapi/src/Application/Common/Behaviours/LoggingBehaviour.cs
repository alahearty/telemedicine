﻿using telemedicine_webapi.Application.Common.Interfaces;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace telemedicine_webapi.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ILogger _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public LoggingBehaviour(ILogger<TRequest> logger, ICurrentUserService currentUserService, IIdentityService identityService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
        _identityService = identityService;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUserService.UserId;
        string userName = string.Empty;

        if (userId != Guid.Empty)
        {
            userName = await _identityService.GetUserNameAsync(userId);
        }

        _logger.LogInformation("telemedicine_webapi Request: {Name} {@UserId} {@UserName} {@Request}",
            requestName, userId, userName, request);
    }
}
