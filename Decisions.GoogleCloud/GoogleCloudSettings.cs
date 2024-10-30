using System;
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
        private CredentialsJson[] credentials;
        
        [DataMember]
        [PropertyHiddenByValue(nameof(UseJsonFile), false, true)]
        [PropertyClassification(1, "Credentials", "Credentials")]
        public CredentialsJson[] Credentials
        {
            get => credentials ?? Array.Empty<CredentialsJson>();
            set
            {
                credentials = value;
                OnPropertyChanged(nameof(Credentials));
            }
        }
        
        public void Initialize()
        {
            // Find all Google Add on Module Settings that need configuration.   
            
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
                // actions.Add(new EditEntityAction(GetType(), "Edit", null));
                
                actions.Add(new EditProviderAction("Edit Google Cloud Settings", "Allows the configuration of any Google Cloud settings.", GetContextProvider(), EditContextCompleted));
            }
            
            return actions.ToArray();
        }

        private void EditContextCompleted(IContextProvider sender, ContextProviderEditCompleteEventArgs args)
        {
            // Do nothing.
            DynamicORM orm = new DynamicORM();
            CompositeContextProvider provider = sender as CompositeContextProvider;
            foreach (IContextProvider eachProv in provider.GetProviders())
            {
                // Each one should be a reflection context provider.
                ReflectionContextProvider reflectionProvider = eachProv as ReflectionContextProvider;
                if (reflectionProvider != null)
                {
                    orm.Store((IORMEntity)reflectionProvider.Target);
                }
            }
        }

        // Registry of additional Google Module Settings if any are created and registered.
        private static List<AbstractAdditionalGoogleModuleSettings> ADDITIONAL_MODULE_SETTINGS = new List<AbstractAdditionalGoogleModuleSettings>();

        public static void RegisterAdditionalSettings(AbstractAdditionalGoogleModuleSettings additionalSettings)
        {
            ADDITIONAL_MODULE_SETTINGS.Add(additionalSettings);
        }

        private IContextProvider GetContextProvider()
        {
            CompositeContextProvider context = new CompositeContextProvider();
            
            // First add the main settings object. 
            context.AddProvider("Main Settings", new ReflectionContextProvider(this));

            if (ADDITIONAL_MODULE_SETTINGS.Count > 0)
            {
                foreach (AbstractAdditionalGoogleModuleSettings eachModuleAdd in ADDITIONAL_MODULE_SETTINGS)
                {
                    context.AddProvider(eachModuleAdd.EntityName, new ReflectionContextProvider(eachModuleAdd));
                }
            }

            return context;
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
