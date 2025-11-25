using System;
using API.DTOs;
using API.Entities;
using API.Interfaces;

namespace API.Extensions;

public static class AppUserExtension
{
    public static UserDto ToDto(this AppUser user,ITokenService tokenService)
    {
          return new UserDto()
            {
                Name = user.DisplayName,
                Id = user.Id,
                Email = user.Email,
                Token = tokenService.CreateToken(user)
            };
    }

}
