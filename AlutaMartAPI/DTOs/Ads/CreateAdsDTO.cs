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

        [Required(ErrorMessage ="Ad quantity is required")]
        [Range(1, 999999, ErrorMessage ="Ad quantity value should be between 1 and 999999")]
        public int QuantityInStock { get; set; }

        public Guid? CurrencyId { get; set; }
        public double? DiscountPrice { get; set; }

        [Required(ErrorMessage ="course image url is required")]
        public List<string> AdsImageUrls { get; set; }
        public bool IsFeatured { get; set; } = false;

        [Range(1, 4, ErrorMessage ="AdType value should be between 1 and 4")]
        public AdsType AdsType { get; set; }

        [Required(ErrorMessage ="Ad quantity is required")]
        public AdsCondition AdsCondition { get; set; }
        public List<Guid> CategoryIds { get; set; }

    }