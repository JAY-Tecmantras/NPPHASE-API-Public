using Microsoft.EntityFrameworkCore;
using NPPHASE.Data.Interface;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using NPPHASE.Services.IRepositories;
using OfficeOpenXml;

namespace NPPHASE.Services.Repositories
{
    public class ContactsService : IContactsService
    {
        private readonly IRepository<Contact> _repository;
        private readonly IUnitofWork _unitofWork;
        private readonly IService<Contact> _service;

        public ContactsService(IUnitofWork unitofWork, IService<Contact> service)
        {
            _unitofWork = unitofWork;
            _repository = _unitofWork.GetRepository<Contact>();
            _service = service;
        }
        public async Task<PagedListViewModel<Contact>> GetAll(GetAllRequestViewModel model)
        {
            var result = _service.GetAllAsync(model).Where(x => x.DeviceUserId == model.DeviceUserId);
            var pagedResult = result;
            if (model.Page.HasValue && model.PageSize.HasValue)
            {
                pagedResult = pagedResult.Skip((model.Page.Value - 1) * model.PageSize.Value)
                                        .Take(model.PageSize.Value);
            }

            return new PagedListViewModel<Contact>
            {
                TotalCount = result.Count(),
                ListResponse = await pagedResult.ToListAsync()
            };
        }

        public async Task<byte[]> GetContactDetailsByExcel(GetAllRequestViewModel model)
        {
            var result = await _service.GetAllAsync(model).Where(x => x.DeviceUserId == model.DeviceUserId && !x.IsDeleted).Select(t => new Contact()
            {
                Name = t.Name,
                HomeNumber = t.HomeNumber,
                MobileNumber = t.MobileNumber,
                OfficeNumber = t.OfficeNumber,
                Email = t.Email
            }).ToListAsync();
            byte[] fileContents;
            int row = 2;
            ExcelPackage.License.SetNonCommercialOrganization("NP_Phase");
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Contacts");
                worksheet.HeaderFooter.FirstHeader.CenteredText = "\"Regular Bold\"";
                worksheet.Cells[1, 1].Value = "Name";
                worksheet.Cells[1, 2].Value = "HomeNumber";
                worksheet.Cells[1, 3].Value = "MobileNumber";
                worksheet.Cells[1, 4].Value = "OfficeNumber";
                worksheet.Cells[1, 5].Value = "Email";
                using (var range = worksheet.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Bold = true;
                }
                result.ForEach(x =>
                {
                    worksheet.Cells[row, 1].Value = x.Name;
                    worksheet.Cells[row, 2].Value = x.HomeNumber;
                    worksheet.Cells[row, 3].Value = x.MobileNumber;
                    worksheet.Cells[row, 4].Value = x.OfficeNumber;
                    worksheet.Cells[row, 5].Value = x.Email;
                    row++;
                });

                fileContents = package.GetAsByteArray();
            }
            return fileContents;
        }

        public async Task<bool> RemoveContacts(int deviceUserId)
        {
            var result = false;
            var Contacts = await _repository.GetAll().Where(x => x.DeviceUserId == deviceUserId && !x.IsDeleted).ToListAsync();
            if (Contacts.Any())
            {
                _repository.RemoveRange(Contacts);
                if (_unitofWork.commit() == 1)
                {
                    result = true;
                }
            }
            else
            {
                result = true;
            }
            return result;
        }
    }
}
