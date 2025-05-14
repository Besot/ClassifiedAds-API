using AlutaMartAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AlutaMartAPI.Database;
public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<Profile, IdentityRole<Guid>, Guid>(options)
{
    public virtual DbSet<Profile> Profiles { get; set; }
	public virtual DbSet<Ads> Ads { get; set; }
	public virtual DbSet<Category> Categories { get; set; }
	public virtual DbSet<AdsCategory> AdsCategories { get; set; }
	public virtual DbSet<AdsImage> AdsImages { get; set; }
	public virtual DbSet<AdsVideo> AdsVideos { get; set; }
	public virtual DbSet<BankAccount> BankAccounts { get; set; }
	public virtual DbSet<SecurityQuestion> SecurityQuestions { get; set; }
	
	public virtual DbSet<Report> Reports { get; set; }


	// public virtual DbSet<AdsComment> AdsComments { get; set; }
	public virtual DbSet<Currency> Currencies { get; set; }
	public virtual DbSet<Vendor> Vendors { get; set; }
	public virtual DbSet<VendorPlanTier> VendorPlan { get; set; }

	public virtual DbSet<Institution> Institutions { get; set; }

	public virtual DbSet<Cart> Carts { get; set; }
	public virtual DbSet<PlanTier> PlanTiers { get; set; }
	public virtual DbSet<PurchasedAd> PurchasedAds { get; set; }
	public virtual DbSet<Message> Messages { get; set; }
	public virtual DbSet<Conversation> Conversations { get; set; }


	public virtual DbSet<IdentityCard> IdentityCards { get; set; }
	public virtual DbSet<AdsReceipt> AdsReceipts { get; set; }
	public virtual DbSet<Review> Reviews { get; set; }

	public virtual DbSet<Wishlist> Wishlists { get; set; }
public virtual DbSet<WishlistItem> WishlistItems { get; set; }

	public virtual DbSet<UserInterestedInstitution> UserInterestedInstitutions { get; set; }

	public virtual DbSet<PaymentInflow> PaymentInflows { get; set; }
	public virtual DbSet<PaymentOutflow> PaymentOutflows { get; set; }
	public virtual DbSet<Transaction> Transactions { get; set; }
	public virtual DbSet<Wallet> Wallets { get; set; }
	public virtual DbSet<ProcessorDataLog> ProcessorDataLogs { get; set; }
	public virtual DbSet<AdsEngagement> AdsEngagements { get; set; }


	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Profile>().ToTable("Profiles");
		modelBuilder.Ignore<IdentityUserClaim<string>>();
		modelBuilder.Ignore<IdentityUserLogin<string>>();
		modelBuilder.Ignore<IdentityUserToken<string>>();
		modelBuilder.Ignore<IdentityRole>();
		modelBuilder.Ignore<IdentityRoleClaim<string>>();
		modelBuilder.Ignore<IdentityUserRole<string>>();


		#region Indexes

		modelBuilder.Entity<Profile>().HasIndex(b => b.Token);

		modelBuilder.Entity<Ads>().HasQueryFilter(p => !p.IsDeleted);
		modelBuilder.Entity<Category>().HasQueryFilter(p => !p.IsDeleted);
		modelBuilder.Entity<AdsImage>().HasQueryFilter(p => !p.IsDeleted);
		modelBuilder.Entity<AdsVideo>().HasQueryFilter(p => !p.IsDeleted);
		modelBuilder.Entity<AdsReceipt>().HasQueryFilter(p => !p.IsDeleted);
		// modelBuilder.Entity<AdsComment>().HasQueryFilter(p => !p.IsDeleted);
		modelBuilder.Entity<PurchasedAd>().HasQueryFilter(p => !p.IsDeleted);
		modelBuilder.Entity<AdsCategory>().HasQueryFilter(p => !p.IsDeleted);
		modelBuilder.Entity<AdsEngagement>().HasQueryFilter(p => !p.IsDeleted);
		modelBuilder.Entity<Report>().HasQueryFilter(p => !p.IsDeleted);
		modelBuilder.Entity<BankAccount>().HasQueryFilter(p => !p.IsDeleted);
		modelBuilder.Entity<SecurityQuestion>().HasQueryFilter(p => !p.IsDeleted);


		modelBuilder.Entity<Currency>().HasQueryFilter(p => !p.IsDeleted);
		modelBuilder.Entity<Vendor>().HasQueryFilter(p => !p.IsDeleted);
		modelBuilder.Entity<VendorPlanTier>().HasQueryFilter(p => !p.IsDeleted);

		modelBuilder.Entity<Institution>().HasQueryFilter(p => !p.IsDeleted);
		modelBuilder.Entity<Review>().HasQueryFilter(p => !p.IsDeleted);


		// modelBuilder.Entity<Buyer>().HasQueryFilter(p => !p.IsDeleted);
		modelBuilder.Entity<Cart>().HasQueryFilter(p => !p.IsDeleted);
		modelBuilder.Entity<PlanTier>().HasQueryFilter(p => !p.IsDeleted);
		modelBuilder.Entity<IdentityCard>().HasQueryFilter(p => !p.IsDeleted);
		modelBuilder.Entity<Wishlist>().HasQueryFilter(p => !p.IsDeleted);
		modelBuilder.Entity<WishlistItem>().HasQueryFilter(p => !p.IsDeleted);
		modelBuilder.Entity<Conversation>().HasQueryFilter(p => !p.IsDeleted);
		modelBuilder.Entity<Message>().HasQueryFilter(p => !p.IsDeleted);
		modelBuilder.Entity<UserInterestedInstitution>().HasQueryFilter(p => !p.IsDeleted);

		modelBuilder.Entity<PaymentInflow>().HasQueryFilter(p => !p.IsDeleted);
		modelBuilder.Entity<PaymentOutflow>().HasQueryFilter(p => !p.IsDeleted);
		modelBuilder.Entity<Transaction>().HasQueryFilter(p => !p.IsDeleted);
		modelBuilder.Entity<Wallet>().HasQueryFilter(p => !p.IsDeleted);
		modelBuilder.Entity<ProcessorDataLog>().HasQueryFilter(p => !p.IsDeleted);



		#endregion
	}
}
