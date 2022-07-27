using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using DecisionsFramework;
using DecisionsFramework.Data.DataTypes;
using DecisionsFramework.Data.ORMapper;
using DecisionsFramework.Design.Properties;
using DecisionsFramework.ServiceLayer;
using DecisionsFramework.ServiceLayer.Actions;
using DecisionsFramework.ServiceLayer.Actions.Common;
using DecisionsFramework.ServiceLayer.Services.Accounts;
using DecisionsFramework.ServiceLayer.Services.Administration;
using DecisionsFramework.ServiceLayer.Services.Folder;
using DecisionsFramework.ServiceLayer.Utilities;

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
        
        [ORMField(typeof(ORMBinaryFieldConverter))]
        private FileData credentialsJson;
        
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
        
        [DataMember]
        [PropertyHiddenByValue(nameof(UseJsonFile), false, true)]
        [PropertyClassification(1, "JSON File", "Credentials")]
        public FileData CredentialsJson
        {
            get { return credentialsJson; }
            set
            {
                credentialsJson = value;
                OnPropertyChanged(nameof(CredentialsJson));
            }
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

            if (canAdministrate)
                return new BaseActionType[]
                {
                    new EditEntityAction(GetType(), "Edit", null),
                };

            return new BaseActionType[0];
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ValidationIssue[] GetValidationIssues()
        {
            List<ValidationIssue> issues = new List<ValidationIssue>();

            if (useJsonFile && CredentialsJson == null)
            {
                issues.Add(new ValidationIssue(nameof(CredentialsJson), "JSON Path cannot be empty."));

            }

            return issues.ToArray();
        }
    }
}
