using EazyQuizy.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EazyQuizy.Core.Infrastructure.Database.EntityConfigs;

public class QuestionEntityConfig : IEntityTypeConfiguration<Question>
{
	public void Configure(EntityTypeBuilder<Question> builder)
	{
		builder.UseTpcMappingStrategy();
		builder.Property(x => x.Id).ValueGeneratedNever();
	}
}

public class SingleAnswerQuestionEntityConfig : IEntityTypeConfiguration<SingleAnswersQuestion>
{
	public void Configure(EntityTypeBuilder<SingleAnswersQuestion> builder)
	{
		builder.UseTpcMappingStrategy();
		builder.Property(x => x.Id).ValueGeneratedNever();
		builder.PrimitiveCollection(x => x.WrongAnswers)
			.HasField("_wrongAnswers");
	}
}

public class MultipleAnswersQuestionEntityConfig : IEntityTypeConfiguration<MultipleAnswersQuestion>
{
	public void Configure(EntityTypeBuilder<MultipleAnswersQuestion> builder)
	{
		builder.UseTpcMappingStrategy();
		builder.Property(x => x.Id).ValueGeneratedNever();
		builder.PrimitiveCollection(x => x.CorrectAnswers)
			.HasField("_correctAnswers")
			.HasColumnType("text[]");
		builder.PrimitiveCollection(x => x.WrongAnswers)
			.HasField("_wrongAnswers");
	}
}