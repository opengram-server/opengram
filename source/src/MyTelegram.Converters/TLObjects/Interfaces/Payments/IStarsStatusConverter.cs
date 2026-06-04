// ReSharper disable All

using MyTelegram.Schema.Payments;
using MyTelegram.Domain.Shared.Stars;

namespace MyTelegram.Converters.Responses.Interfaces.Payments;

public partial interface IStarsStatusConverter : ILayeredConverter
{
    IStarsStatus ToStarsStatus(StarsStatus starsStatus);
}
