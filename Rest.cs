using System;
using System.Threading.Tasks;
using EifelMono.KaOS.Extensions;

namespace EifelMono.Rest
{
    /// <summary>
    /// Rest for Json 
    /// no XML!!!!!
    /// </summary>
    public static class Rest
    {
        /// <summary>
        /// Http the specified baseAddress.
        /// </summary>
        /// <returns>The http.</returns>
        /// <param name="baseAddress">Base address.</param>
        public static RestPipeRequest Request(string baseAddress)
        {
            return new RestPipeRequest().Pipe(p => p.Config.BaseAddress = baseAddress);
        }

        /// <summary>
        /// Use the specified restPipe.
        /// </summary>
        /// <returns>The use.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        public static RestPipeRequest Use(RestPipeRequest restPipe)
        {
            return new RestPipeRequest().Pipe(p => p.Config.HttpClient = restPipe.Config.HttpClient);
        }
    }
}
