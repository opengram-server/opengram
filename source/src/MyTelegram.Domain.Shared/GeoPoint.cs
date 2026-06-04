namespace MyTelegram;

/// <summary>
/// Defines a GeoPoint by its coordinates.
/// </summary>
/// <param name="Lat">Latitude</param>
/// <param name="Long">Longitude</param>
/// <param name="AccuracyRadius">the estimated horizontal accuracy of the location, in meters; as defined by the sender</param>
public record GeoPoint(double Lat, double Long, int? AccuracyRadius);