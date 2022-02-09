namespace AniGate.Core.DataAccess
{
    public static class JsonDataAccess
    {
        public static string SaveFilePath = "animes.json";

        //public static List<Anime> GetAnimes()
        //{
        //    if (!File.Exists(SaveFilePath))
        //        SaveAnimes(new List<Anime>());

        //    var json = File.ReadAllText(SaveFilePath);
        //    var animes = JsonSerializer.Deserialize<List<Anime>>(json)!;
        //    foreach (var anime in animes)
        //    {
        //        foreach (var episode in anime.Episodes)
        //        {
        //            episode.Anime = anime;
        //        }
        //    }

        //    return animes;
        //}

        //public static void SaveAnimes(List<Anime> animes)
        //{
        //    var json = JsonSerializer.Serialize(animes);
        //    File.WriteAllText(SaveFilePath, json);
        //}
    }
}
