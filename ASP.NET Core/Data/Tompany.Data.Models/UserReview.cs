using System;
using System.Collections.Generic;
using System.Text;
using Tompany.Data.Common.Models;

namespace Tompany.Data.Models
{
    public class UserReview : BaseModel<int>, IDeletableEntity
    {
        public int UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public int ReviewId { get; set; }

        public virtual Review Review { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
