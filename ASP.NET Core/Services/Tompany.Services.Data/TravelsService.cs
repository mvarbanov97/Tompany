using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tompany.Data.Common.Repositories;
using Tompany.Data.Models;
using Tompany.Services.Data.Contracts;
using Tompany.Web.ViewModels.Travels;

namespace Tompany.Services.Data
{
    public class TravelsService : ITravelsService
    {
        private readonly IRepository<Travel> travelsRepository;

        public TravelsService(
            IRepository<Travel> travelsRepository)
        {
            this.travelsRepository = travelsRepository;
        }

        public async Task CreateAsync(TravelCreateInputModel input)
        {
            var travel = new Travel
            {
                FromCity = input.FromCity,
                ToCity = input.ToCity,
                DateOfDeparture = input.DateOfDeparture,
                TimeOfDeparture = input.TimeOfDeparture,
                AdditionalInformation = input.AdditionalInformation
            };

            await this.travelsRepository.AddAsync(travel);
            await this.travelsRepository.SaveChangesAsync();
        }
    }
}
