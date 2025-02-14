using EmployeeHub.Dtos.AuthDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeHub.Application.Contracts.IService;

public interface IAuthService
{
    Task<AuthResponseDto> Register(RegisterDto registerDto);
    Task<AuthResponseDto> Login(LoginDto loginDto);
    Task<AuthResponseDto> LoginWithGoogle(string googleToken);
}
