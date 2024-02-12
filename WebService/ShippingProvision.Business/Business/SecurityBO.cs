using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShippingProvision.Services;
using ShippingProvision.Services.Caching;

namespace ShippingProvision.Business
{
    public class SecurityBO : BaseBO<User>
    {
        public AuthToken AuthenticateUser(User user)
        {
            var authUser = this.Items
                             .Where(u => u.LoginName.Equals(user.LoginName)
                                 && u.LoginPassword.Equals(user.LoginPassword))
                             .Select(u => u)
                             .FirstOrDefault();


            AuthToken authToken = authUser != null ? new AuthToken
            {
                DisplayName = authUser.LoginName,
                Username = user.LoginName,
                UserId = authUser.Id,
                UserGroupId = authUser.UserGroupId,
            } : new AuthToken();

            if (authUser != null && !authUser.IsActive)
            {
                throw new Exception("User '" + authToken.DisplayName + "' is disabled. Please contact adminitrator.");
            }

            if (authToken.IsAuthenicated)
            {
                authToken.SessionId = Guid.NewGuid().ToString();
                ObjectCacheProvider.AddItem(authToken.SessionId, authToken, ObjectCacheProvider.ExpirationPolicy);
            }
            return authToken;
        }
    }
}
