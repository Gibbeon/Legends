namespace Legends.Engine.Input;

public interface IInputHandlerService
{
    InputManager? Current { get; }
    void Push(InputManager manager);
    void Remove(InputManager manager);
}
