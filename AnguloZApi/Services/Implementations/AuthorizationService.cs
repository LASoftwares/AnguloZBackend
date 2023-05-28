using AnguloZApi.Domain;
using AnguloZApi.Services.Abstractions;
using MongoDB.Driver;

namespace AnguloZApi.Services.Implementations
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IMongoCollection<User> _db;
        public AuthorizationService(IMongoDatabase database)
        {
            this._db = database.GetCollection<User>("users");
        }
        public async Task<bool> ValidateUserSecretAsync(Guid secret)
        {
            var filter = Builders<User>.Filter.Eq(x=>x.UserSecret, secret);
            var result = await _db.FindAsync(filter);
            return result.Any();
        }
    }
}
