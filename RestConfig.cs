using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EifelMono.Rest
{
    /// <summary>
    /// Rest config.
    /// </summary>
    public class RestConfig
    {
        /// <summary>
        /// Gets or sets the web proxy.
        /// </summary>
        /// <value>The web proxy.</value>
        public IWebProxy Proxy { get; set; } = null;

        /// <summary>
        /// Gets or sets the cookie container.
        /// </summary>
        /// <value>The cookie container.</value>
        public CookieContainer CookieContainer { get; set; } = null;

        /// <summary>
        /// Gets or sets the http client handler.
        /// </summary>
        /// <value>The http client handler.</value>
        public HttpClientHandler HttpClientHandler { get; set; } = null;

        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        /// <value>The client.</value>
        public HttpClient HttpClient { get; set; } = null;

        /// <summary>
        /// Gets or sets the base address.
        /// </summary>
        /// <value>The base address.</value>
        public string BaseAddress { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>The path.</value>
        public List<string> Path { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the headers.
        /// </summary>
        /// <value>The headers.</value>
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>The body.</value>
        public object Body { get; set; }

        /// <summary>
        /// Gets or sets the type of the MIME.
        /// </summary>
        /// <value>The type of the MIME.</value>
        public RestMimeType MimeType { get; set; } = RestMimeType.Unknown;

        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        /// <value>The encoding.</value>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Gets or sets the http method.
        /// </summary>
        /// <value>The http method.</value>
        public HttpMethod HttpMethod { get; set; } = HttpMethod.Get;

        /// <summary>
        /// Gets or sets the cancellation token source.
        /// </summary>
        /// <value>The cancellation token source.</value>
        public CancellationTokenSource CancellationTokenSource { get; set; }

        /// <summary>
        /// Gets or sets the http request message.
        /// </summary>
        /// <value>The http request message.</value>
        public HttpRequestMessage HttpRequestMessage { get; set; } = null;

        /// <summary>
        /// Gets or sets the before execute.
        /// </summary>
        /// <value>The before execute.</value>
        public Action<RestPipeRequest> BeforeExecute { get; set; } = null;

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>The exception.</value>
        public Exception Exception { get; set; } = null;

        /// <summary>
        /// Gets or sets the on exception.
        /// </summary>
        /// <value>The on exception.</value>
        public Action<RestPipeRequest, Exception> OnException { get; set; } = null;

        /// <summary>
        /// Gets or sets the http response message.
        /// </summary>
        /// <value>The http response message.</value>
        public HttpResponseMessage HttpResponseMessage { get; set; } = null;
    }
}
