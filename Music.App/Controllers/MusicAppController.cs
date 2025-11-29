using Microsoft.AspNetCore.Mvc;
using Music.BL;
using Music.BL.Interfaces;
using System.ComponentModel;

namespace Music.App.Controllers;

/// <summary>
/// Controller for Apple Music API endpoints.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MusicAppController"/> class.
/// </remarks>
/// <param name="musicServiceFactory">Factory for creating music services.</param>
/// <param name="config"></param>
/// <param name="logger">Logger instance for logging.</param>
[ApiController]
[Route("api/[controller]")]
public class MusicAppController(
    Service service, 
    IConfiguration config, 
    ILogger<MusicAppController> logger) : ControllerBase
{
    /// <summary>
    /// Get Apple Music Library Songs in Excel format.
    /// </summary>
    /// <remarks>
    /// Use the Apple Music User Token obtained from the client-side sign-in process.
    /// </remarks>
    /// <param name="userToken">The token generated from Apple User's sign-in.</param>
    [HttpGet("excel")]
    public async Task<IActionResult> SearchMusicAsync([FromQuery, DefaultValue("example_user_token")] string userToken)
    {
        userToken = config["APPLE_MUSIC_TOKEN"];

        if (string.IsNullOrWhiteSpace(userToken))
        {
            return BadRequest("Query parameter cannot be empty.");
        }

        var result = await service.DoStuff(userToken);
        return Ok(result);
    }

    /// <summary>
    /// Gets the Apple Music developer token.
    /// </summary>
    [HttpGet("developer-token")]
    public IActionResult GetDeveloperToken()
    {
        var developerToken = service.GetDeveloperToken();
        return Ok(developerToken);
    }
}