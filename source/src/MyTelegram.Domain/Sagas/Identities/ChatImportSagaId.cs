using EventFlow.Sagas;
using EventFlow.ValueObjects;
using System.Text.Json.Serialization;
using MyTelegram.EventFlow;

namespace MyTelegram.Domain.Sagas.Identities;

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<ChatImportSagaId>))]
public class ChatImportSagaId(string value) : SingleValueObject<string>(value), ISagaId;
