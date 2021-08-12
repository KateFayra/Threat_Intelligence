namespace ServiceInfoGatherer
{
    public class Workers
    {
        public static int numWorkers = 2;
        public static string[] workerURLs = { "http://127.0.0.1:10001", "http://127.0.0.1:10002" }; // TODO: Read from config
    }
}