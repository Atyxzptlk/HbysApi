using System;

namespace HbysApi.Domain.Entities;

public interface IEntity
{
    Guid Id { get; set; }
}
