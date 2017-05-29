using System;
namespace EifelMono.Rest
{
    /// <summary>
    /// Rest pipe.
    /// </summary>
    public class RestPipeRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:EifelMono.Rest.RestPipe"/> class.
        /// </summary>
        public RestPipeRequest()
        {
            Config = new RestConfig();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:EifelMono.Rest.RestPipe"/> class.
        /// </summary>
        /// <param name="restPipe">Rest pipe.</param>
        public RestPipeRequest(RestPipeRequest restPipe) : this()
        {
            Config = restPipe.Config;
        }

        /// <summary>
        /// Gets or sets the config.
        /// </summary>
        /// <value>The config.</value>
        public RestConfig Config { get; set; }
    }
}
