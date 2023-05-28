namespace AnguloZApi.Services.Abstractions
{
    public interface IAuthorizationService
    {
        Task<bool> ValidateUserSecretAsync(Guid secret);
    }
}
