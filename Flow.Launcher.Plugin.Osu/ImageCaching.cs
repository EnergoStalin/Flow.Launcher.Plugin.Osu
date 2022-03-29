using Flow.Launcher.Plugin.Osu.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flow.Launcher.Plugin.Osu
{
	public class ImageCaching
	{
		private PluginInitContext _pluginInitContext;
		public string CacheDirectory => Path.Combine(_pluginInitContext.CurrentPluginMetadata.PluginDirectory, "ImageCache");
		public ImageCaching(PluginInitContext ctx)
		{
			_pluginInitContext = ctx;
			Directory.CreateDirectory(CacheDirectory);
		}

		private void ClearCache()
		{
			 foreach(var fn in Directory.GetFiles(CacheDirectory))
				File.Delete(fn);
		}

		private string? TryGetFromCache(string fn)
		{
			var info = new FileInfo(Path.Combine(CacheDirectory, fn));
			if (info.Exists) return info.FullName;
			else return null;
		}
		
		public async Task<string> GetBeatmapThumbnailAsync(Beatmapset bms, CancellationToken token)
		{
			var fn = $"{bms.Id}l.jpg";
			var thumb = TryGetFromCache(fn);
			if (thumb != null) return thumb;

			thumb = Path.Combine(CacheDirectory, fn);
			await _pluginInitContext.API.HttpDownloadAsync($"https://b.ppy.sh/thumb/{bms.Id}l.jpg", thumb);

			return thumb;
		}

		public async Task<string> GetAvatarAsync(string name, CancellationToken token) => await GetAvatarAsync(await OsuUtils.GetUserdataAsync(name, token), token);
		public async Task<string> GetAvatarAsync(Userdata user, CancellationToken token)
		{
			var source = TryGetFromCache($"{user.Id}.png");
			if(source != null) return source;

			var resultPath = Path.Combine(CacheDirectory, $"{user.Id}.png");
			await _pluginInitContext.API.HttpDownloadAsync(user.ProfileImageUrl, resultPath, token);

			return resultPath;
		}
	}
}
