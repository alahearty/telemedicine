using Microsoft.AspNetCore.Mvc;

namespace telemedicine.api.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("_configuration")]
public class OidcConfigurationController : Controller
{
    private readonly ILogger<OidcConfigurationController> _logger;

    public OidcConfigurationController(ILogger<OidcConfigurationController> logger)
    {
        _logger = logger;
    }

    [HttpGet("{clientId}")]
    public IActionResult GetClientRequestParameters([FromRoute] string clientId)
    {
        // Stub for OIDC configuration - app uses JWT auth via /api/auth/login
        var parameters = new Dictionary<string, string>
        {
            ["authority"] = $"{Request.Scheme}://{Request.Host}",
            ["client_id"] = clientId,
            ["response_type"] = "code",
            ["scope"] = "openid profile"
        };
        return Ok(parameters);
    }
}
