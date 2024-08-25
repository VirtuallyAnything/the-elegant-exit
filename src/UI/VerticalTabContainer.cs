using System.Collections;
using System.Linq;
using Godot;
using Godot.Collections;
namespace customUI
{
	[GlobalClass, Tool]
	public partial class VerticalTabContainer : Container
	{
		private bool _setupComplete;
		private ButtonGroup _buttonGroup = new();
		private Array<Control> _nodesToDisplay = new();
		private Array<Button> _tabButtons = new();
		private Control _currentTab;
		private HBoxContainer _hBox = new();
		private VBoxContainer _vBox = new();
		private Panel _panel = new();
		public Panel Panel{
			get{return _panel;}
		}
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{	
			_hBox.Size = Size;
			_hBox.AddChild(_vBox);
			_hBox.AddChild(_panel);
			AddChild(_hBox, false, InternalMode.Back);
			_setupComplete = true;
		}

		public override void _Notification(int what)
		{
			if(!_setupComplete){
				return;
			}
			if(what == NotificationResized){
				_hBox.Size = Size;
			}
			if (what == NotificationChildOrderChanged)
			{				
				foreach (Control child in GetChildren())
				{
					int index = child.GetIndex();
					//When child is already present but got moved
					if(_nodesToDisplay.Contains(child)){
						int indexInArray = _nodesToDisplay.IndexOf(child);
						Button buttonAtIndex = _tabButtons[indexInArray];
						_vBox.MoveChild(buttonAtIndex, index);
						continue;
					}
					//If this child is the first one added
					if(_currentTab == null){
						_currentTab = child;
					}
					//Make a button for the newly added child
					Button tabButton = new()
					{
						ToggleMode = true,
						ThemeTypeVariation = "",
						Text = child.Name,
						ButtonGroup = _buttonGroup
					};
					
					_nodesToDisplay.Add(child);
					_tabButtons.Add(tabButton);
					_vBox.AddChild(tabButton);
					tabButton.Pressed += OnTabButtonPressed;

					if(!child.Equals(_currentTab)){
						child.Visible = false;
					}
				}
				//Any node that is in the _nodesToDisplay but isn't a child must have been removed. 
				//Remove from Arrays and get rid of its corresponding button.
				IEnumerable items = _nodesToDisplay.Except(GetChildren());
				foreach(Control item in items){
					int indexInArray = _nodesToDisplay.IndexOf(item);
					_nodesToDisplay.Remove(item);
					_vBox.RemoveChild(_tabButtons[indexInArray]);
					_tabButtons.RemoveAt(indexInArray);
				}
			}
		}

		private void OnTabButtonPressed()
		{
			_currentTab.Visible = false;
			Button button = (Button)_buttonGroup.GetPressedButton();
			_currentTab = _nodesToDisplay[_tabButtons.IndexOf(button)];
			_currentTab.Visible = true;
		}

	}
}
