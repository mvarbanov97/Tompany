using System;
using System.Collections.Generic;
using System.Text;
using Tompany.Data.Models;
using Tompany.Data.Models.Enums;
using Tompany.Services.Mapping;

namespace Tompany.Web.ViewModels.Users.ViewModels
{
    public class UserProfileViewModel
    {
        public ProfileTab ActiveTab { get; set; }

        public ApplicationUserViewModel ApplicationUser { get; set; }

        public bool HasAdmin { get; set; }

        public int Page { get; set; }

        public int CreatedPosts { get; set; }

        public int LikedPosts { get; set; }

        public int CommentsCount { get; set; }

        public double RatingScore { get; set; }

        public int LatestScore { get; set; }
    }
}
