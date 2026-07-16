using Microsoft.EntityFrameworkCore;
using SmarketApiOracle.Data;
using SmarketApiOracle.Models;

namespace SmarketApiOracle.Services
{
    public class ClientService
    {
        private readonly ApplicationDbContext _db;
        public ClientService(ApplicationDbContext db) => _db = db;

        public async Task<List<TblClient>> GetAllAsync()
        {
            return await _db.TblClient.ToListAsync();
        }

        public async Task<TblClient?> GetByIdAsync(int id)
        {
            return await _db.TblClient.FindAsync(id);
        }

        public async Task<TblClient> AddAsync(TblClient client)
        {
            int nextId = _db.TblClient.Any()
                ? _db.TblClient.Max(c => c.ClientId) + 1
                : 1;

            client.ClientId    = nextId;
            client.ClientCode  = $"CL-{nextId:D3}-{DateTime.Now.Year}";
            client.DateCreated = DateTime.Now;

            _db.TblClient.Add(client);
            await _db.SaveChangesAsync();
            return client;
        }

        public async Task<TblClient?> UpdateAsync(int id, TblClient client)
        {
            var existing = await _db.TblClient.FindAsync(id);
            if (existing is null) return null;

            existing.ClientName    = client.ClientName;
            existing.ClientPhone   = client.ClientPhone;
            existing.ClientEmail   = client.ClientEmail;
            existing.ClientAddress = client.ClientAddress;
            existing.IsActive      = client.IsActive;

            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var client = await _db.TblClient.FindAsync(id);
            if (client is null) return false;

            _db.TblClient.Remove(client);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
