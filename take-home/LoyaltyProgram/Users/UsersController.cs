using Microsoft.AspNetCore.Mvc;

namespace LoyaltyProgram.Users
{
    [Route("/users")]
    public class UsersController : Controller
    {
        private readonly UserDb _db;

        public UsersController(UserDb db)
        {
            _db = db;
        }
    }
}