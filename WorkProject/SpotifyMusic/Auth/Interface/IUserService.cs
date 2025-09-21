using WorkProject.Models.Entities;
using WorkProject.Models.External;

namespace WorkProject.Auth.Interface;

public interface IUserService
{
    Task<UserEnity> UpsertFromSpotifyAsync(
        SpotifyUserResponse me, string? refreshToken, int expiresIn,
        CancellationToken ct = default);
}