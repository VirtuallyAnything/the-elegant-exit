using Godot;
using Godot.Collections;
using System.Text.Json;

namespace tee
{
    public partial class StaticData : Node
    {
        private static Array<EnemyAttack> _KFGLines = new();
        private string _dataFilePathKFG = "res://Data/KFG_Quotes.json";
        private static Array<EnemyAttack> _cryptoBroLines = new();
        private string _dataFilePathCryptoBro = "res://Data/CryptoBro_Quotes.json";
        private static Array<EnemyAttack> _influencerLines = new();
        private string _dataFilePathInfluencer = "res://Data/Influencer_Quotes.json";
        private static Array<EnemyAttack> _neighborLines = new();
        private string _dataFilePathNeighbor = "res://Data/Neighbor_Quotes.json";
        private static Array<EnemyAttack> _academicLines = new();
        private string _dataFilePathAcademic = "res://Data/Academic_Quotes.json";
        private static Array<EnemyAttack> _cougarLines = new();
        private string _dataFilePathCougar = "res://Data/Cougar_Quotes.json";


        public override void _Ready()
        {
            _KFGLines = LoadJsonFile(_dataFilePathKFG);
            _cryptoBroLines = LoadJsonFile(_dataFilePathCryptoBro);
            _influencerLines = LoadJsonFile(_dataFilePathInfluencer);
            _neighborLines = LoadJsonFile(_dataFilePathNeighbor);
            _academicLines = LoadJsonFile(_dataFilePathAcademic);
            _cougarLines = LoadJsonFile(_dataFilePathCougar);
        }

        public static Array<EnemyAttack> GetLines(string forCharacter){
            switch(forCharacter){
                case "John Parker":
                return _cryptoBroLines;
                case "Babi":
                return _influencerLines;
                case "Neighbor Dave":
                return _neighborLines;
                
            }
            return null;
        }

        public Array<EnemyAttack> LoadJsonFile(string filePath)
        {
            if (FileAccess.FileExists(filePath))
            {
                var dataFile = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
                try
                {
                    //var parsedResult = Json.ParseString(dataFile.GetAsText());
                    Array<EnemyAttack> attacks = JsonSerializer.Deserialize<Array<EnemyAttack>>(dataFile.GetAsText())!;
                    //var dataReceived = parsedResult.AsGodotArray();
                    return attacks;
                }
                catch (System.Exception e)
                {
                    GD.Print(e.Message);
                }
                dataFile.Close();
            }
            return null;
        }
    }
}
