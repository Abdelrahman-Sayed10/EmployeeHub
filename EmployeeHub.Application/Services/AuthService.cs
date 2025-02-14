using EmployeeHub.Application.Contracts.IRepository;
using EmployeeHub.Application.Contracts.IService;
using EmployeeHub.Domain.Entities.Operation;
using EmployeeHub.Dtos.AuthDto;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeHub.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _config;
    private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();


    public AuthService(IUnitOfWork unitOfWork, IConfiguration config)
    {
        _unitOfWork = unitOfWork;
        _config = config;
    }
    public async Task<AuthResponseDto> Register(RegisterDto registerDto)
    {
        var userRepo = _unitOfWork.Repository<User>();

        var existing = (await userRepo.GetAllAsync())
            .FirstOrDefault(x => x.UserName == registerDto.UserName);
        if (existing != null)
            throw new Exception("UserName already taken");

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = registerDto.UserName,
            IsAdmin = registerDto.IsAdmin
        };

        var hashedValue = _passwordHasher.HashPassword(user, registerDto.Password);
        user.PasswordHash = hashedValue;

        await userRepo.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return GenerateJwtToken(user);
    }
    public async Task<AuthResponseDto> Login(LoginDto loginDto)
    {
        var userRepo = _unitOfWork.Repository<User>();

        var user = (await userRepo.GetAllAsync())
            .FirstOrDefault(x => x.UserName == loginDto.UserName);

        if (user == null)
            throw new Exception("User not found.");

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
            throw new Exception("Invalid credentials.");

        return GenerateJwtToken(user);
    }

    public async Task<AuthResponseDto> LoginWithGoogle(string googleToken)
    {
        var payload = await validationGoogleToken(googleToken);
        if (payload == null)
            throw new UnauthorizedAccessException("Invalid Google Token.");

        var userRepo = _unitOfWork.Repository<User>();

        var user = (await userRepo.GetAllAsync())
            .FirstOrDefault(x => x.UserName == payload.Email);

        if (user == null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                UserName = payload.Email,
                IsAdmin = false 
            };
            await userRepo.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        return GenerateJwtToken(user);
    }

    private AuthResponseDto GenerateJwtToken(User user)
    {
        var key = _config["JwtSettings:Key"];
        var issuer = _config["JwtSettings:Issuer"];
        var audience = _config["JwtSettings:Audience"];
        var expires = DateTime.Now.AddHours(Convert.ToDouble(_config["JwtSettings:ExpiresInHours"]));

        var tokenHandler = new JwtSecurityTokenHandler();
        var keyBytes = Encoding.UTF8.GetBytes(key);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim("userId", user.Id.ToString()),
            new Claim("isAdmin", user.IsAdmin.ToString())
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(keyBytes),
            SecurityAlgorithms.HmacSha256
            );

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: expires,
            signingCredentials: credentials
            );

        return new AuthResponseDto
        {
            Token = tokenHandler.WriteToken(token),
            Expires = expires
        };
    }

    private async Task<GoogleJsonWebSignature.Payload?> validationGoogleToken(string googleToken)
    {
        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(googleToken);
            return payload;
        }
        catch (InvalidJwtException ex)
        {
            throw new UnauthorizedAccessException("Invalid Google token.", ex);
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException("Invalid argument provided.", ex);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("An error occurred while validating the Google token.", ex);
        }
    }
}
