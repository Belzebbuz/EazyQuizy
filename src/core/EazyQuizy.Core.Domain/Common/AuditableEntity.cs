namespace EazyQuizy.Core.Domain.Common;

public abstract class AuditableEntity<T> : BaseEntity<T>, IAuditableEntity where T: notnull
{
	public Guid CreatedBy { get; set; }
	public DateTimeOffset CreatedOn { get; private set; } = DateTimeOffset.UtcNow;
	public DateTimeOffset? LastModifiedOn { get; set; } = DateTimeOffset.UtcNow;
	public Guid LastModifiedBy { get; set; }
	public DateTimeOffset? DeletedOn { get; set; }
	public Guid? DeletedBy { get; set; }

	public void MarkAsDeleted(Guid userId)
	{
		DeletedOn = DateTimeOffset.UtcNow;
		DeletedBy = userId;
	}
}