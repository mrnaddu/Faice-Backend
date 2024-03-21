using Faice_Backend.Data;
using Faice_Backend.Models;
using Faice_Backend.RepositoryInterfaces;
using System;

namespace Faice_Backend.Repositories;

/*public class RepositoryAppUser(
    ILogger<AppUser> logger,
    FaiceDbContext dbContext) : IRepository<AppUser>
{
    private readonly FaiceDbContext _dbContext = dbContext;
    private readonly ILogger _logger = logger;

    public async Task<AppUser> Create(AppUser appuser)
    {
        try
        {
            if (appuser != null)
            {
                var obj = _dbContext.Add<AppUser>(appuser);
                await _dbContext.SaveChangesAsync();
                return obj.Entity;
            }
            else
            {
                return null;
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    public void Delete(AppUser appuser)
    {
        try
        {
            if (appuser != null)
            {
                var obj = _dbContext.Remove(appuser);
                if (obj != null)
                {
                    _dbContext.SaveChangesAsync();
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    public IEnumerable<AppUser> GetAll()
    {
        try
        {
            var obj = _dbContext.AppUsers.ToList();
            if (obj != null)
                return obj;
            else
                return null;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public AppUser GetById(int Id)
    {
        try
        {
            if (Id != null)
            {
                var Obj = _dbContext.AppUsers.FirstOrDefault(x => x.Id == Id);
                if (Obj != null)
                    return Obj;
                else
                    return null;
            }
            else
            {
                return null;
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    public void Update(AppUser appuser)
    {
        try
        {
            if (appuser != null)
            {
                var obj = _appDbContext.Update(appuser);
                if (obj != null)
                    _appDbContext.SaveChanges();
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
}*/
