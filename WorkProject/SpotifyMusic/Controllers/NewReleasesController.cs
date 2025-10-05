using Microsoft.AspNetCore.Mvc;
using WorkProject.GrpcClient.Albums;

namespace WorkProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewReleasesController : ControllerBase
{
    private readonly NewReleasesGrpcClient _grpcClient;

    public NewReleasesController(NewReleasesGrpcClient grpcClient)
    {
        _grpcClient = grpcClient;
    }
    
    [HttpGet("{countryCode}/{limit}/{offset}")]
    public async Task<IActionResult> GetNewReleases(string countryCode, int limit, int offset, CancellationToken cancellationToken)
    {
        var releases = await _grpcClient.GetNewReleasesAsync(countryCode, limit, offset, cancellationToken);
        return Ok(releases.Albums);
    }
}