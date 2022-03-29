using System;
using System.Collections.Generic;

namespace Flow.Launcher.Plugin.Osu
{
	public record User(
		string Name,
		long Id,
		string Joined,
		long Count300,
		long Count100,
		long Count50,
		long Playcount,
		long RankedScore,
		long TotalScore,
		long PpRank,
		double Level,
		double PpRaw,
		double Accuracy,
		long CountRankSS,
		long CountRankSSH,
		long CountRankS,
		long CountRankSH,
		long CountRankA,
		string Country,
		long TotalSecondsPlayed,
		long PpCountryRank,
		List<Event> Events
	)
	{
		public string ProfileUrl => $"https://osu.ppy.sh/users/{Id}";
		public string ProfileImageUrl => $"https://a.ppy.sh/{Id}";
	}
}
