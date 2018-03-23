using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace CountingKs.ActionResults
{
    public class VersionedActionResult<TBody> : IHttpActionResult where TBody : class
    {
        private readonly HttpRequestMessage _request;
        private readonly string _version;
        private readonly TBody _body;

        public VersionedActionResult(HttpRequestMessage request, string version, TBody body)
        {
            _request = request;
            _version = version;
            _body = body;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var message = _request.CreateResponse(_body);
            message.Headers.Add("XXX-OurVersion", _version);
            return Task.FromResult(message);
        }
    }
}