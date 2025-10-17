using Godot;

namespace tee
{
	public partial class TextSetter : Label
	{
		public void ValueChanged(float value)
		{
			Text = $"{value}%";
		}
	}
}