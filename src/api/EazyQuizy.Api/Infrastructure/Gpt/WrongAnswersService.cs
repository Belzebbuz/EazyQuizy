using System.Text.Json;
using EazyQuizy.Common.Grpc.Quiz;
using ErrorOr;

namespace EazyQuizy.Api.Infrastructure.Gpt;

public interface IWrongAnswersService
{
	public Task<ErrorOr<IReadOnlyCollection<string>>> GenerateAsync(GenerateWrongAnswersRequest request);
}
public class WrongAnswersService(IGptService service, ILogger<WrongAnswersService> logger) : IWrongAnswersService
{
	private const string SystemText= "Ты создаёшь приложение для создания квизов. " +
	                              "Нужно, чтобы ты предложил указанное кол-во НЕПРАВИЛЬНЫХ ВАРИАНТОВ ОТВЕТА к вопросу квиза, " +
	                              "но близких к правильному, чтобы запутать игроков, " +
	                              "но при этом они могли отличить их от правильного. " +
	                              "Формат ответа должен быть ввиде JSON { \"result\" : [\"вариант 1\", и т.д] }";

	private const string Prompt = "Вопрос квиза: '{0}', {1} НЕПРАВИЛЬНЫХ вариантов ответа";
	private record Response(string[] Result);
	public async Task<ErrorOr<IReadOnlyCollection<string>>> GenerateAsync(GenerateWrongAnswersRequest request)
	{
		try
		{
			var safeText = request.Text.Replace("\"", "\\\"");
			var prompt = string.Format(Prompt,safeText, request.Count);
			var response = await service.SendMessageAsync(prompt, SystemText);
			if (response.IsError)
				return response.FirstError;
			var clearText = response.Value.Replace("`", string.Empty).Replace("json", string.Empty);
			var result = JsonSerializer.Deserialize<Response>(clearText, new JsonSerializerOptions()
			{
				PropertyNameCaseInsensitive = true
			});
			if (result is null)
				return Error.Failure(description:"Нейросеть предоставила невалидный ответ. Пожалуйста попробуйте еще раз");
			return result.Result;
		}
		catch (Exception e)
		{
			logger.LogError(e.Message,e);
			return Error.Failure(description:"Нейросеть предоставила невалидный ответ. Пожалуйста попробуйте еще раз");
		}
	}
}