namespace Jrc.LigoCampaignGateway.Application.Models;

public record OutboundHsmRequest(
    string Source,
    string Campaign,
    string CampaignRunId,
    string RecordId,
    string NumberChip,
    string Destination,
    string TemplateId,
    IReadOnlyList<string> BodyParameters
);

public record OutboundHsmResponse(
    bool Ok,
    string CorrelationId,
    string Status,
    string? ProviderMessageId,
    string? Error,
    MediaLeaseInfoDto? Media
);

public record MediaLeaseInfoDto(
    string IdMedia,
    string ValidUntil
);

public record PrepareMediaRequest(
    Guid TemplateId,
    Guid MediaAssetId,
    string Mode = "Multipart"
);

public record PrepareMediaResponse(
    bool Success,
    string IdMedia,
    string ValidUntilRaw,
    DateTime? ValidUntilParsed,
    string ErrorMessage
);

public record LigoStatusWebhookPayload(
    string Status,
    string MessageId,
    string? Telephone,
    string? ErrorMessage,
    long Timestamp
);

public record LigoHsmMessage(
    string CorrelationId,
    string NumberChip,
    string Telephone,
    string Template,
    string IdMedia,
    IReadOnlyList<string> BodyParameters,
    string? StatusCallbackUrl,
    string? ResponseCallbackUrl
);

public record LigoSendResult(
    bool Success,
    string Status,
    string? ProviderMessageId,
    string? ErrorCode,
    string? ErrorMessage,
    string RawResponse
);

public record LigoAuthResult(
    string Token,
    DateTime Expiration,
    string RawResponse
);

public record LigoMediaUploadResult(
    string IdMedia,
    string ValidUntilRaw,
    DateTime? ValidUntilParsed,
    bool ParseSucceeded,
    string RawResponse
);
