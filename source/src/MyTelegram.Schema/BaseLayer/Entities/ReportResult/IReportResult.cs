// ReSharper disable All

namespace MyTelegram.Schema;

///<summary>
/// Represents a report menu or result
/// See <a href="https://corefork.telegram.org/constructor/ReportResult" />
///</summary>
[JsonDerivedType(typeof(TReportResultChooseOption), nameof(TReportResultChooseOption))]
[JsonDerivedType(typeof(TReportResultAddComment), nameof(TReportResultAddComment))]
[JsonDerivedType(typeof(TReportResultReported), nameof(TReportResultReported))]
public interface IReportResult : IObject
{

}
