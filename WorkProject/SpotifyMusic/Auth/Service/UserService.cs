using Microsoft.EntityFrameworkCore;
using SpotifyWebApi.Database;
using WorkProject.Auth.Interface; // твій DbContext = DbConnection
using WorkProject.Mappers;
using WorkProject.Models.Entities;
using WorkProject.Models.External;

namespace WorkProject.Auth.Service;



public class UserService : IUserService
{
    private readonly DbConnection _db;
    private readonly ILogger<UserService> _logger;

    public UserService(DbConnection db, ILogger<UserService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<UserEnity> UpsertFromSpotifyAsync(SpotifyUserResponse userResponse, string? refreshToken,
        int expiresIn,
        CancellationToken cancellationToken = default)
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.SpotifyUserId == userResponse.Id, cancellationToken);

        var expiresAt = DateTime.UtcNow.AddSeconds(Math.Max(0, expiresIn - 30));

        if (user == null)
        {
            user = new UserEnity
            {
                SpotifyUserId = userResponse.Id,
                RefreshToken = refreshToken, // може бути null, якщо Spotify не віддав
                TokenExpiresAt = expiresAt
            };
            user.ApplySpotifyProfile(userResponse);
            _db.Users.Add(user);
            _logger.LogInformation("Create new user SpotifyId={Id}", userResponse.Id);
        }

        else
        {
            user.ApplySpotifyProfile(userResponse);

            // Spotify може не повертати refresh_token при повторному логіні — не затираємо існуючий
            if (!string.IsNullOrEmpty(refreshToken))
                user.RefreshToken = refreshToken;

            user.TokenExpiresAt = expiresAt;
            _db.Users.Update(user);
            _logger.LogInformation("Update user SpotifyId={Id}", userResponse.Id);
        }

        await _db.SaveChangesAsync(cancellationToken);
        return user;
    }
}