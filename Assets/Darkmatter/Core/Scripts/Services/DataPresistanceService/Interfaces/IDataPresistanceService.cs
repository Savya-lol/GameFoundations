namespace Darkmatter.Core.Services.DataPresistanceService.Interfaces
{
    public interface IDataPresistanceService
    {
        void SaveData<T>(string key, T data);
        T LoadData<T>(string key, T defaultValue = default);
    }
}