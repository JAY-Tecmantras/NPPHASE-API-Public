using NPPHASE.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Services.IRepositories
{
    public interface IExceptionLoggerServices
    {
        void InsertErrorLog(Exceptions exceptions);
    }
}
