using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flow.Launcher.Plugin.Osu
{
    public static class OsuUtils
    {
        private static HttpClient _client = new(new HttpClientHandler()
        {
            AllowAutoRedirect = false
        });

        public static async Task<int> GetUserIdFromNameAsync(string name, CancellationToken token) => int.Parse((await _client.GetAsync(@$"https://osu.ppy.sh/users/{name}", token)).Headers.Location!.AbsolutePath.Split('/')[^1]);
        public static async Task<string> GetProfileImageFromNameAsync(string name, CancellationToken token) => @$"http://s.ppy.sh/a/{(await GetUserIdFromNameAsync(name, token))}";
    }
}
