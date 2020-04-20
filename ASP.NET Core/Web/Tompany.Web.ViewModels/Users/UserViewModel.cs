using System;
using System.Collections.Generic;
using System.Text;
using Tompany.Data.Models;
using Tompany.Services.Mapping;

namespace Tompany.Web.ViewModels.Users
{
    public class UserViewModel : IMapFrom<ApplicationUser>
    {
        public string PhoneNumber { get; set; }

        public string UserName { get; set; }

        public DateTime CreatedOn { get; set; }


    }
}
