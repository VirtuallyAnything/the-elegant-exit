using Godot;

namespace tee
{
    public delegate void OccluderHandler(DynamicLightOccluder2D occluder);
    [GlobalClass]
    public partial class DynamicLightOccluder2D : LightOccluder2D
    {
        public static event OccluderHandler PositionChanged;
        public static event OccluderHandler RotationChanged;

        public void OnRotationChanged()
        {
            RotationChanged(this);
        }

        public void OnPositionChanged()
        {
            PositionChanged(this);
        }
    }
}