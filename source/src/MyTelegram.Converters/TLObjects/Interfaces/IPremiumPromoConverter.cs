using MyTelegram.Schema.Help;

namespace MyTelegram.Converters.TLObjects.Interfaces;

public interface IPremiumPromoConverter : ILayeredConverter
{
    IPremiumPromo ToPremiumPromo();
}