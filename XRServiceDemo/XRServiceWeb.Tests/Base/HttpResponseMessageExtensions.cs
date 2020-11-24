using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace XRServiceWeb.Tests.Base
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task EnsureSuccessStatusCode(this HttpResponseMessage response, ITestOutputHelper output)
        {
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                var content = await response.Content.ReadAsStringAsync();
                output.WriteLine("Response:");
                output.WriteLine("=======");
                output.WriteLine(content);
                throw;
            }
        }
    }
}
