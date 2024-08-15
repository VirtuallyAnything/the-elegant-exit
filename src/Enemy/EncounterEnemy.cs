using System.Linq;
using Godot;
using Godot.Collections;

namespace tee
{
	public partial class EncounterEnemy : Node
	{
		private string _displayName;
		private Array<EnemyAttack> _enemyAttacks;
		private int _conversationInterest;
		private Array<EnemyInterest> _likes;
		private Array<EnemyInterest> _neutrals;
		private Array<EnemyInterest> _dislikes;
		public int ConversationInterest{
			get;
		}
		public Array<EnemyInterest> Likes{
			get{return _likes;}
			init{_likes = value;}
		}
		public Array<EnemyInterest> Neutrals{
			get{return _neutrals;}
			init{_neutrals = value;}
		}
		public Array<EnemyInterest> Dislikes{
			get{return _dislikes;}
			init{_dislikes = value;}
		}

		public EncounterEnemy(EnemyData data){
			_displayName = data.DisplayName;
			_enemyAttacks = data.EnemyAttacks;
			_conversationInterest = data.ConversationInterest;

			Likes = data.Likes.ToConversationTopics();
			Neutrals = data.Neutrals.ToConversationTopics();
			Dislikes = data.Dislikes.ToConversationTopics();
		}
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

		public EnemyInterest ChooseTopic(){
			Array<EnemyInterest> allTopics = _likes + _neutrals + _dislikes;
			return allTopics.Random<EnemyInterest>();
		}

		public EnemyAttack ChooseAttack(){
			TopicName topic = ChooseTopic().Name;
			Array<EnemyAttack> potentialAttacks = new();
			foreach(EnemyAttack attack in _enemyAttacks){
				if(attack.Topic == topic){
					potentialAttacks.Add(attack);
				}
			}
			return potentialAttacks.PickRandom();
		}

		public bool HasLike(TopicName topic){
			return Likes.Any(enemyInterest => enemyInterest.Name == topic);
		}
	}
}
