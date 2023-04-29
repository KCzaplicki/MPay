namespace MPay.Infrastructure.Validation;

public record ValidationError(string ErrorCode, Dictionary<string, object> Parameters, string Message);