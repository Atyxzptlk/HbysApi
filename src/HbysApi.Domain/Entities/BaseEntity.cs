using System;

namespace HbysApi.Domain.Entities;

public abstract class BaseEntity : IEntity
{
    public Guid Id { get; set; }
}
