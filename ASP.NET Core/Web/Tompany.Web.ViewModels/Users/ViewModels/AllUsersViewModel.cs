using System;
using System.Collections.Generic;
using System.Text;
using Tompany.Data.Models;
using Tompany.Data.Models.Enums;
using Tompany.Services.Mapping;

namespace Tompany.Web.ViewModels.Users.ViewModels
{
    public class AllUsersViewModel : IMapTo<ApplicationUser>
    {
        public int Page { get; set; }

        public AllUsersTab ActiveTab { get; set; }

        public string Search { get; set; }
    }
}
