using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController(AppDbContext dbContext, ITokenService tokenService) : BaseApiController
    {
       [HttpPost("register")]  //  /api/account/register
       public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if(registerDto == null) return BadRequest("Provide information for user");

            if(await IsEmailExist(registerDto.Email)) return BadRequest("Email already Registered");

            using var hmac = new HMACSHA512();
            var user = new AppUser()
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            return user.ToDto(tokenService);
        } 


        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await dbContext.Users.SingleOrDefaultAsync(user => user.Email == loginDto.Email);
            if(user == null) return BadRequest("EmailID does not exist");

            using HMAC hmac = new HMACSHA512(user.PasswordSalt);

            var hashPassword = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for(int i=0;i<hashPassword.Length;i++)
            {
                if(hashPassword[i] != user.PasswordHash[i])
                {
                    return BadRequest("Invalid Password");

                }
            }

            return user.ToDto(tokenService);
        }

        private async Task<bool> IsEmailExist(string email)
        {
            return await dbContext.Users.AnyAsync(user=> user.Email.ToLower() == email.ToLower());
        }    
    }
}
