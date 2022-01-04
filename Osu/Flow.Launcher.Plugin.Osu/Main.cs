using Flow.Launcher.Plugin;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flow.Launcher.Plugin.Osu.Model;
using System.Web;
using System.Net.Http;
using Newtonsoft.Json;

namespace Flow.Launcher.Plugin.Osu
{
    /// <summary>
    /// 
    /// </summary>
    public class Osu : IAsyncPlugin
    {
        private PluginInitContext _context;

        private string _requestUrl = @"https://osu.ppy.sh/home/search?q={0}&m=0";
        private HttpClient _client = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InitAsync(PluginInitContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<List<Result>> QueryAsync(Query query, CancellationToken token)
        {
            switch(query.SecondSearch)
            {
                case "user":
                    return new List<Result>
                    {
                        new Result()
                        {
                            IcoPath = await OsuUtils.GetProfileImageFromNameAsync(query.SecondSearch, token),
                            ContextData = query.SecondSearch,
                            Title = query.SecondSearch,
                        }
                    };
            }

            return new List<Result>();
        }

        public async Task<BeatmapsetsCollection> QueryBeatmapsAsync(string search, CancellationToken token) => JsonConvert.DeserializeObject<BeatmapsetsCollection>(await _client.GetStringAsync(string.Format(_requestUrl, HttpUtility.UrlEncode(search)), token));
    }
}