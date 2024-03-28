using AutoMapper;
using HelloWorld.Entities;
using HelloWorld.Models;
using HelloWorld.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HelloWorld.Controllers;

[ApiController]
[Route("api/login")]
public class LoginController : ControllerBase
{
    private ILogger<LoginController> _logger;
    private readonly IUserRepository _repo;
    private readonly IMapper _mapper;
    private IConfiguration _config;
    const int maxPageSize = 10;

    public LoginController(ILogger<LoginController> logger, IUserRepository repo, IMapper mapper, IConfiguration config)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _repo = repo;
        _mapper = mapper;
        _config = config;
    }

    [HttpPost]
    public async Task<ActionResult<string>> Login(LoginRequestDTO login)
    {
        User? user = await _repo.GetUser(login.UserName, login.Password);

        if (user == null)
        {
            return Unauthorized();
        }

        //Generate a key
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Authentication:SecretKey"]));

        //Generate credentials:
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //build the token:
        var token = new JwtSecurityToken(
            _config["Authentication:Issuer"],
            _config["Authentication:Audience"],
            new List<Claim>()
            {
                new Claim ("sub", user.ID.ToString()), //"subject", this is by convention used to identify the claims prinicple (the user)
                new Claim ("auth", user.AutherizationLevel.ToString()),
                new Claim ("user_name", user.Username.ToString()),
                //new Claim ("password", user.Password.ToString()) - Do not put the password in the claims because it will be visible!
            },
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(1),
            creds
            );
        
        //This generates the JWT and signs it by hashing the token with the creds.
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        
        return Ok(tokenString);
    }
}
