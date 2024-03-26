﻿using AutoMapper;
using HelloWorld.Entities;
using HelloWorld.Models;
using HelloWorld.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

        var key = new SymmetricSecurityKey(Convert.FromBase64String(_config["Authentication:SecretKey"]));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //build the token:
        var token = new JwtSecurityToken(
            _config["Authentication:Issuer"],
            _config["Authentication:Audience"],
            new List<Claim>(),
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(1),
            creds
            );
        
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        
        return Ok(tokenString);
    }
}