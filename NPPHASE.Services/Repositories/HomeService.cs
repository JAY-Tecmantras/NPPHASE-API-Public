using Microsoft.EntityFrameworkCore;
using NPPHASE.Data.Interface;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;

namespace NPPHASE.Services.Repositories
{
    public class HomeService : IHomeService
    {
        private readonly IUnitofWork _unitofWork;
        private readonly IRepository<DeviceData> _repositoryData;
        public HomeService(IUnitofWork unitofWork)
        {
            _unitofWork = unitofWork;
            _repositoryData = _unitofWork.GetRepository<DeviceData>();
        }

        public async Task<PagedListViewModel<DashBoardDetailsViewModel>> GetAllDeviceUser()
        {
            var result = _repositoryData.GetAll().Include(y => y.DeviceUser.User).Where(x => !x.IsDeleted).OrderByDescending(x => x.CreationDate).Take(5);

            //if (model.FromDate.HasValue && model.ToDate.HasValue)
            //{
            //    result = result.Where(t => t.CreationDate.Date >= model.FromDate.Value.Date && t.CreationDate.Date <= model.ToDate.Value.Date);
            //}
            //var pagedResult = result;

            //if (model.Page.HasValue && model.PageSize.HasValue)
            //{
            //    return new PagedListViewModel<DashBoardDetailsViewModel>
            //    {
            //        TotalCount = result.Count(),
            //        ListResponse = await pagedResult.Select(x => new DashBoardDetailsViewModel
            //        {
            //            DeviceName = x.DeviceUser!=null ? x.DeviceUser.DeviceName : null,
            //            UserName = x.DeviceUser != null ? (x.DeviceUser.User!=null ? x.DeviceUser.User.UserName : null) : null ,
            //            CNIC = x.DeviceUser != null ? (x.DeviceUser.User != null ? x.DeviceUser.User.CNIC : null) : null,
            //            Number = x.DeviceUser != null ? (x.DeviceUser.User != null ? x.DeviceUser.User.ContactNumber : null) : null,
            //            PhoneId = x.DeviceUser != null ? x.DeviceUser.DeviceUniqueId : null,
            //            Email = x.DeviceUser != null ? (x.DeviceUser.User != null ? x.DeviceUser.User.Email : null) : null,
            //            DeviceUserId = x.DeviceUser != null ? x.DeviceUser.DeviceUserId : 0,
            //            DeviceUserUniqueId = x.DeviceUser != null ? x.DeviceUser.DeviceUniqueId : null,
            //            Status = x.Status.ToString(),
            //            CreatedDate = x.DeviceUser != null ? (x.DeviceUser.User != null ? x.DeviceUser.User.CreationDate.ToString() : null) : null,
            //            Password = x.DeviceUser != null ? (x.DeviceUser.User != null ? x.DeviceUser.User.PasswordNormal : null) : null,

            //        }).Skip((model.Page.Value - 1) * model.PageSize.Value)
            //                            .Take(model.PageSize.Value).ToListAsync()
            //    };
            //}
            return new PagedListViewModel<DashBoardDetailsViewModel>
            {
                TotalCount = result.Count(),
                ListResponse = await result.Select(x => new DashBoardDetailsViewModel
                {
                    Name = x.DeviceUser != null ? (x.DeviceUser.User != null ? x.DeviceUser.User.Name : null) : null,
                    DeviceName = x.DeviceUser != null ? x.DeviceUser.DeviceName : null,
                    UserName = x.DeviceUser != null ? (x.DeviceUser.User != null ? x.DeviceUser.User.UserName : null) : null,
                    CNIC = x.DeviceUser != null ? (x.DeviceUser.User != null ? x.DeviceUser.User.CNIC : null) : null,
                    Number = x.DeviceUser != null ? (x.DeviceUser.User != null ? x.DeviceUser.User.ContactNumber : null) : null,
                    PhoneId = x.DeviceUser != null ? x.DeviceUser.DeviceUniqueId : null,
                    Email = x.DeviceUser != null ? (x.DeviceUser.User != null ? x.DeviceUser.User.Email : null) : null,
                    DeviceUserId = x.DeviceUser != null ? x.DeviceUser.DeviceUserId : 0,
                    DeviceUserUniqueId = x.DeviceUser != null ? x.DeviceUser.DeviceUniqueId : null,
                    Status = x.Status.ToString(),
                    CreatedDate = x.DeviceUser != null ? (x.DeviceUser.User != null ? x.DeviceUser.User.CreationDate.ToString() : null) : null,
                    Password = x.DeviceUser != null ? (x.DeviceUser.User != null ? x.DeviceUser.User.PasswordNormal : null) : null,

                }).ToListAsync()
            };

        }
    }
}
