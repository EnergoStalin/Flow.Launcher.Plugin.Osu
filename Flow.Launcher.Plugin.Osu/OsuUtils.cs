using Flow.Launcher.Plugin.Osu.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Flow.Launcher.Plugin.Osu
{
	public static class OsuUtils
	{
		private static HttpClient _client = new(new HttpClientHandler()
		{
			AllowAutoRedirect = false
		});

		public static async Task<Userdata> GetUserdataAsync(string username, CancellationToken token)
		{
			try
			{
				if(int.TryParse((await _client.GetAsync(@$"https://osu.ppy.sh/users/{username}", token)).Headers.Location!.AbsolutePath.Split('/')[^1], out int res))
					return new Userdata(username, res);
			} catch { }
			return null;
		}

		public static async Task<List<Beatmapset>> SearchBeatmapsetsAsync(string query, CancellationToken token) => JsonConvert.DeserializeObject<BeatmapsetsCollection>(await _client.GetStringAsync(@$"http://osu.ppy.sh/beatmapsets/search?q={HttpUtility.UrlEncode(query)}&m=0", token)).Beatmapsets;
	}
}
