using HolzShots.Net;
using HolzShots.Net.Custom;
using System.IO;
using System.Threading;
using Xunit;

namespace HolzShots.Core.Tests.Net.Custom
{
    public class CustomUploaderLoaderTests
    {
        [Theory]
        [FileStringContentData("Files/DirectUpload.net.hsjson")]
        [FileStringContentData("Files/FotosHochladen.hsjson")]
        public void ValidateTest(string content)
        {
            var parseResult = CustomUploader.TryParse(content, out var result);
            Assert.True(parseResult);
            Assert.NotNull(result);
            Assert.NotNull(result.CustomData);
            Assert.NotNull(result.CustomData.Info);
            Assert.NotNull(result.CustomData.Uploader);
            Assert.NotNull(result.CustomData.SchemaVersion);
        }
    }
}
