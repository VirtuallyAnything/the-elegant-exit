using Godot;
using System;

namespace tee
{
        public partial class PartyFloorScene : Scene
        {
                private Veil _veil;
                public override void _Ready()
                {
                        _veil = this.GetFirstChildOfType<Veil>();
                        _veil.SetupOccluders();
                }

        }
}
