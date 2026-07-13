using DrMusa.Business.DTOs;

namespace DrMusa.Business.Interfaces;

public interface ISettingService
{
    Task<string?> GetValueAsync(string key);
    Task SetValueAsync(string key, string value);
    Task<IEnumerable<SettingDto>> GetAllAsync();
}
