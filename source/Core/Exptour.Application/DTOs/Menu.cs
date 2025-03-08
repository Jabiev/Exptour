namespace Exptour.Application.DTOs;

public class Menu
{
    public string Name { get; set; }
    public List<Action> Actions { get; set; } = new();
}

public class Action
{
    public string ActionType { get; set; }
    public string HttpType { get; set; }
    public string Definition { get; set; }
    public string Code { get; set; }
}
