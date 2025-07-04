#nullable enable
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine.UI;

namespace OpenAI
{
    #region Common Data Types

    public struct Choice
    {
        public string Text { get; set; }
        public int? Index { get; set; }
        public int? Logprobs { get; set; }
        public string FinishReason { get; set; }
    }

    public struct Usage
    {
        public string PromptTokens { get; set; }
        public string CompletionTokens { get; set; }
        public string TotalTokens { get; set; }
        public int? InputTokens { get; set; }
        public int? OutputTokens { get; set; }
    }

    public class OpenAIFile
    {
        public string Prompt { get; set; }
        public object Completion { get; set; }
        public string Id { get; set; }
        public string Object { get; set; }
        public long Bytes { get; set; }
        public long CreatedAt { get; set; }
        public string Filename { get; set; }
        public string Purpose { get; set; }
        public object StatusDetails { get; set; }
        public string Status { get; set; }
    }

    public class OpenAIFileResponse : OpenAIFile, IResponse
    {
        public ApiError Error { get; set; }
        public string Warning { get; set; }
    }

    public class ApiError
    {
        public string Message;
        public string Type;
        public object Param;
        public object Code;
    }

    public struct Auth
    {
        [JsonRequired] public string ApiKey { get; set; }
        public string Organization { get; set; }
    }

    #endregion

    #region Models API Data Types

    public struct ListModelsResponse : IResponse
    {
        public ApiError Error { get; set; }
        public string Warning { get; set; }
        public string Object { get; set; }
        public List<OpenAIModel> Data { get; set; }
    }

