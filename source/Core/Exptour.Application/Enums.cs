using System.ComponentModel;

namespace Exptour.Application;

public enum ActionType
{
    [Description("Reading")] Reading,
    [Description("Writing")] Writing,
    [Description("Updating")] Updating,
    [Description("Deleting")] Deleting
}
