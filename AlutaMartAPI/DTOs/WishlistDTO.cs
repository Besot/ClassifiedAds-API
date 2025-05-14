using System;
using System.Collections.Generic;

namespace AlutaMartAPI.DTOs
{
    public class GetWishlistDTO
    {
        public Guid Id { get; set; }
        public List<GetWishlistItemDTO> WishlistItems { get; set; } = new List<GetWishlistItemDTO>();
    }

    public class GetWishlistItemDTO
    {
        public Guid Id { get; set; }
        public Guid AdsId { get; set; }
        public GetAdsDTO Ad { get; set; }
        public DateTimeOffset DateAdded { get; set; }
    }

    public class AddToWishlistDTO
    {
        public Guid AdsId { get; set; }
    }
}
