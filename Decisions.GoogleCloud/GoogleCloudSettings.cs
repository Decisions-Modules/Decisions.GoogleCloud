using System.Collections.Generic;
using System.ComponentModel;
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

namespace Decisions.GoogleCloud
{
    public class GoogleCloudSettings : AbstractModuleSettings, IInitializable, INotifyPropertyChanged, IValidationSource
    {

        [ORMField] 
        private bool useJsonFile = true;

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

        [ORMField]
        [PropertyHiddenByValue(nameof(UseJsonFile), false, true)]
        [PropertyClassification(1, "JSON File Path", "Credentials")]
        public string CredentialsJsonPath { get; set; }
        
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
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ValidationIssue[] GetValidationIssues()
        {
            List<ValidationIssue> issues = new List<ValidationIssue>();

            if (useJsonFile && string.IsNullOrEmpty(CredentialsJsonPath))
            {
                issues.Add(new ValidationIssue(CredentialsJsonPath, "JSON Path cannot be empty."));

            }

            return issues.ToArray();
        }
    }
}
