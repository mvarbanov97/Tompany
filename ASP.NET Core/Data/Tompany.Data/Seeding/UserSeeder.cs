using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tompany.Data.Models;

namespace Tompany.Data.Seeding
{
    public class UserSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();

            for (int i = 1; i <= 36; i++)
            {
                var result = await userManager.CreateAsync(
                    new ApplicationUser
                {
                    UserName = $"FooUser{i}",
                    Email = $"FooBar{i}@besl.bg",
                }, "awedawe1");


                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
                }

                var createdUser = await userManager.FindByNameAsync($"FooPlayer{i}");
            }
        }
    }
}
