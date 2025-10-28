using System.Threading.Tasks;
using Godot;

namespace tee
{
	public delegate Task TempSceneHandler(SceneName sceneName);
	public partial class TempScene : Scene
	{
		public static TempSceneHandler SceneTimerFinished;
		private float _timerSeconds;
		private SceneName _requestedScene;
		private Timer _timer;
		[Export]
		public float TimerSeconds
		{
			get { return _timerSeconds; }
			set { _timerSeconds = value; }
		}
		[Export]
		public SceneName RequestedScene
        {
			get { return _requestedScene; }
			set{ _requestedScene = value; }
        }
		
		public override void _EnterTree()
		{
			_timer = new();
			AddChild(_timer);
			_timer.Start(TimerSeconds);
			_timer.Timeout += RequestSceneChange;
		}

		private void RequestSceneChange()
		{
            if (!SceneManager.IsSceneChanging)
            {
               SceneTimerFinished?.Invoke(_requestedScene); 
            }
		}
		
		public override void _ExitTree() {
			_timer.Stop();
			_timer.Timeout -= RequestSceneChange;
			base._ExitTree();
		}
	}
}
