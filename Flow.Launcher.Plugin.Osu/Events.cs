
namespace Flow.Launcher.Plugin.Osu
{
	public record Event(
		string DisplayHtml,
		long BeatmapId,
		long BeatmapsetId,
		string Date,
		byte Epificator
	)
	{
		
	}
}