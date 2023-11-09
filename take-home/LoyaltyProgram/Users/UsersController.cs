using System;
using Microsoft.AspNetCore.Mvc;

namespace LoyaltyProgram.Users;

[Route("/users")]
public class UsersController : Controller
{
    private readonly UserDb _db;

    public UsersController(UserDb db)
    {
        _db = db;
    }

    [HttpGet("{userid:int}")]
    [ResponseCache(Duration = 300)]
    public ActionResult<LoyaltyProgramUser> Get(int userId) =>
        (ActionResult<LoyaltyProgramUser>) _db.GetUserById(userId) ?? NotFound();

    [HttpPost("")]
    public ActionResult<LoyaltyProgramUser> Create([FromBody] LoyaltyProgramUser user)
    {
        var registeredUser = _db.RegisterUser(user);
        return Created(new Uri($"/users/{registeredUser.Id}", UriKind.Relative), registeredUser);
    }

    [HttpPut("{userid:int}")]
    public LoyaltyProgramUser Update(int userId, [FromBody] LoyaltyProgramUser user) =>
        _db.UpdateUser(userId, user);
}