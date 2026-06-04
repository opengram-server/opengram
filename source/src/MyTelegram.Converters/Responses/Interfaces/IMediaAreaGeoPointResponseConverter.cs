// ReSharper disable All

namespace MyTelegram.Converters.Responses;

public partial interface IMediaAreaGeoPointResponseConverter
    : IResponseConverter<
        TMediaAreaGeoPoint,
        IMediaArea
    >
{
}