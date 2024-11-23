using EazyQuizy.Core.Domain.Entities;
using MongoDB.Bson.Serialization;

namespace EazyQuizy.Core.Silo.MongoConfig;

public class MongoDbClassMap
{
	public static void Initialize()
	{
		
		BsonClassMap.RegisterClassMap<Quiz>(cm =>
		{
			cm.AutoMap();
			cm.SetIgnoreExtraElements(true);
			cm.MapProperty(x => x.Name).SetElementName("name");
			cm.MapField("_questions").SetElementName("questions");
		});
		BsonClassMap.RegisterClassMap<Question>(cm =>
		{
			cm.AutoMap();
			cm.SetIgnoreExtraElements(true);
			cm.MapProperty(x => x.Text).SetElementName("text");
			cm.MapProperty(x => x.Order).SetElementName("order");
			cm.MapProperty(x => x.ImageUrl).SetElementName("image_url");
			cm.AddKnownType(typeof(SingleAnswersQuestion));
			cm.AddKnownType(typeof(MultipleAnswersQuestion));
			cm.MapIdProperty(x => x.Id).SetElementName("id");
		});
		BsonClassMap.RegisterClassMap<SingleAnswersQuestion>(cm =>
		{
			cm.AutoMap();
			cm.SetIgnoreExtraElements(true);
			cm.MapProperty(x => x.CorrectAnswer).SetElementName("correct_answer");
			cm.MapField("_wrongAnswers").SetElementName("_wrongAnswers");
		});
		BsonClassMap.RegisterClassMap<MultipleAnswersQuestion>(cm =>
		{
			cm.AutoMap();
			cm.SetIgnoreExtraElements(true);
			cm.MapField("_wrongAnswers").SetElementName("_wrongAnswers");
			cm.MapField("_correctAnswers").SetElementName("_correctAnswers");
		});
	}
}