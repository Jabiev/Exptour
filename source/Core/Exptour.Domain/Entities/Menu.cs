using Exptour.Domain.Entities.Common;

namespace Exptour.Domain.Entities;

public class Menu : BaseEntity
{
    public string Name { get; set; }
    public ICollection<Endpoint> Endpoints { get; set; }
}
