using Flow.Launcher.Plugin.Osu.Model;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Flow.Launcher.Plugin.Osu
{
	public class CachingComponent
	{
		public delegate Task<Stream> AsyncStreamProvider(CancellationToken token);
		public readonly string RootDirectory;
		public CachingComponent(string root)
		{
			RootDirectory = Path.Combine(root, "Cache");
			Directory.CreateDirectory(RootDirectory);
		}

		public async Task<string> TryGetOrSaveAsync(string fn, AsyncStreamProvider sp, CancellationToken token)
		{
			var rfn = Path.Combine(RootDirectory, fn);
			if(TryGet(fn) != default)
				return rfn;

			var fp = File.Create(rfn);
			var st = await sp(token);
			try
			{
				await st.CopyToAsync(fp);
			}
			finally
			{
				fp.Close();
			}


			return rfn;
		}

		public void ClearCache()
		{
			 foreach(var fn in Directory.GetFiles(RootDirectory))
				File.Delete(fn);
		}

		public string? TryGet(string fn)
		{
			var fi = new FileInfo(Path.Combine(RootDirectory, fn));
			return fi.Exists ? fi.FullName : default;
		}
	}
}
