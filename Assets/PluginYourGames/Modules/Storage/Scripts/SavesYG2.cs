namespace YG
{
    [System.Serializable]
    public partial class SavesYG
    {
        public int idSave;
        public GuessWordGame.GameData gameData = new GuessWordGame.GameData();
    }
}