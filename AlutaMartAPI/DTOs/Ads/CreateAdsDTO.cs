using System.ComponentModel.DataAnnotations;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.DTOs;
    public class CreateAdsDTO
    {
        
        [Required(ErrorMessage ="Ads title is required")]
        [MaxLength(150, ErrorMessage ="Ads title should not be more than 150 characters")]
        [MinLength(2, ErrorMessage ="Ads title should not be less than 2 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage ="Ads description is required")]
        [MaxLength(4000, ErrorMessage ="Ads description should not be more than 4000 characters")]
        [MinLength(2, ErrorMessage ="Ads description should not be less than 3 characters")]
        public string Description { get; set; }
        
        [Range(0, 15999999, ErrorMessage = "Ads price should be between 0 and 15,999,999")]
        public double Price { get; set; }
        public Guid? AdsCategoryId { get; set; }
        public Guid? CurrencyId { get; set; }
        public double DiscountPrice { get; set; }

        [Required(ErrorMessage ="course image url is required")]
        [MaxLength(100, ErrorMessage ="course image url should not be more than 100 characters")]
        [MinLength(7, ErrorMessage ="course image url should not be less than 7 characters")]
        public List<string> AdsImageUrl { get; set; }
        public bool IsFeatured { get; set; } = false;

        [Range(1, 4, ErrorMessage ="roles value should be between 1 and 4")]
        public AdsType AdsType { get; set; }
        public AdsCondition AdsCondition { get; set; }
    }