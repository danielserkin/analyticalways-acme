using System.ComponentModel.DataAnnotations;
namespace Domain.Entities;

public abstract class BaseEntity<TKey> 
{
    public TKey Id { get; protected set; } = default!;

    public DateTime CreadoEn { get; protected set; }

    public DateTime? ActualizadoEn { get; protected set; }

    protected BaseEntity()
    {
        CreadoEn = DateTime.UtcNow;
    }

    protected void Update() => ActualizadoEn = DateTime.UtcNow;
}