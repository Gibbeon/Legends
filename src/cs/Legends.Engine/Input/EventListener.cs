using System;

namespace Legends.Engine.Input;
public class EventListener
{
    public InputCommandSet CommandSet { get; private set;}
    public string Name { get; private set; }
    private readonly Func<EventType, EventArgs, bool> _evaluate;
    public bool Eval(EventType type, EventArgs args) => _evaluate(type, args);

    public EventListener(InputCommandSet commandSet, string actionName, Func<EventType, EventArgs, bool> evaluator)
    {
        CommandSet = commandSet;
        Name = actionName;
        _evaluate = evaluator;
    }
}
