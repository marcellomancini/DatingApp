using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _ctx;
        public DatingRepository(DataContext ctx)
        {
            this._ctx = ctx;

        }
        public void Add<T>(T entity) where T : class
        {
            _ctx.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _ctx.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _ctx.Users.Include(u => u.Photos).FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var user = await _ctx.Users.Include(u => u.Photos).ToListAsync();
            return user;
        }

        public async Task<bool> SavaAll()
        {
            return await _ctx.SaveChangesAsync() > 0;
        }
    }
}