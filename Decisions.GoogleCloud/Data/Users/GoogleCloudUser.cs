using System.Linq;
using System.Runtime.Serialization;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Google.Apis.Admin.Directory.directory_v1.Data;

namespace Decisions.GoogleCloud.Data.Users;

[Writable, DataContract]
public class GoogleCloudUser
{
	[WritableValue, DataMember]
	public string Id { get; set; }
    
	[WritableValue, DataMember]
	public GoogleCloudUserName Name { get; set; }
    
	[WritableValue, DataMember]
	public string ETag { get; set; }
    
	[WritableValue, DataMember]
	public string CustomerId { get; set; }
    
	[WritableValue, DataMember]
	public string Kind { get; set; }
    
	[WritableValue, DataMember]
	public string PrimaryEmail { get; set; }
    
	[WritableValue, DataMember]
	public string RecoveryEmail { get; set; }
    
	[WritableValue, DataMember]
	public string RecoveryPhone { get; set; }
    
	[WritableValue, DataMember]
	public string OrgUnitPath { get; set; }
    
	[WritableValue, DataMember]
	public string HashFunction { get; set; }
    
	[WritableValue, DataMember]
	public string Password { get; set; }
    
	[WritableValue, DataMember]
	public string CreatedOn { get; set; }
    
	[WritableValue, DataMember]
	public string DeletedOn { get; set; }
    
	[WritableValue, DataMember]
	public string LastLoginOn { get; set; }
    
	[WritableValue, DataMember]
	public string[] Aliases { get; set; }
    
	[WritableValue, DataMember]
	public string[] NonEditableAliases { get; set; }
    
	[WritableValue, DataMember]
	public string ThumbnailPhotoEtag { get; set; }
    
	[WritableValue, DataMember]
	public string ThumbnailPhotoUrl { get; set; }
	
    
	[WritableValue, DataMember]
	public bool? AgreedToTerms { get; set; }
    
	[WritableValue, DataMember]
	public bool? Archived { get; set; }
    
	[WritableValue, DataMember]
	public bool? ChangePasswordAtNextLogin  { get; set; }
    
	[WritableValue, DataMember]
	public bool? IncludeInGlobalAddressList { get; set; }
    
	[WritableValue, DataMember]
	public bool? IpWhitelisted { get; set; }
    
	[WritableValue, DataMember]
	public bool? IsAdmin { get; set; }
    
	[WritableValue, DataMember]
	public bool? IsDelegatedAdmin { get; set; }
    
	[WritableValue, DataMember]
	public bool? IsEnforcedIn2Sv { get; set; }
    
	[WritableValue, DataMember]
	public bool? IsEnrolledIn2Sv { get; set; }
    
	[WritableValue, DataMember]
	public bool? IsMailboxSetup	{ get; set; }
    
	[WritableValue, DataMember]
	public bool? Suspended { get; set; }
    
	[WritableValue, DataMember]
	public string SuspensionReason { get; set; }
	
	public GoogleCloudUserAddress[] Addresses { get; set; }
	public GoogleCloudUserEmail[] Emails { get; set; }
	public GoogleCloudUserPhone[] Phones { get; set; }
	
    public static GoogleCloudUser FromUser(User user)
    {
        return new GoogleCloudUser() 
        {
	        Id = user.Id,
	        Name = GoogleCloudUserName.FromUserName(user.Name),
	        ETag = user.ETag,
	        CustomerId = user.CustomerId,
	        Kind = user.Kind,
	        PrimaryEmail = user.PrimaryEmail,
	        RecoveryEmail = user.RecoveryEmail,
	        RecoveryPhone = user.RecoveryPhone,
	        OrgUnitPath = user.OrgUnitPath,
	        HashFunction = user.HashFunction,
	        Aliases = user.Aliases.ToArray(),
	        NonEditableAliases = user.NonEditableAliases.ToArray(),
	        ThumbnailPhotoEtag = user.ThumbnailPhotoEtag,
	        ThumbnailPhotoUrl = user.ThumbnailPhotoUrl,
	        AgreedToTerms = user.AgreedToTerms,
	        Archived = user.Archived,
	        ChangePasswordAtNextLogin = user.ChangePasswordAtNextLogin,
	        IncludeInGlobalAddressList = user.IncludeInGlobalAddressList,
	        IpWhitelisted = user.IpWhitelisted,
	        IsAdmin = user.IsAdmin,
	        IsDelegatedAdmin = user.IsDelegatedAdmin,
	        IsEnforcedIn2Sv = user.IsEnforcedIn2Sv,
	        IsEnrolledIn2Sv = user.IsEnrolledIn2Sv,
	        IsMailboxSetup = user.IsMailboxSetup,
	        Suspended = user.Suspended,
	        SuspensionReason = user.SuspensionReason,
	        Addresses = user.Addresses.Select(GoogleCloudUserAddress.FromUserAddress).ToArray(),
	        Emails = user.Emails.Select(GoogleCloudUserEmail.FromUserEmail).ToArray(),
	        Phones = user.Phones.Select(GoogleCloudUserPhone.FromUserPhone).ToArray(),
        };
    }

    public User ToUser()
    {
        return new User()
        {
	        Id = Id,
	        Name = Name.ToUserName(),
	        ETag = ETag,
	        CustomerId = CustomerId,
	        Kind = Kind,
	        PrimaryEmail = PrimaryEmail,
	        RecoveryEmail = RecoveryEmail,
	        RecoveryPhone = RecoveryPhone,
	        OrgUnitPath = OrgUnitPath,
	        HashFunction = HashFunction,
	        Password = Password,
	        Aliases = Aliases,
	        NonEditableAliases = NonEditableAliases,
	        ThumbnailPhotoEtag = ThumbnailPhotoEtag,
	        ThumbnailPhotoUrl = ThumbnailPhotoUrl,
	        AgreedToTerms = AgreedToTerms,
	        Archived = Archived,
	        ChangePasswordAtNextLogin = ChangePasswordAtNextLogin,
	        IncludeInGlobalAddressList = IncludeInGlobalAddressList,
	        IpWhitelisted = IpWhitelisted,
	        IsAdmin = IsAdmin,
	        IsDelegatedAdmin = IsDelegatedAdmin,
	        IsEnforcedIn2Sv = IsEnforcedIn2Sv,
	        IsEnrolledIn2Sv = IsEnrolledIn2Sv,
	        IsMailboxSetup = IsMailboxSetup,
	        Suspended = Suspended,
	        SuspensionReason = SuspensionReason,
	        Addresses = Addresses.Select(m => m.ToUserAddress()).ToArray(),
	        Emails = Emails.Select(m => m.ToUserEmail()).ToArray(),
	        Phones = Phones.Select(m => m.ToUserPhone()).ToArray(),
        };
    }
}