using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow.Launcher.Plugin.Osu
{
	public record Userdata(
		string Name,
		int Id
	)
	{
		public string ProfileUrl => $"https://osu.ppy.sh/users/{Id}";
		public string ProfileImageUrl => $"https://a.ppy.sh/{Id}";
	}
}
