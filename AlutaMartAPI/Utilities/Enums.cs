
using System.ComponentModel.DataAnnotations;

namespace AlutaMartAPI.Utilities;

public enum Roles
{
	[Display(Name ="Super Admin")]
	SuperAdmin = 1,

	[Display(Name ="Vendor")]
	Vendor,

	[Display(Name ="Buyer")]
	Buyer,

	[Display(Name ="Admin User")]
	AdminUser,

	[Display(Name ="Business Manager")]
	BusinessManager,

	[Display(Name ="Platform Manager")]
	PlatformManager
}

public enum TokenType
{
	ResetPassword,
	BankTransfer
}

public enum AccessType
{
	Allow = 1,
	Block
}

public enum AdsCondition
{
    [Display(Name = "Brand New")]
    BrandNew = 1,

    [Display(Name = "Refurbished")]
    Refurbished,

    [Display(Name = "Used")]
    Used
}

public enum Discount
{
	[Display(Name = "Discounted")]
	Discounted = 1,

	[Display(Name = "Fixed-Price")]
	FixedPrice,
}

public enum AdsType
{
	[Display(Name = "For Sale")]
	ForSale = 1,

	[Display(Name = "For Rent")]
	ForRent,

	[Display(Name = "Services")]
	Services,

	[Display(Name = "Jobs")]
	Jobs,

	[Display(Name = "Campus Politics")]
	CampusPolitics
}



public enum VerificationStatus
{
	[Display(Name = "None")]
	NotVerify= 1,

	[Display(Name = "Verified")]
	Verified

}

public enum Level
{
	[Display(Name = "100L")]
	YearOne = 1,

	[Display(Name = "200L")]
	YearTwo,

	[Display(Name = "300L")]
	YearThree,

	[Display (Name = "400L")]
	YearFour,

	[Display(Name = "500L")]
	YearFive,
	[Display(Name = "Graduate")]
	Graduate


}

public enum AdsStatus
    {
        [Display(Name = "Active")]
        Active = 1,

        [Display(Name = "Inactive")]
        Inactive,

        [Display(Name = "Sold")]
        Sold,

        [Display(Name = "Expired")]
        Expired
    }

	public enum PaymentStatus
{
	[Display(Name = "Initiated")]
	Initiated = 1,

	[Display(Name = "Processing")]
	Processing,

	[Display(Name = "Successful")]
	Successful,

	[Display(Name = "Failed")]
	Failed,

	[Display(Name = "Cancled")]
	Cancled
}

public enum PaymentType
{
	[Display(Name = "Credit")]
	Inflow = 1,

	[Display(Name = "Debit")]
	Outflow
}

public enum PaymentProcessor
{
	[Display(Name = "Stripe")]
	Stripe = 1,

	[Display(Name = "Paystack")]
	Paystack
}