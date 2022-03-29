using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flow.Launcher.Plugin.Osu.Model;

namespace Flow.Launcher.Plugin.Osu
{
	public class Osu : IAsyncPlugin, IContextMenu, IResultUpdated
	{
		private PluginInitContext _context  = default!;
		private OsuApi _api = default!;

		public event ResultUpdatedEventHandler ResultsUpdated  = default!;

		public async Task InitAsync(PluginInitContext context)
		{
			_context = context;
			_api = new("", context);
		}

		public List<Result> LoadContextMenus(Result selectedResult)
		{
			if (selectedResult.ContextData.GetType() != typeof(Beatmapset)) return new List<Result>();
			var bmdata = (selectedResult.ContextData as Beatmapset)!;

			return bmdata.Beatmaps.Select(e => new Result()
			{
				Title = $"{e.Version} {e.DifficultyRating}* [{e.Mode}] {e.Bpm}bpm",
				SubTitle = $"CS{e.Cs} AR{e.Ar} OD{e.Accuracy} HP{e.Drain} MaxCombo:{e.MaxCombo} Cirlces:{e.CountCircles} Sliders:{e.CountSliders} Spinners:{e.CountSpinners}",
				IcoPath = _api.GetBeatmapThumbnailAsync(bmdata, CancellationToken.None).Result,
				Action = (ctx) => { _context.API.OpenUrl(e.Link); return true; }
			}).ToList();
		}

		public async Task<List<Result>> QueryAsync(Query query, CancellationToken token)
		{
			switch (query.FirstSearch)
			{
				case "u":
				case "user":
					List<Result> data = new();
					foreach(var name in query.SearchTerms.Skip(1))
					{
						if (name == null) continue;
						var userdata = await _api.GetUserdataAsync(name, token);
						if (userdata == null) continue;
						data.Add(new Result()
						{
							IcoPath = await _api.GetAvatarAsync(userdata, token),
							ContextData = userdata,
							Title = userdata.Name,
							SubTitle = $"{userdata.Id}",
							Action = (ctx) => { _context.API.OpenUrl(userdata.ProfileUrl); return true; }
						});
					}
					return data;
				default:
					List<Result> results = new();
					foreach (var e in await _api.SearchBeatmapsetsAsync(query.SecondToEndSearch, token))
					{
						results.Add(new Result()
						{
							IcoPath = await _api.GetBeatmapThumbnailAsync(e, token),
							Title = $"{e.Artist} - {e.Title} ~{Math.Round(e.Beatmaps.Sum(b => b.DifficultyRating) / e.Beatmaps.Count, 2)}*",
							SubTitle = e.Creator,
							ContextData = e,
							Action = (ctx) => { _context.API.OpenUrl($"https://osu.ppy.sh/s/{e.Id}"); return true; }
						});
						ResultsUpdated?.Invoke(this, new ResultUpdatedEventArgs() { Token = token, Query = query, Results = results });
					}
					return results;
			}
		}
	}
}