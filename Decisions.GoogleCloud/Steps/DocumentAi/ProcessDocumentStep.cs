using DecisionsFramework.Data.DataTypes;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Google.Cloud.DocumentAI.V1;
using Google.Protobuf;

namespace Decisions.GoogleCloud.Steps.DocumentAi;

[Writable]
[AutoRegisterStep("Process Document", Constants.STEP_CATEGORY_DOCUMENT_AI)]
public class ProcessDocumentStep : BaseCredentialsStep
{
    private const string INPUT_PROJECT_ID = "Project Id";
    private const string INPUT_LOCATION_ID = "Location Id";
    private const string INPUT_PROCESSOR_ID = "Processor Id";
    private const string INPUT_FILE = "File";
    private const string OUTPUT_GROUP_SUCCESS = "Success";
    private const string PATH_DONE  = "Done";
    
    public override DataDescription[] InputData =>
    [
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_PROJECT_ID, false, false, false),
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_LOCATION_ID, false, false, false),
        new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_PROCESSOR_ID, false, false, false),
        new DataDescription(new DecisionsNativeType(typeof(FileData)), INPUT_FILE, false, false, false),
    ];
    
    public override OutcomeScenarioData[] OutcomeScenarios =>
    [
        new OutcomeScenarioData(PATH_DONE, new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(bool)), OUTPUT_GROUP_SUCCESS, false, true, false)})
    ];

    public override ResultData Run(StepStartData data)
    {
        var projectId = data[INPUT_PROJECT_ID] as string;
        var locationId = data[INPUT_LOCATION_ID] as string;
        var processorId = data[INPUT_PROCESSOR_ID] as string;
        var fileData = data[INPUT_FILE] as FileData;

        CredentialsJson credentials = GoogleCloudUtility.GetCredentialsByName(Credentials);
        var result = ProcessDocument(credentials, projectId, locationId, processorId, fileData);

        return default;
    }

    public static string ProcessDocument(CredentialsJson credentials, string projectId, string locationId, string processorId, FileData fileData)
    {
        var client = GoogleCloudUtility.GetDocumentProcessorClient(credentials, locationId);

        ProcessorName processorName = ProcessorName.FromProjectLocationProcessor(projectId, locationId, processorId);
        ByteString content = ByteString.CopyFrom(fileData.Contents);

        var document = new RawDocument()
        {
            Content = content,
            MimeType = "application/pdf",
        };

        var request = new ProcessRequest()
        {
            Name = processorName.ToString(),
            RawDocument = document,
        };

        var result = client.ProcessDocument(request);

        return result.Document.Text;
    }
}