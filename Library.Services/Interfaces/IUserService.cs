using Library.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Services.Interfaces
{
    public interface IUserService
    {
        bool RegistrationNewUser(User user);
        bool Login(User user);
    }
}
