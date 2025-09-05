using Godot;

namespace tee
{
    public delegate void DynamicOccluderHandler(DynamicLightOccluder2D occluder);
    [GlobalClass]
    public partial class DynamicLightOccluder2D : LightOccluder2D
    {
        public static event DynamicOccluderHandler TransformChanged;

        public override void _Ready()
        {
            base._Ready();
            GD.Print("Instance ID:" + GetInstanceId() + "Parent: " + GetParent());
        }

        public override void _Notification(int what)
        {
            if (what == NotificationTransformChanged)
            {
                TransformChanged?.Invoke(this);
            }
        }
    }
}