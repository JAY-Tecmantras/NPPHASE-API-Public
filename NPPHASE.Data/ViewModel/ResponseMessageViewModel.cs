using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Data.ViewModel
{
    public class ResponseMessageViewModel
    {
        public object? Data { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
    public class PagedListViewModel<T> where T : class
    {
        public int TotalCount { get; set; }
        public List<T> ListResponse { get; set; }
    }
}
