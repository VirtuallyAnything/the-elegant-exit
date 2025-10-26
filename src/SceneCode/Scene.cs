using System.Threading.Tasks;
using Godot;

namespace tee
{
    public partial class Scene : Control
    {
        public virtual async Task TransitionIn()
        {
            Tween tween = CreateTween();
            tween.TweenProperty(this, $"{PropertyName.Modulate}", new Color(1, 1, 1, 1), .5);
            await ToSignal(tween, Tween.SignalName.Finished);
        }

        public virtual async Task TransitionOut()
        {
            Tween tween = CreateTween();
            tween.TweenProperty(this, $"{PropertyName.Modulate}", new Color(0, 0, 0, 1), .5);
            await ToSignal(tween, Tween.SignalName.Finished);
        }
    }
}
