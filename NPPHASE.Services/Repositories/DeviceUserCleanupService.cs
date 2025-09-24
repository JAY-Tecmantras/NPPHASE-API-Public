using Microsoft.EntityFrameworkCore;
using NPPHASE.Data.Context;
using NPPHASE.Data.Implementations;
using NPPHASE.Data.Interface;
using NPPHASE.Data.Model;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Services.Repositories
{
    public class DeviceUserCleanupService : IDeviceUserCleanupService
    {
        private readonly IRepository<CallLog> _callLogService;
        private readonly IRepository<SMSLog> _smsLogs;
        private readonly IRepository<Gmail> _gmail;
        private readonly IRepository<Contact> _contacts;
        private readonly IRepository<Calendar> _calendar;
        private readonly IRepository<Whatsapp> _whatsapp;
        private readonly IRepository<Facebook> _facebook;
        private readonly IRepository<Gallery> _gallery;
        private readonly IRepository<Skype> _skype;
        private readonly IRepository<Kik> _kik;
        private readonly IRepository<Line> _line;
        private readonly IRepository<Viber> _viber;
        private readonly IRepository<InstalledApp> _installedApps;
        private readonly IRepository<DeviceData> _deviceData;
        private readonly IUnitofWork _unitofWork;
        private readonly IRepository<ScreenTime> _screenTime;
        private readonly IRepository<Location> _location;
        private readonly IRepository<WiFiNetwork> _wifiNetwork;

        public DeviceUserCleanupService(
            IUnitofWork unitofWork)
            //IRepository<CallLog> callLogService,
            //IRepository<SMSLog> smsLogs,
            //IRepository<Gmail> gmail,
            //IRepository<Contact> contacts,
            //IRepository<Calendar> calendar,
            //IRepository<Whatsapp> whatsapp,
            //IRepository<Facebook> facebook,
            //IRepository<Gallery> gallery,
            //IRepository<Skype> skype,
            //IRepository<Kik> kik,
            //IRepository<Line> line,
            //IRepository<Viber> viber,
            //IRepository<InstalledApp> installedApps,
            //IRepository<DeviceData> deviceData)
        {
            _unitofWork = unitofWork;
            _callLogService =  _unitofWork.GetRepository<CallLog>();
            _smsLogs = _unitofWork.GetRepository<SMSLog>();
            _gmail = _unitofWork.GetRepository<Gmail>();
            _contacts = _unitofWork.GetRepository<Contact>();
            _calendar = _unitofWork.GetRepository<Calendar>();
            _whatsapp = _unitofWork.GetRepository<Whatsapp>();
            _facebook = _unitofWork.GetRepository<Facebook>();
            _gallery = _unitofWork.GetRepository<Gallery>();
            _skype = _unitofWork.GetRepository<Skype>();
            _kik = _unitofWork.GetRepository<Kik>();
            _line = _unitofWork.GetRepository<Line>();
            _viber = _unitofWork.GetRepository<Viber>();
            _installedApps = _unitofWork.GetRepository<InstalledApp>();
            _deviceData = _unitofWork.GetRepository<DeviceData>();
            _screenTime = _unitofWork.GetRepository<ScreenTime>();
            _location = _unitofWork.GetRepository<Location>();
            _wifiNetwork = _unitofWork.GetRepository<WiFiNetwork>();
        }
        public async Task CleanupRelatedEntitiesAsync(int deviceUserId)
        {
            await _callLogService.GetAll().Where(x => x.DeviceUserId == deviceUserId).ExecuteDeleteAsync();
            await _smsLogs.GetAll().Where(x => x.DeviceUserId == deviceUserId).ExecuteDeleteAsync();
            await _gmail.GetAll().Where(x => x.DeviceUserId == deviceUserId).ExecuteDeleteAsync();
            await _contacts.GetAll().Where(x => x.DeviceUserId == deviceUserId).ExecuteDeleteAsync();
            await _calendar.GetAll().Where(x => x.DeviceUserId == deviceUserId).ExecuteDeleteAsync();
            await _whatsapp.GetAll().Where(x => x.DeviceUserId == deviceUserId).ExecuteDeleteAsync();
            await _facebook.GetAll().Where(x => x.DeviceUserId == deviceUserId).ExecuteDeleteAsync();
            await _gallery.GetAll().Where(x => x.DeviceUserId == deviceUserId).ExecuteDeleteAsync();
            await _skype.GetAll().Where(x => x.DeviceUserId == deviceUserId).ExecuteDeleteAsync();
            await _kik.GetAll().Where(x => x.DeviceUserId == deviceUserId).ExecuteDeleteAsync();
            await _line.GetAll().Where(x => x.DeviceUserId == deviceUserId).ExecuteDeleteAsync();
            await _viber.GetAll().Where(x => x.DeviceUserId == deviceUserId).ExecuteDeleteAsync();
            await _installedApps.GetAll().Where(x => x.DeviceUserId == deviceUserId).ExecuteDeleteAsync();
            await _screenTime.GetAll().Where(x => x.DeviceUserId == deviceUserId).ExecuteDeleteAsync();
            await _location.GetAll().Where(x => x.DeviceUserId == deviceUserId).ExecuteDeleteAsync();
            await _wifiNetwork.GetAll().Where(x => x.DeviceUserId == deviceUserId).ExecuteDeleteAsync();
            await _deviceData.GetAll().Where(x => x.DeviceUserId == deviceUserId).ExecuteDeleteAsync();
        }
    }
}
