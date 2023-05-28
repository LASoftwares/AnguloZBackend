
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnguloZApi.Domain
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        [Column("passwordHash")]
        public string Password { get; set; }
        public Guid UserSecret { get; set; }

        public bool TryRegister(string password)
        {
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            this.Password = hasher.HashPassword(this, password);
            this.UserSecret = Guid.NewGuid();
            return true;
        }

        public bool TryLogin(string password)
        {
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(this, this.Password, password);
            bool success = result == PasswordVerificationResult.Success;
            if(success)
                this.UserSecret = Guid.NewGuid();
            return success;
        }
    }
}
