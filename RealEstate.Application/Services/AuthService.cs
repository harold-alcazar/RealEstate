
using Microsoft.AspNetCore.Identity;
using RealEstate.Application.Interfaces;
using RealEstate.Application.DTOs;
using RealEstate.Application.Results;
using RealEstate.Domain.Entities;
using RealEstate.Application.Exceptions;
using RealEstate.Application.IServices;

namespace RealEstate.Application.Services
{
    public class AuthService: IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenGenerator _jwt;

        public AuthService(UserManager<ApplicationUser> userManager,
                           SignInManager<ApplicationUser> signInManager,
                           IJwtTokenGenerator jwt)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwt = jwt;
        }

        public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest req)
        {
            var user = new ApplicationUser { UserName = req.Email, Email = req.Email, PhoneNumber = req.PhoneNumber };
            var result = await _userManager.CreateAsync(user, req.Password);
            
            if(!result.Succeeded)
            {
                throw new BadRequestException(string.Join("; ", result.Errors.Select(e => e.Description)));
            }

            var token = _jwt.Generate(user, new List<string>());
            return Result<AuthResponse>.Ok(new AuthResponse(token, user.Id, user.Email!));
        }

        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest req)
        {
            var user = await _userManager.FindByEmailAsync(req.Email);
            _ = user ?? throw new UnauthorizedException("Invalid credentials");

            var check = await _signInManager.CheckPasswordSignInAsync(user, req.Password, lockoutOnFailure: false);
            if (!check.Succeeded) throw new UnauthorizedException("Invalid credentials");

            var token = _jwt.Generate(user, new List<string>());
            return Result<AuthResponse>.Ok(new AuthResponse(token, user.Id, user.Email!));
        }
    }
}
