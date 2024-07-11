namespace SeaDrop
{
    public class Internet
    {
        public static HttpClient Client = new HttpClient();

        public static void Dispose()
        {
            Client.Dispose();
        }
        public static (string State, Task<string>? Data) Connect(string url)
        {
            Dispose();
            Client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };

            try
            {
                var response = Client.SendAsync(request);
                var result = response.Result;
                return ($"{(int)result.StatusCode} {result.StatusCode}", result.Content.ReadAsStringAsync());
            }
            catch (Exception)
            {
                return ("Not Connected", null);
            }
        }

        public static void Upload(string path, string url)
        {

        }
    }

    public class User
    {
        public string Name = "";
        public string Password = "";
    }
}
