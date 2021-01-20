using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using Tompany.Data.Models;
using Tompany.Data.Models.Enums;
using Tompany.Services.Mapping;

namespace Tompany.Web.ViewModels.Users.ViewModels
{
    public class ApplicationUserViewModel : IMapFrom<ApplicationUser>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public Country Country { get; set; }

        public State State { get; set; }

        public City City { get; set; }

        public DateTime BirthDate { get; set; }

        public DateTime RegisteredOn { get; set; }

        public Gender Gender { get; set; }

        public string AboutMe { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ImageUrl { get; set; }

        public string CoverImageUrl { get; set; }

        public string GitHubUrl { get; set; }

        public string StackoverflowUrl { get; set; }

        public string FacebookUrl { get; set; }

        public string LinkedinUrl { get; set; }

        public string TwitterUrl { get; set; }

        public string InstagramUrl { get; set; }

        public bool IsBlocked { get; set; }

        public bool IsFollowed { get; set; }

        public ICollection<ApplicationRole> Roles { get; set; } = new HashSet<ApplicationRole>();

        public string GroupName { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<IdentityUserRole<string>, ApplicationRole>().ReverseMap();
        }
    }
}
