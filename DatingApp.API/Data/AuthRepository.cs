using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _ctx;
        public AuthRepository(DataContext ctx)
        {
            _ctx = ctx;
        }
        public async Task<User> Login(string userName, string password)
        {
            var user=await _ctx.Users.FirstOrDefaultAsync(u=>u.UserName==userName);
            if(user==null)
                return null;
            if(!VerifyPasswordHash(password,user.PasswordHash,user.PasswordSalt))
                return null;
            return user;

        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
             using(var hmac=new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var hash =hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));                
                for(int i=0;i<hash.Length;i++)
                {
                    if(passwordHash[i]!=hash[i])
                        return false;
                }
                return true;
            }         
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            
            user.PasswordHash=passwordHash;
            user.PasswordSalt=passwordSalt;
            
            await _ctx.Users.AddAsync(user);
            await _ctx.SaveChangesAsync();
            
            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac=new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt=hmac.Key;
                passwordHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));                
            }            
        }

        public async Task<bool> UserExists(string userName)
        {
            if(await _ctx.Users.AnyAsync(u=>u.UserName==userName))
                return true;
            return false;
        }
    }
}