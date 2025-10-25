using Microsoft.AspNetCore.Mvc;
using Spotify.Newrelease;


namespace WorkProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewReleasesController : ControllerBase
{
    private readonly NewReleasesService.NewReleasesServiceClient _grpcClient;

    public NewReleasesController(NewReleasesService.NewReleasesServiceClient grpcClient)
    {
        _grpcClient = grpcClient;
    }
    
    [HttpGet("{countryCode}/{limit}/{offset}")]
    public async Task<IActionResult> GetNewReleases(string countryCode, int limit, int offset, CancellationToken cancellationToken)
    {
        var request = new GetNewReleasesRequest
        {
            CountryCode = countryCode,
            Limit = limit,
            Offset = offset
        };
        var releases = await _grpcClient.GetNewReleasesAsync(request, cancellationToken: cancellationToken);
        return Ok(releases.Albums);
    }
}