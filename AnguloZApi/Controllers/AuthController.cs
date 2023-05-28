using AnguloZApi.APIModels.AuthModels;
using AnguloZApi.Domain;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace AnguloZApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public readonly IMongoCollection<User> _db;
        public AuthController(IMongoDatabase db)
        {
            this._db = db.GetCollection<User>("users");

        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> UserLogin(UserLoginRequest request)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Username, request.Username);
            var result = await _db.FindAsync(filter);
            var user = await result.FirstOrDefaultAsync();
            if(user.TryLogin(request.Password))
            {
                var update = Builders<User>.Update.Set(x => x.UserSecret, user.UserSecret);
                await _db.FindOneAndUpdateAsync(filter,update);
                return Ok(user.UserSecret);
            }
            else
                return Unauthorized();
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> UserRegister(UserRegisterRequest request)
        {
            bool existingUser = await _db.EstimatedDocumentCountAsync() > 0;
            if (existingUser)
                return Conflict();

            var user = new User
            {
                Username = request.Username
            };
            if (user.TryRegister(request.Password))
            {
                await _db.InsertOneAsync(user);
                return Ok(user.UserSecret);
            }
            else
                return BadRequest();
        }

        [HttpPut]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(UserRegisterRequest request)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Username, request.Username);
            var result = await _db.FindAsync(filter);
            var user = await result.FirstOrDefaultAsync();
            if (user.TryRegister(request.Password))
            {
                await _db.FindOneAndReplaceAsync(filter, user);
                return Ok(user.UserSecret);
            }
            else
                return BadRequest();
        }
    }
}
