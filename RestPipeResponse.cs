using System;
namespace EifelMono.Rest
{
    /// <summary>
    /// Rest pipe response.
    /// </summary>
    public class RestPipeResponse : RestPipeRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:EifelMono.Rest.RestPipeResponse"/> class.
        /// </summary>
        /// <param name="restPipe">Rest pipe.</param>
        public RestPipeResponse(RestPipeRequest restPipe) : base(restPipe)
        {
        }
    }

    /// <summary>
    /// Rest pipe response.
    /// </summary>
    public class RestPipeResponse<T> : RestPipeRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:EifelMono.Rest.RestPipeResponse`1"/> class.
        /// </summary>
        /// <param name="restPipe">Rest pipe.</param>
        public RestPipeResponse(RestPipeRequest restPipe) : base(restPipe)
        {
        }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public T Data { get; set; } = default(T);
    }
}
