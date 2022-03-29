using Flow.Launcher.Plugin.Osu.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Flow.Launcher.Plugin.Osu
{
	public class OsuApi
	{
		private static string _baseUrl = "https://osu.ppy.sh/p/api";
		private string _apiKey = string.Empty;
		private HttpClientHandler _clientHandler;
		private CachingComponent _cache;
		private IPublicAPI _pluginApi;
		private HttpClient _client ;

		public OsuApi(string apiKey, PluginInitContext context)
		{
			_clientHandler = new();
			_client = new(_clientHandler);
			_apiKey = apiKey;
			_cache = new(context.CurrentPluginMetadata.PluginDirectory);
			_pluginApi = context.API;
		}
		public CookieCollection Cookies
		{
			set {
				foreach(var cookie in value)
					_clientHandler.CookieContainer.Add((cookie as Cookie)!);
			}
		}
		public async Task<User> GetUserdataAsync(string username, CancellationToken token) =>
			JsonConvert.DeserializeObject<User>(await _client.GetStringAsync(_($"{_baseUrl}/get_user?u={username}"), token))!;
		public async Task<List<Beatmapset>> SearchBeatmapsetsAsync(string query, CancellationToken token) =>
			JsonConvert.DeserializeObject<BeatmapsetsCollection>(
				await _client.GetStringAsync(@$"http://osu.ppy.sh/beatmapsets/search?q={HttpUtility.UrlEncode(query)}&m=0", token)
			)!.Beatmapsets;
		public async Task<string> GetAvatarAsync(string name, CancellationToken token) =>
			await GetAvatarAsync(await GetUserdataAsync(name, token), token);
		public async Task<string> GetAvatarAsync(User user, CancellationToken token) =>
			await _cache.TryGetOrSaveAsync($"{user.Id}.png", async (CancellationToken token) => 
				await GetStreamAsync(user.ProfileImageUrl, token),
				token);
		public async Task<string> GetBeatmapThumbnailAsync(Beatmapset bms, CancellationToken token) =>
			await _cache.TryGetOrSaveAsync($"{bms.Id}l.jpg", async (CancellationToken token) => 
				await GetStreamAsync($"https://b.ppy.sh/thumb/{bms.Id}l.jpg", token),
				token);
		private async Task<Stream> GetStreamAsync(string url, CancellationToken token) =>
			await (await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token)).Content.ReadAsStreamAsync(token);
		private string _(string url) => $"{url}&k={_apiKey}";
	}
}
