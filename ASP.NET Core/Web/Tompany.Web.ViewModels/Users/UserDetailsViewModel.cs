using System;
using System.Collections.Generic;
using System.Text;
using Tompany.Data.Models;
using Tompany.Data.Models.Enums;
using Tompany.Services.Mapping;

namespace Tompany.Web.ViewModels.Users
{
    public class UserDetailsViewModel : IMapFrom<ApplicationUser>
    {
        public UserViewModel User { get; set; }
    }
}
