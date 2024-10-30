using System;
using System.Collections.Generic;
using System.Linq;
using Decisions.Silverlight.UI.Utilities;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using DecisionsFramework.Design.Properties;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;

namespace Decisions.GoogleCloud.Translate
{
    [Writable]
    [AutoRegisterStep("Translate Text with GCP", Constants.STEP_CATEGORY_TRANSLATION)]
    public class TranslateTextStep : AbstractGoogleCloudTranslationStep, IStepWithDefaultShapeSettings
    {
        private const string PATH_DONE = "Done";
        private const string PATH_ERROR = "Error";

        private const string INPUT_TEXT_TO_TRANSLATE = "Text to Translate";
        private const string INPUT_SOURCE_LANG = "Source Language";
        private const string INPUT_TARGET_LANG = "Target Language";
        
        private const string OUTPUT_RESULT_TEXT = "Translated Result";
        private const string OUTPUT_ERROR_MESSAGE = "Error Message";
        
        public override ResultData Run(StepStartData data)
        {
            TranslationServiceClient client = GetTranslationServiceClient(data);

            string projectId = GetProjectId(data);
            
            string errorMessage = null;
            TranslateTextRequest request = new TranslateTextRequest();
            request.SourceLanguageCode = data[INPUT_SOURCE_LANG] as string;
            request.TargetLanguageCode = data[INPUT_TARGET_LANG] as string;
            
            request.ParentAsLocationName = LocationName.FromProjectLocation(projectId, GetRegion(data));
            
            request.Contents.Add(data[INPUT_TEXT_TO_TRANSLATE] as string);
            try
            {
                TranslateTextResponse results = client.TranslateText(request);
                var firstTranslation = results.Translations.FirstOrDefault();
                if (firstTranslation != null)
                {
                    return new ResultData(PATH_DONE,
                        new KeyValuePair<string, object>(OUTPUT_RESULT_TEXT, firstTranslation.TranslatedText));
                }
            }
            catch (Exception r)
            {
                errorMessage = r.Message;
            }

            if (string.IsNullOrEmpty(errorMessage))
            {
                errorMessage = "Text could not be translated or resulted in empty translation.";
            }

            return new ResultData(PATH_ERROR, new KeyValuePair<string, object>(OUTPUT_ERROR_MESSAGE, errorMessage));
        }
        
        internal override IEnumerable<DataDescription> GetAdditionalInputs()
        {
            List<DataDescription> additionalInputs = new List<DataDescription>();

            additionalInputs.Add(new DataDescription(typeof(string), INPUT_TEXT_TO_TRANSLATE));
            additionalInputs.Add(new DataDescription(typeof(string), INPUT_TARGET_LANG)
            {
                Categories = new[] {"Language Settings"},
                SortIndex = 1,
                EditorAttribute = new SelectStringEditorAttribute(AllowedLanguages) { SortValues = true, AllowFreeText = true }
            });
            additionalInputs.Add(new DataDescription(typeof(string), INPUT_SOURCE_LANG)
            {
                Categories = new[] {"Language Settings"},
                SortIndex = 2,
                EditorAttribute = new SelectStringEditorAttribute(AllowedLanguages) { SortValues = true, AllowFreeText = true }
            });
            
            return additionalInputs;
        }

        public override OutcomeScenarioData[] OutcomeScenarios {
            get
            {
                return new[]
                {
                    new OutcomeScenarioData("Done", new DataDescription(typeof(string), OUTPUT_RESULT_TEXT)),
                    new OutcomeScenarioData(PATH_ERROR, new DataDescription(typeof(string), OUTPUT_ERROR_MESSAGE))
                };
            }
        }

        public FlowStepShapeSettings DefaultShapeSettings
        {
            get
            {
                FlowStepShapeSettings settings = new FlowStepShapeSettings();
                settings.ShapeType = FlowStepShapeType.Image;
                settings.HasImage = true;
                settings.ImageWidth = 60;
                settings.ImageHeight = 60;
                //settings.ImageOpacity = 60;
            
                settings.Image = new DecisionsFramework.ServiceLayer.Services.Image.ImageInfo()
                {
                    ImageId = Constants.STEP_ICON_TRANSLATION,
                    ImageType = DecisionsFramework.ServiceLayer.Services.Image.ImageInfoType.StoredImage
                };

                settings.ShapeSettings = new FlowStepCustomShapeSettings()
                {
                    BorderColor = DesignerColor.Transparent,
                    BackgroundColor = DesignerColor.Transparent 
                };
                
                settings.Width = 75;
                settings.Height = 75;
                return settings;
            }

        }
    }
}