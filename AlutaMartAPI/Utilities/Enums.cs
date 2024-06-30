
using System.ComponentModel.DataAnnotations;

namespace AlutaMartAPI.Utilities;

public enum Roles
{
	[Display(Name ="Admin")]
	Admin = 1,

	[Display(Name ="Vendor")]
	Vendor,

	[Display(Name ="Buyer")]
	Buyer
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

public enum AdsDetailType
{
	[Display(Name = "Brand")]
	Brand = 1,

	[Display(Name = "")]
	LearnerDiscovery,

	[Display(Name = "Prerequisite")]
	Prerequisite
}

public enum QuizType
{
	[Display(Name = "Pre-Class")]
	PreClass = 1,

	[Display(Name = "In-Class")]
	InClass,

	[Display(Name = "Post-Class")]
	PostClass
}

public enum AdsType
{
	[Display(Name = "Discounted")]
	Discounted = 1,

	[Display(Name = "Fixed-Price")]
	FixedPrice,
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

public enum CareerGoal
{
	[Display(Name = "Enter a new field")]
	EnterANewField = 1,

	[Display(Name = "Advance in my field")]
	AdvanceInMyField,

	[Display(Name = "Become a manager in my field")]
	BecomeAManagerInMyfield,

	[Display(Name = "Advance as a manager")]
	AdvanceAsAManager
}