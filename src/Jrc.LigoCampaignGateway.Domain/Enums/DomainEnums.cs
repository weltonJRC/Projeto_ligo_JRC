namespace Jrc.LigoCampaignGateway.Domain.Enums;

public enum DispatchState
{
    Reserved = 0,
    Sending = 1,
    Accepted = 2,
    Unknown = 3,
    Sent = 4,
    Delivered = 5,
    Read = 6,
    FailedTransient = 7,
    FailedPermanent = 8
}

public enum MediaLeaseState
{
    Active = 0,
    Expired = 1,
    Revoked = 2
}

public enum MediaUploadMode
{
    Multipart = 0,
    Url = 1
}
