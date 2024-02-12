using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;
using System.Diagnostics.Contracts;

namespace ShippingProvision.Business
{
    public class UserBO:BaseBO<User>
    {
        public long RegisterUser(User user)
        {
            user.IsActive = true;
            //mandatory check
            //Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(user.Username), "Username");
            //check already exists
            if (IsUserExists(user.LoginName))
            {
                throw new Exception("User already exists.");
            }
            //encrypt password

            user.UserGroupId = (int)UserGroup.User;
            user.CompanyCode = BOHelper.GetCompanyCodeById(user.CompanyId);

            //create user
            this.SaveOrUpdate(user);
            //send email
            return user.Id;
        }

        public long UpdateUser(User user)
        {
            var existingUser = this.GetById(user.Id);

            existingUser.CompanyId = user.CompanyId;
            existingUser.CompanyCode = BOHelper.GetCompanyCodeById(user.CompanyId);
            existingUser.LoginName = user.LoginName;            
            existingUser.FullName = user.FullName;
            existingUser.Gender = user.Gender;
            existingUser.Role = user.Role;
            existingUser.Email = user.Email;
            existingUser.Phone = user.Phone;            
            existingUser.Mobile = user.Mobile;
            existingUser.Fax = user.Fax;
            existingUser.Address = user.Address;

            this.SaveOrUpdate(existingUser);
            return existingUser.Id;
        }


        public bool IsUserExists(String userName)
        {
            var userId = this.Items
                           .Where(u => u.LoginName == userName)
                           .Select(u => u.Id)
                           .FirstOrDefault();
            return userId != 0;
        }

        public long DeleteUser(long userId)
        {            
            var user = this.GetById(userId);
            if (user == null)
            {
                throw new Exception("Non-existing user.");
            }

            this.MarkAsDelete(user);
          
            return userId;
        }

        public IList<User> GetUsers()
        {
            IList<User> lsResult = this.Items.Where(item => item.Status == Constants.STATUS_LIVE && item.UserGroupId == (int)UserGroup.User).ToList();
            return lsResult;
        }

        //This method used by admin 
        public long ResetPassword(long userId, string password)
        {
            var existingUser = this.GetById(userId);
            existingUser.LoginPassword = password;
            this.SaveOrUpdate(existingUser);

            return userId;
        }

        //This method used for user change password by his own
        public long ChangePassword(long userId, string oldpassword, string newpassword)
        {
            var existingUser = this.GetById(userId);

            if (existingUser.LoginPassword != oldpassword)
            {
                throw new Exception("Old password is incorrect");
            }
            
            existingUser.LoginPassword = newpassword;
            this.SaveOrUpdate(existingUser);

            return userId;
        }

        //This method used for user change profile details by his own
        public long UpdateMyProfile(User user)
        {
            var existingUser = this.GetById(user.Id);            
           
            existingUser.FullName = user.FullName;
            existingUser.Gender = user.Gender;
            existingUser.Role = user.Role;
            existingUser.Email = user.Email;
            existingUser.Phone = user.Phone;
            existingUser.Mobile = user.Mobile;
            existingUser.Fax = user.Fax;
            existingUser.Address = user.Address;            

            this.SaveOrUpdate(existingUser);
            return existingUser.Id;
        }

        public List<OptionItem> GetOptions()
        {
            var options = this.Items
                              .Where(i => i.Status == Constants.STATUS_LIVE)
                              .Where(i => i.UserGroupId == (int)UserGroup.User)
                              .Where(i => i.IsActive)
                              .OrderBy(i => i.FullName)
                              .Select(i => new OptionItem() { Id = i.Id, Text = i.FullName})
                              .ToList();
            return options;
        }

        public UserBO() { }

        
    }
}
