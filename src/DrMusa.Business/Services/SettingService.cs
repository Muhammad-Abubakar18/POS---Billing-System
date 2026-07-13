using DrMusa.Business.DTOs;
using DrMusa.Business.Interfaces;
using DrMusa.Data.Context;
using DrMusa.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DrMusa.Business.Services;

public class SettingService : ISettingService
{
    private readonly DrMusaDbContext _context;

    public SettingService(DrMusaDbContext context)
    {
        _context = context;
    }

    public async Task<string?> GetValueAsync(string key)
    {
        var setting = await _context.Settings.FirstOrDefaultAsync(s => s.Key == key);
        return setting?.Value;
    }

    public async Task SetValueAsync(string key, string value)
    {
        var setting = await _context.Settings.FirstOrDefaultAsync(s => s.Key == key);
        if (setting == null)
        {
            _context.Settings.Add(new Setting { Key = key, Value = value, UpdatedAt = DateTime.Now });
        }
        else
        {
            setting.Value = value;
            setting.UpdatedAt = DateTime.Now;
        }
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<SettingDto>> GetAllAsync()
    {
        return await _context.Settings
            .Select(s => new SettingDto(s.Key, s.Value, s.Description))
            .ToListAsync();
    }
}
