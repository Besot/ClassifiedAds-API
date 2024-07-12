
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



public enum ApprovalStatus
{
	[Display(Name = "None")]
	NotStarted = 1,

	[Display(Name = "Pending Approval")]
	PendingApproval,

	[Display(Name = "Declined")]
	Declined,

	[Display(Name = "Approved")]
	Approved
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
	YearFive

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