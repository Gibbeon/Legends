namespace SlyEngine.Animation;

public class AnimationMessageCallbackEventArgs : AnimationEventArgs
{
    public string Message { get; private set; }

    public AnimationMessageCallbackEventArgs(IAnimation animation, string message) : base(animation)
    {
        Message = message;
    }
}
