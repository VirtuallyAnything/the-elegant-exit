using Godot;
using System;

namespace tee
{
    public partial class ItemData : Resource
    {
        private string _itemDisplayName;
        private Texture2D _texture;
        private PlayerAttack _attackFromItem;

        [Export] public string ItemDisplayName{
            get{return _itemDisplayName;}
            set{_itemDisplayName = value;}
        }
        [Export] public Texture2D Texture{
            get{return _texture;}
            set{_texture = value;}
        }
        [Export] public PlayerAttack AttackFromItem{
            get{return _attackFromItem;}
            set{_attackFromItem = value;}
        }
    }
}
