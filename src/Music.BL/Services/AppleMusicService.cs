using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Music.BL.Interfaces;
using Music.DL.Interfaces;
using Music.DL.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace Music.BL.Services;

internal class AppleMusicService(IMusicRepoFactory musicRepoFactory, IConfiguration configuration) : IMusicService
{
    private readonly IMusicRepo appleMusicRepo = musicRepoFactory.GetService(DL.Models.MusicServiceType.Apple);

    private string? _developerToken = null;

    public string DeveloperToken
    {
        get => _developerToken ??= GenerateDeveloperToken();
        set => _developerToken = value;
    }

    public async Task<IEnumerable<LibrarySong>> GetLibraryAsync(string userToken)
    {
        var library = await appleMusicRepo.GetLibraryAsync(userToken, DeveloperToken);
        
        var orderedLibrary = library.OrderBy(s => s.Artist).ThenBy(s => s.Name);

        return orderedLibrary;
    }

    internal string GenerateDeveloperToken()
    {
        var privateKeyPathOrPem = configuration["APPLE_MUSIC_PRIVATE_KEY_PATH"]
            ?? throw new InvalidOperationException("Missing APPLE_MUSIC_PRIVATE_KEY_PATH or APPLE_MUSIC_PRIVATE_KEY in configuration.");
        var keyId = configuration["APPLE_MUSIC_KEY_ID"]
            ?? throw new InvalidOperationException("Missing APPLE_MUSIC_KEY_ID in configuration.");
        var teamId = configuration["APPLE_MUSIC_TEAM_ID"]
            ?? throw new InvalidOperationException("Missing APPLE_MUSIC_TEAM_ID in configuration.");

        string privateKeyPem;
        if (privateKeyPathOrPem.TrimStart().StartsWith("-----BEGIN", StringComparison.Ordinal))
        {
            privateKeyPem = privateKeyPathOrPem;
        }
        else
        {
            if (!File.Exists(privateKeyPathOrPem))
                throw new FileNotFoundException("Apple Music private key file not found.", privateKeyPathOrPem);

            privateKeyPem = File.ReadAllText(privateKeyPathOrPem);
        }

        ECDsa ecdsa = ECDsa.Create();
        try
        {
            ecdsa.ImportFromPem(privateKeyPem.ToCharArray());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to import Apple Music private key. Ensure the .p8/PEM is valid.", ex);
        }

        var securityKey = new ECDsaSecurityKey(ecdsa)
        {
            KeyId = keyId
        };

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.EcdsaSha256);

        // Apple allows up to 6 months for developer tokens; pick a safe duration for dev
        var now = DateTime.UtcNow;
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = teamId,
            NotBefore = now,
            Expires = now.AddMonths(6),
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
