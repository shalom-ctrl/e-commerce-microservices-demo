using Authentication.Application.DTOs;
using Authentication.Application.Interfaces;
using Authentication.Infrastructure.Data;
using e_commerce.sharedlibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Infrastructure.Repositories
{
    public class UserRepository(AuthenticationDbContext context, IConfiguration configuration) : IUser
    {
        public async Task<AppUserDTO> GetUser(int userId)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == )
        }

        public Task<Response> Login(LoginDTO loginDTO)
        {
            throw new NotImplementedException();
        }

        public Task<Response> Register(AppUserDTO appUserDTO)
        {
            throw new NotImplementedException();
        }
    }
}
