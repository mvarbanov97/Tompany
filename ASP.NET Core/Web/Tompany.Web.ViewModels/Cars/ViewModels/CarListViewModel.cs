using System;
using System.Collections.Generic;
using System.Text;

namespace Tompany.Web.ViewModels.Cars.ViewModels
{
    public class CarListViewModel
    {
        public string UserUsername { get; set; }

        public string UserId { get; set; }

        public IEnumerable<CarViewModel> Cars { get; set; }
    }
}
