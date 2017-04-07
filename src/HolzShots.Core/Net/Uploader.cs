using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HolzShots.Net
{
    public abstract class Uploader
    {
        protected string SuggestedUserAgent { get; } = "Mozilla/5.0 (compatible; HolzShots; +https://holzshots.net)"; // "HolzShots/2.0";

        /// <summary>
        /// Override tbhis to present some settings UI to the user.
        /// This function can be invoked on uploader invokation or via the plugin settings located in the application settings.
        /// </summary>
        public virtual Task InvokeSettingsAsync(SettingsInvocationContexts context) => Task.FromResult(false);

        public virtual SettingsInvocationContexts GetSupportedSettingsContexts() => SettingsInvocationContexts.None;

        // Return null if no action needed?
        public abstract Task<UploadResult> InvokeAsync(Stream data, string suggestedFileName, string mimeType, CancellationToken cancellationToken, IProgress<UploadProgress> progress);
    }

    [Flags]
    public enum SettingsInvocationContexts
    {
        None = 0,
        OnUse = 1,
        OnPluginList = 2,
        All = OnUse | OnPluginList,
    }
}
