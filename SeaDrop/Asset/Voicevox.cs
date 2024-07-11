using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SeaDrop
{
    public class Voicevox
    {

        private readonly string ipPort;
        public string DownloadPath { set; get; }

        public Voicevox(string _ipAdress = "localhost", string _port = "50021")
        {
            ipPort = "http://" + _ipAdress + ":" + _port;
            DownloadPath = Directory.GetCurrentDirectory();
        }

        public static void Play(string text, int speaker = 1, string title = "voicevoxtext", bool upspeak = true)
        {
            if (text == "") return;
            Voicevox voicevox = new Voicevox();
            Sound sound = voicevox.Make(title, text, speaker, upspeak);
            sound.PlayWait();
            if (sound.Length > 0) File.Delete($"{title}.wav");
            sound.Dispose();
        }
        public Sound Make(string title, string text, int speaker = 1, bool upspeak = true)
        {
            MakeSound(title, text, speaker, upspeak).ContinueWith(_ => {; });
            string fileName = title + ".wav";
            while (true)
            {
                try
                {
                    if (File.Exists(fileName))
                    {
                        Sound sound = new Sound(fileName);
                        if (sound.Enable)
                        {
                            if (sound.Length > -1 && sound.Frequency > 1)
                                return sound;
                        }

                    }
                }
                catch { }
            }
        }

        private async Task<string> MakeQuery(string _text, int _speakerId)
        {
            string jsonQuery;
            using (var httpClient = new HttpClient())
            {
                string url = ipPort + "/audio_query?text=" + _text + "&speaker=" + _speakerId;
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), url))
                {
                    request.Headers.TryAddWithoutValidation("accept", "application/json");

                    request.Content = new StringContent("");
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                    var response = await httpClient.SendAsync(request);
                    jsonQuery = await response.Content.ReadAsStringAsync();
                }
            }
            return jsonQuery;
        }

        private async Task MakeSound(string _title, string _text, int _speakerId, bool _upspeak)
        {
            if (_text == "") return;
            using (var httpClient = new HttpClient())
            {
                string url = ipPort + "/synthesis?speaker=" + _speakerId + "&enable_interrogative_upspeak=" + _upspeak;
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), url))
                {
                    request.Headers.TryAddWithoutValidation("accept", "audio/wav");

                    request.Content = new StringContent(await MakeQuery(_text, _speakerId));

                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                    var response = await httpClient.SendAsync(request);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        // Make wav file.
                        string fileName = _title + ".wav";
                        using (var fileStream = File.Create(DownloadPath + "/" + fileName))
                        {
                            using (var httpStream = await response.Content.ReadAsStreamAsync())
                            {
                                httpStream.CopyTo(fileStream);
                                fileStream.Flush();
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error");
                        throw new Exception();
                    }
                }
            }
        }

        public async Task<List<SpeakerModel>> Speakers()
        {
            string jsonQuery;
            using (var httpClient = new HttpClient())
            {
                string url = ipPort + "/speakers";
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), url))
                {
                    request.Headers.TryAddWithoutValidation("accept", "application/json");

                    request.Content = new StringContent("");
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                    var response = await httpClient.SendAsync(request);
                    jsonQuery = await response.Content.ReadAsStringAsync();
                }
            }
            var list = JsonSerializer.Deserialize<List<SpeakerModel>>(jsonQuery);
            return list;
        }
    }

    public class SpeakerModel
    {
        public Supported_Features? supported_features { get; set; }
        public string? name { get; set; }
        public string? speaker_uuid { get; set; }
        public Style[]? styles { get; set; }
        public string? version { get; set; }
    }

    public class Supported_Features
    {
        public string? permitted_synthesis_morphing { get; set; }
    }

    public class Style
    {
        public string? name { get; set; }
        public int id { get; set; }
    }
}
