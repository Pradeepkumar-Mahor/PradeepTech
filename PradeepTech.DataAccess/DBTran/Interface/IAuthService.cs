using PradeepTech.DataAccess.Models;
using PradeepTech.DataAccess.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PradeepTech.DataAccess.DBTran.Interface
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationModel registrationModel);

        Task<UserModel> Login(string UserName, string Password);

        Task<bool> AssignRole(string email, string roleName);

        public bool CreateRole(string roleName);

        public bool DeleteRole(string roleName);
    }
}