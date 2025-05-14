using System.ComponentModel.DataAnnotations;
using AlutaMartAPI.Models;

namespace AlutaMartAPI.DTOs;

public class CreateBankAccountDTO
{
    [Required(ErrorMessage = "Bank account number is required")]
    public string BankAccountNumber { get; set; }

    [Required(ErrorMessage = "Bank account name is required")]
    public string BankAccountName { get; set; }

    [Required(ErrorMessage = "Bank name is required")]
    public string BankName { get; set; }

    [Required(ErrorMessage = "Currency is required")]
    public string Currency { get; set; }

    [Required(ErrorMessage = "Security question answer is required")]
    public string SecurityQuestionAnswer { get; set; }

#nullable enable
    public string? BankCode { get; set; } = string.Empty;
#nullable disable
}


public class GetBankAccountsDTO
{
    public bool HasSetTransactionPIN { get; set; }

    public bool HasSetSecurityQuestion { get; set; }

    public string SecurityQuestion { get; set; }

    public List<BankAccount> BankAccounts { get; set; }
}


public class CreateTransactionPINDTO
{
    [Required(ErrorMessage = "PIN is required")]
    public string PIN { get; set; }

    [Required(ErrorMessage = "PIN Confirmation is required")]
    public string PINConfirmation { get; set; }
}


public class AddSecurityQuestionDTO
{
    [Required(ErrorMessage = "Security question key is required")]
    public string SecurityQuestionKey { get; set; }

    [Required(ErrorMessage = "Security question value is required")]
    public string SecurityQuestionAnswer { get; set; }
}