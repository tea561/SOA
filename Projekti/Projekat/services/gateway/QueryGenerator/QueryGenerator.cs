namespace gateway.QueryGenerator
{
    public class QueryGenerator
    {
        private string[] neutralParameters = { "radio"};
        private string[] calmMusic = { "calm", "quiet playlist", "serenenity", "calmness", "calm playlist", "gentle playlist", "tranquil", "calm your soul", "chill", "chill playlist", "soft", "soft songs"};
        private string[] classicalMusic = { "classical", "classical playlist", "piano", "piano playlist", "the best of bach", "the best of mozart", "beethoven", "tchaikovsky", "arias" };
        private string[] relaxingMusic = { "relax", "relaxing music", "peaceful", "peaceful music", "relaxing sunday morning", "relaxing jazz", "lofi beats"};
        private string[] upBeatMusic = { "upbeat", "upbeat music", "upbeat playlist", "feel good music", "upbeat work music", "dance playlist"};


        public static string getQueryParameter(string type)
        {
            string[] neutralParameters = { "radio"};
            string[] calmMusic = { "calm", "quiet playlist", "serenenity", "calmness", "calm playlist", "gentle playlist", "tranquil", "calm your soul", "chill", "chill playlist", "soft", "soft songs"};
            string[] classicalMusic = { "classical", "classical playlist", "piano", "piano playlist", "the best of bach", "the best of mozart", "beethoven", "tchaikovsky", "arias" };
            string[] relaxingMusic = { "relax", "relaxing music", "peaceful", "peaceful music", "relaxing sunday morning", "relaxing jazz", "lofi beats"};
            string[] upBeatMusic = { "upbeat", "upbeat music", "upbeat playlist", "feel good music", "upbeat work music", "dance playlist"};

            int randomNumber;
            Random random = new Random();

            switch(type)
            {
                case "neutral":
                    randomNumber = random.Next(0, neutralParameters.Length);
                    return neutralParameters[randomNumber];
                case "relaxing":
                    randomNumber = random.Next(0, relaxingMusic.Length);
                    return relaxingMusic[randomNumber];
                case "classical":
                    randomNumber = random.Next(0, classicalMusic.Length);
                    return classicalMusic[randomNumber];
                case "calm":
                    randomNumber = random.Next(0, calmMusic.Length);
                    return calmMusic[randomNumber];
                case "upbeat":
                    randomNumber = random.Next(0, upBeatMusic.Length);
                    return upBeatMusic[randomNumber]; 
                default:
                    return "radio";
            }
  
        }


    }
}