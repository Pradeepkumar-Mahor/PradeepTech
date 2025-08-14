using Microsoft.AspNetCore.Identity;
using PradeepTech.DataAccess.DBTran.Interface;
using PradeepTech.DataAccess.Models;
using PradeepTech.DataAccess.Models.Auth;
using PradeepTech.Domain.Context;
using PradeepTech.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PradeepTech.DataAccess.DBTran.Repositories
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthService(DataContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    //create role if it does not exist
                    var result = CreateRole(roleName);
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public bool CreateRole(string roleName)
        {
            if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
            {
                //create role if it does not exist
                _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                return true;
            }

            return false;
        }

        public bool DeleteRole(string roleName)
        {
            if (_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
            {
                //create role if it does not exist
                _roleManager.DeleteAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                return true;
            }

            return false;
        }

        public async Task<UserModel> Login(string UserName, string Password)
        {
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
            {
                return new UserModel();
            }

            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == UserName.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(user, Password);

            if (user == null || isValid == false)
            {
                return new UserModel();
            }

            //if user was found , Generate JWT Token
            var roles = await _userManager.GetRolesAsync(user);

            UserModel userModel = new()
            {
                Email = user.Email,
                ID = user.Id,
                Name = user.LastName + "" + user.LastName,
                PhoneNumber = user.PhoneNumber,
                Roles = roles
            };

            return userModel;
        }

        public async Task<string> Register(RegistrationModel RegistrationModel)
        {
            ApplicationUser user = new()
            {
                UserName = RegistrationModel.Email,
                Email = RegistrationModel.Email,
                NormalizedEmail = RegistrationModel.Email.ToUpper(),
                FirstName = RegistrationModel.FirstName,
                MiddleName = RegistrationModel.MiddleName,
                LastName = RegistrationModel.LastName,
                PhoneNumber = RegistrationModel.PhoneNumber
            };

            try
            {
                var result = await _userManager.CreateAsync(user, RegistrationModel.Password);
                if (result.Succeeded)
                {
                    var userToReturn = _db.ApplicationUsers.First(u => u.UserName == RegistrationModel.Email);

                    UserModel UserModel = new()
                    {
                        Email = userToReturn.Email,
                        ID = userToReturn.Id,
                        FirstName = userToReturn.FirstName,
                        MiddleName = userToReturn.MiddleName,
                        LastName = userToReturn.LastName,
                        Name = userToReturn.FirstName + " " + userToReturn.FirstName,
                        PhoneNumber = userToReturn.PhoneNumber,
                        ProfileImg = userToReturn.ProfileImg,
                    };

                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch (Exception)
            {
            }
            return "Error Encountered";
        }
    }
}