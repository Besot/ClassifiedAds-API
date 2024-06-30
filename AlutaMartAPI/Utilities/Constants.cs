namespace AlutaMartAPI.Utilities;

public static class Constants
{
    
    public static string DBConnection => $"Host={Environment.GetEnvironmentVariable("DB_HOST")};"+
        $"Database={Environment.GetEnvironmentVariable("DB_NAME")};Username={Environment.GetEnvironmentVariable("DB_USER")};"+
        $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};Pooling=false;Timeout=300;CommandTimeout=300";

   
    public static string AWSSecreteKey => Environment.GetEnvironmentVariable("AWS_SECRETE_KEY");
    public static string AWSClientId => Environment.GetEnvironmentVariable("AWS_CLIENT_ID");
    public static string AWSS3Bucket => Environment.GetEnvironmentVariable("AWS_S3_BUCKET_NAME");
    public static string AWSS3BaseUrl => Environment.GetEnvironmentVariable("AWS_S3_BASE_URL");
    
    
    public static string AdminFirstName => Environment.GetEnvironmentVariable("ADMIN_FIRSTNAME");
    public static string AdminLastName => Environment.GetEnvironmentVariable("ADMIN_LASTNAME");
    public static string AdminPhone => Environment.GetEnvironmentVariable("ADMIN_PHONE");
    public static string AdminEmail => Environment.GetEnvironmentVariable("ADMIN_EMAIL");

    public static string ExpertFrontendBaseUrl => Environment.GetEnvironmentVariable("EXPERT_FRONT_END_BASE_URL");
    public static string LearnerFrontendBaseUrl => Environment.GetEnvironmentVariable("LEARNER_FRONT_END_BASE_URL");


    public static bool IsProduction => Environment.GetEnvironmentVariable("ENVIRONMENT").Equals("production", StringComparison.CurrentCultureIgnoreCase);
    public static string EnvironmentName => Environment.GetEnvironmentVariable("ENVIRONMENT");

    public static string RedisAddress => Environment.GetEnvironmentVariable("REDIS_ADDRESS");
    public static string RedisSecurityKey => Environment.GetEnvironmentVariable("REDIS_SECURITY_KEY");

    public static string JWTKey => Environment.GetEnvironmentVariable("JWT_KEY");
    public static string JWTKeyId => Environment.GetEnvironmentVariable("JWT_KEY_ID");
    public static string JWTIssuerAndAudience => Environment.GetEnvironmentVariable("JWT_ISSUER_AUDIENCE");

    public static string SentryKey => Environment.GetEnvironmentVariable("SENTRY_KEY");


    public static string PostMarkBaseURL => Environment.GetEnvironmentVariable("POSTMARKAPP_BASEURL");
    public static string PostMarkToken => Environment.GetEnvironmentVariable("POSTMARKAPP_TOKEN");

    public static string CurrencyCacheKey => "currency";
    public static string AdsCategoryCacheKey => "adsCategory";
    public static string VendorInstitutionCacheKey => "vendorInstitution";

    public static string GoogleClientId => Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
    public static string GoogleClientSecrete => Environment.GetEnvironmentVariable("GOOGLE_SECRETE_KEY");
    public static string GoogleCalendarScope => "https://www.googleapis.com/auth/calendar";
}