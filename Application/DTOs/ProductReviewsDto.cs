using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public record ProductReviewsDto(
         int Id,
         string Review,
         decimal Rating,
         string ReviewerName,
         string ReviewerEmail,
         int? ReviewerId,
         int ProductId

    );
}