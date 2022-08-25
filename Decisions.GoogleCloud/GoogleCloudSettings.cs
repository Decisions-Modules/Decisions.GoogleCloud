using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using DecisionsFramework;
using DecisionsFramework.Data.ORMapper;
using DecisionsFramework.Design.Properties;
using DecisionsFramework.ServiceLayer;
using DecisionsFramework.ServiceLayer.Actions;
using DecisionsFramework.ServiceLayer.Actions.Common;
using DecisionsFramework.ServiceLayer.Services.Accounts;
using DecisionsFramework.ServiceLayer.Services.Administration;
using DecisionsFramework.ServiceLayer.Services.Folder;
using DecisionsFramework.ServiceLayer.Utilities;
using DecisionsFramework.Utilities;

namespace Decisions.GoogleCloud
{
    [ORMEntity("gcloud_settings")]
    [DataContract]
    [ValidationRules]
    public class GoogleCloudSettings : AbstractModuleSettings, IInitializable, INotifyPropertyChanged, IValidationSource
    {
        public GoogleCloudSettings()
        {
            EntityName = "Google Cloud Settings";
        }

        public sealed override string EntityName
        {
            get => base.EntityName;
            set => base.EntityName = value;
        }

        [ORMField] 
        private bool useJsonFile = true;

        [DataMember]
        [PropertyClassification(0, "Use JSON File", "Credentials")]
        public bool UseJsonFile
        {
            get
            {
                return useJsonFile;
            }
            set
            {
                useJsonFile = value;
                OnPropertyChanged(nameof(UseJsonFile));
            }
        }

        [ORMField(typeof(ORMXmlSerializedFieldConverter))]
        [PropertyHiddenByValue(nameof(UseJsonFile), false, true)]
        [PropertyClassification(1, "Credentials", "Credentials")]
        public CredentialsJson[] Credentials
        {
            get;
            set;
        }

        public void Initialize()
        {
            // Read the Settings here
            ModuleSettingsAccessor<GoogleCloudSettings>.GetSettings();
        }

        public override BaseActionType[] GetActions(AbstractUserContext userContext, EntityActionType[] types)
        {
            Account userAccount = userContext.GetAccount();

            FolderPermission permission = FolderService.Instance.GetAccountEffectivePermission(
                new SystemUserContext(), this.EntityFolderID, userAccount.AccountID);

            bool canAdministrate = FolderPermission.CanAdministrate == (FolderPermission.CanAdministrate & permission) ||
                                   userAccount.GetUserRights<PortalAdministratorModuleRight>() != null ||
                                   userAccount.IsAdministrator();

            List<BaseActionType> actions = new List<BaseActionType>();

            if (canAdministrate)
            {
                actions.Add(new EditEntityAction(GetType(), "Edit", null));
            }
            
            return actions.ToArray();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ValidationIssue[] GetValidationIssues()
        {
            List<ValidationIssue> issues = new List<ValidationIssue>();

            if (useJsonFile && Credentials == null)
            {
                issues.Add(new ValidationIssue("At least one set of credentials must be specified."));
            }

            return issues.ToArray();
        }
    }
}
