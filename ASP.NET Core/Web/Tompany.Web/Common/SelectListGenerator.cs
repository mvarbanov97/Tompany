using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tompany.Services.Data.Contracts;
using Tompany.Web.ViewModels.Destinations;

namespace Tompany.Web.Common
{
    public static class SelectListGenerator
    {
        public static IEnumerable<SelectListItem> GetAllDestinations(IDestinationService destinationsService)
        {
            var destinations = destinationsService.GetAllDestinationsAsync().GetAwaiter().GetResult();
            var groups = new List<SelectListGroup>();
            foreach (var destinationViewModel in destinations)
            {
                if (groups.All(g => g.Name != destinationViewModel.Name))
                {
                    groups.Add(new SelectListGroup { Name = destinationViewModel.Name });
                }
            }

            return destinations.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name,
            });
        }
    }
}