using System;
using System.Collections.Generic;

namespace AlutaMartAPI.Models
{
    public class Wishlist : BaseEntity
    {
        public Guid BuyerId { get; set; }
        public virtual Buyer Buyer { get; set; }
        public virtual ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
    }

    public class WishlistItem : BaseEntity
    {
        public Guid WishlistId { get; set; }
        public virtual Wishlist Wishlist { get; set; }
        public Guid AdsId { get; set; }
        public virtual Ads Ads { get; set; }
    }
}
