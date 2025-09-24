namespace NPPHASE.Services.IRepositories
{
    public interface IDeviceUserCleanupService
    {
        Task CleanupRelatedEntitiesAsync(int deviceUserId);
    }
}