    public class OpenAIModel
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public string OwnedBy { get; set; }
        public long Created { get; set; }
        public string Root { get; set; }
        public string Parent { get; set; }
        public List<Dictionary<string, object>> Permission { get; set; }
    }

    public class OpenAIModelResponse : OpenAIModel, IResponse
    {
        public ApiError Error { get; set; }
        public string Warning { get; set; }
    }

    #endregion

    #region Chat API Data Types

    public sealed class CreateChatCompletionRequest
    {
        public string Model { get; set; }
        public List<ChatMessage> Messages { get; set; }
        public float? Temperature { get; set; } = 1;
        public int N { get; set; } = 1;
        public bool Stream { get; set; } = false;
        public string Stop { get; set; }
        public int? MaxTokens { get; set; }
        public float? PresencePenalty { get; set; } = 0;
        public float? FrequencyPenalty { get; set; } = 0;
        public Dictionary<string, string> LogitBias { get; set; }
        public string User { get; set; }
        public string SystemFingerprint { get; set; }
        public List<Tool> Tools { get; set; }
    }

    public struct CreateChatCompletionResponse : IResponse
    {
        public ApiError Error { get; set; }
        public string Warning { get; set; }
        public string Model { get; set; }
        public string Id { get; set; }
        public string Object { get; set; }
        public long Created { get; set; }
        public List<ChatChoice> Choices { get; set; }
        public Usage Usage { get; set; }
        public string SystemFingerprint { get; set; }
    }

    public struct ChatChoice
    {
        public ChatMessage Message { get; set; }
        public ChatMessage Delta { get; set; }
        public int? Index { get; set; }
        public string FinishReason { get; set; }
        public string Logprobs { get; set; }
    }

    public struct ChatMessage
    {
        public string Role { get; set; }
        public string Content { get; set; }
        public List<ToolCallsType>? ToolCalls { get; set; }
        public string? Type { get; set; }
        public string? CallId { get; set; }
        public string? Output { get; set; }
    }

    public struct ToolCallsType
    {
        public string Id { get; set; }
        public string CallId { get; set; }
        public string Type { get; set; }
        public ToolFunctionResponse Function { get; set; }
    }

    public struct ToolFunctionResponse
    {
        public string Name { get; set; }
        public string Arguments { get; set; } // Json
    }

    public struct Tool
    {
        public string Type { get; set; } //	This should always be function
        public ToolFunction? Function { get; set; } //	This should always be function
        public string? Name { get; set; } //	The function's name (e.g. get_weather)
        public string? Description { get; set; } //	Details on when and how to use the function
        public ToolParameters? Parameters { get; set; } //	JSON schema defining the function's input arguments
        public bool? Strict { get; set; } //	Whether to enforce strict mode for the function call
    }

    public struct ToolFunction
    {
        public string Name { get; set; } //	The function's name (e.g. get_weather)
        public string Description { get; set; } //	Details on when and how to use the function
        public ToolParameters Parameters { get; set; } //	JSON schema defining the function's input arguments
        public bool Strict { get; set; } //	Whether to enforce strict mode for the function call
    }

    public struct ResponseTool
    {
        public string Type { get; set; } //	This should always be function
        public string Name { get; set; } //	The function's name (e.g. get_weather)
        public string Description { get; set; } //	Details on when and how to use the function
        public ToolParameters Parameters { get; set; } //	JSON schema defining the function's input arguments
        public bool Strict { get; set; } //	Whether to enforce strict mode for the function call
    }

    public struct ToolParameters
    {
        public string Type { get; set; }
        public Dictionary<string, ToolProperty> Properties { get; set; }
        public List<string> Required { get; set; }
        public bool AdditionalProperties { get; set; }
    }

    public struct ToolProperty
    {
        public string Type { get; set; }
        public string Description { get; set; }
        public List<string> Enum { get; set; }
    }

    #endregion

    #region Response Data Types

    // See https://platform.openai.com/docs/api-reference/responses/create
    public struct ResponseRequest
    {
        public bool Background { get; set; }
        public List<string> Include { get; set; }
        public List<InputResponse> Input { get; set; }
        public string? Instructions { get; set; }
        public int? MaxOutputTokens { get; set; }
        public int? MaxToolCalls { get; set; }
        public Dictionary<string, string>? Metadata { get; set; }
        public string? Model { get; set; }
        public bool? ParallelToolCalls { get; set; }
        public string? PreviousResponseId { get; set; }
        public Prompt? Prompt { get; set; }
        public Reasoning? Reasoning { get; set; }
        public string? ServiceTier { get; set; }
        public bool? Store { get; set; }
        public bool? Stream { get; set; } // No?
        public float? Temperature { get; set; }
        public TextType? Text { get; set; } // Object
        public string ToolChoice { get; set; }
        public List<ResponseTool>? Tools { get; set; }
        public int? TopLogprobs { get; set; }
        public float? TopP { get; set; }
        public string? Truncation { get; set; }
        public string? User { get; set; }
    }

    public struct TextType
    {
        public TextTypeFormat Format { get; set; }
    }

    public struct TextTypeFormat
    {
        public string Type { get; set; } // "text" or "json_object" (json_schema not supported)
    }

    public struct Prompt
    {
        public string Id { get; set; }
        public Dictionary<string, string>? Variables { get; set; }
        public string? Version { get; set; }
    }

    public struct Reasoning
    {
        public string? Effort { get; set; } // low, medium or high
        public string? Summary { get; set; } // auto, concise, or detailed
    }

    public struct Details
    {
        public string Reason { get; set; }
    }

    public struct Response : IResponse
    {
        public bool Background { get; set; } //
        public float CreatedAt { get; set; } //
        public string Id { get; set; } //
        public Details? IncompleteDetails { get; set; } //
        public string? Instructions { get; set; } //
        public int? MaxOutputTokens { get; set; } //
        public int? MaxToolCalls { get; set; } //
        public Dictionary<string, string>? Metadata { get; set; } //
        public string? Model { get; set; } //
        public string? Object { get; set; } //
        /// Different types possible!!!
        public List<OutputResponse> Output { get; set; }
        public bool? ParallelToolCalls { get; set; } //
        public string? PreviousResponseId { get; set; } //
        public Prompt? Prompt { get; set; }
        public Reasoning? Reasoning { get; set; } //
        public string? ServiceTier { get; set; } //
        public string Status { get; set; } //
        public bool Store { get; set; } //
        public float? Temperature { get; set; } //
        public TextType Text { get; set; } // 
        public string ToolChoice { get; set; } //
        public List<Tool>? Tools { get; set; } //
        public int? TopLogprobs { get; set; } //
        public float? TopP { get; set; } //
        public string? Truncation { get; set; } //
        public Usage Usage { get; set; } //
        public string? User { get; set; } //
        public ApiError Error { get; set; } //
        public string Warning { get; set; }
    }

    public class ResponseMessage
    {
        public string Id { get; set; }
        public string Status  { get; set; } // "in_progress", "completed" or "incomplete"
        public string Type { get; set; } // "message", "function_call";  other types not supported 
        
        // output message
        public string? Role { get; set; } // Always "assistant"
        
        // Function tool call
        public string? Arguments { get; set; }
        public string? CallId { get; set; }
        public string? Name { get; set; }

        public string? Output { get; set; }
        public List<ToolCallsType>? ToolCalls { get; set; }
    }

    public class OutputResponse : ResponseMessage
    {
        public List<ResponseContent> Content  { get; set; } 
    }

    public class InputResponse : ResponseMessage
    {
        public string Content  { get; set; } 
    }

    public struct ResponseContent
    {
        public string Type { get; set; } // "output_text", "refusal"
        
        // Output text
        public string? Text { get; set; }
        public List<object> Annotations { get; set; }

        // Or refusal
        public string Refusal { get; set; }
    }
    
    #endregion

    #region Audio Transcriptions Data Types

    public struct FileData
    {
        public byte[] Data;
        public string Name;
    }

    public class CreateAudioRequestBase
    {
        public string File { get; set; }
        public FileData FileData { get; set; }
        public string Model { get; set; }
        public string Prompt { get; set; }
        public string ResponseFormat { get; set; } = AudioResponseFormat.Json;
        public float? Temperature { get; set; } = 0;
    }

    public class CreateAudioTranscriptionsRequest : CreateAudioRequestBase
    {
        public string Language { get; set; }
    }

    public class CreateAudioTranslationRequest : CreateAudioRequestBase
    {
    }

    public struct CreateAudioResponse : IResponse
    {
        public ApiError Error { get; set; }
        public string Warning { get; set; }
        public string Text { get; set; }
    }

    #endregion

    #region Images API Data Types

    public class CreateImageRequestBase
    {
        public int? N { get; set; } = 1;
        public string Size { get; set; } = ImageSize.Size1024;
        public string ResponseFormat { get; set; } = ImageResponseFormat.Url;
        public string User { get; set; }
    }

    public sealed class CreateImageRequest : CreateImageRequestBase
    {
        public string Prompt { get; set; }
    }

    public sealed class CreateImageEditRequest : CreateImageRequestBase
    {
        public string Image { get; set; }
        public string Mask { get; set; }
        public string Prompt { get; set; }
    }

    public sealed class CreateImageVariationRequest : CreateImageRequestBase
    {
        public string Image { get; set; }
    }

    public struct CreateImageResponse : IResponse
    {
        public ApiError Error { get; set; }
        public string Warning { get; set; }
        public long Created { get; set; }
        public List<ImageData> Data { get; set; }
    }

    public struct ImageData
    {
        public string Url { get; set; }
        public string B64Json { get; set; }
    }

    #endregion

    #region Embeddins API Data Types

    public struct CreateEmbeddingsRequest
    {
        public string Model { get; set; }
        public string Input { get; set; }
        public string User { get; set; }
    }

    public struct CreateEmbeddingsResponse : IResponse
    {
        public ApiError Error { get; set; }
        public string Warning { get; set; }
        public string Object { get; set; }
        public List<EmbeddingData> Data;
        public string Model { get; set; }
        public Usage Usage { get; set; }
    }

    public struct EmbeddingData
    {
        public string Object { get; set; }
        public List<float> Embedding { get; set; }
        public int Index { get; set; }
    }

    #endregion

    #region Files API Data Types

    public struct ListFilesResponse : IResponse
    {
        public ApiError Error { get; set; }
        public string Warning { get; set; }
        public string Object { get; set; }
        public List<OpenAIFile> Data { get; set; }
        public bool HasMore { get; set; }
    }

    public struct DeleteResponse : IResponse
    {
        public ApiError Error { get; set; }
        public string Warning { get; set; }
        public string Id { get; set; }
        public string Object { get; set; }
        public bool Deleted { get; set; }
    }

    public struct CreateFileRequest
    {
        public string File { get; set; }
        public string Purpose { get; set; }
    }

    #endregion

    #region FineTunes API Data Types

    public class CreateFineTuneRequest
    {
        public string TrainingFile { get; set; }
        public string ValidationFile { get; set; }
        public string Model { get; set; }
        public int NEpochs { get; set; } = 4;
        public int? BatchSize { get; set; } = null;
        public float? LearningRateMultiplier { get; set; } = null;
        public float PromptLossWeight { get; set; } = 0.01f;
        public bool ComputeClassificationMetrics { get; set; } = false;
        public int? ClassificationNClasses { get; set; } = null;
        public string ClassificationPositiveClass { get; set; }
        public List<float> ClassificationBetas { get; set; }
        public string Suffix { get; set; }
    }

    public struct ListFineTunesResponse : IResponse
    {
        public ApiError Error { get; set; }
        public string Warning { get; set; }
        public string Object { get; set; }
        public List<FineTune> Data { get; set; }
        public object NextStartingAfter { get; set; }
    }

    public struct ListFineTuneEventsResponse : IResponse
    {
        public ApiError Error { get; set; }
        public string Warning { get; set; }
        public string Object { get; set; }
        public List<FineTuneEvent> Data { get; set; }
    }

    public class FineTune
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public long CreatedAt { get; set; }
        public long UpdatedAt { get; set; }
        public string Model { get; set; }
        public string FineTunedModel { get; set; }
        public string OrganizationId { get; set; }
        public string Status { get; set; }
        public Dictionary<string, object> Hyperparams { get; set; }
        public List<OpenAIFile> TrainingFiles { get; set; }
        public List<OpenAIFile> ValidationFiles { get; set; }
        public List<OpenAIFile> ResultFiles { get; set; }
        public List<FineTuneEvent> Events { get; set; }
    }

    public class FineTuneResponse : FineTune, IResponse
    {
        public ApiError Error { get; set; }
        public string Warning { get; set; }
    }

    public struct FineTuneEvent
    {
        public string Object { get; set; }
        public long CreatedAt { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
    }

    #endregion

    #region Moderations API Data Types

    public class CreateModerationRequest
    {
        public string Input { get; set; }
        public string Model { get; set; } = ModerationModel.Latest;
    }

    public struct CreateModerationResponse : IResponse
    {
        public ApiError Error { get; set; }
        public string Warning { get; set; }
        public string Id { get; set; }
        public string Model { get; set; }
        public List<ModerationResult> Results { get; set; }
    }

    public struct ModerationResult
    {
        public bool Flagged { get; set; }
        public Dictionary<string, bool> Categories { get; set; }
        public Dictionary<string, float> CategoryScores { get; set; }
    }

    #endregion

    #region Static String Types

    public static class ContentType
    {
        public const string MultipartFormData = "multipart/form-data";
        public const string ApplicationJson = "application/json";
    }

    public static class ImageSize
    {
        public const string Size256 = "256x256";
        public const string Size512 = "512x512";
        public const string Size1024 = "1024x1024";
    }

    public static class ImageResponseFormat
    {
        public const string Url = "url";
        public const string Base64Json = "b64_json";
    }

    public static class AudioResponseFormat
    {
        public const string Json = "json";
        public const string Text = "text";
        public const string Srt = "srt";
        public const string VerboseJson = "verbose_json";
        public const string Vtt = "vtt";
    }

    public static class ModerationModel
    {
        public const string Stable = "text-moderation-stable";
        public const string Latest = "text-moderation-latest";
    }

    #endregion

    #region TTS API Data Types

    public struct CreateTTSRequest
    {
        public string model { get; set; }
        public string input { get; set; }
        public string voice { get; set; }
    }

    #endregion
}