using System;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using EifelMono.KaOS.Extensions;

namespace EifelMono.Rest
{
    /// <summary>
    /// Rest pipe extensions.
    /// </summary>
    public static class RestPipeExtensions
    {
        #region Request
        /// <summary>
        /// Proxy the specified restPipe and urlDetails.
        /// </summary>
        /// <returns>The proxy.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="proxy">WebProxy</param>
        public static RestPipeRequest Proxy(this RestPipeRequest restPipe, IWebProxy proxy)
        {
            return restPipe.Pipe((p) => p.Config.Proxy = proxy);
        }

        /// <summary>
        /// Cokis the container.
        /// </summary>
        /// <returns>The container.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="cookieContainer">Cookie container.</param>
        public static RestPipeRequest CookieContainer(this RestPipeRequest restPipe, CookieContainer cookieContainer)
        {
            return restPipe.Pipe((p) => p.Config.CookieContainer = cookieContainer);
        }

        /// <summary>
        /// Path the specified restPipe and urlDetails.
        /// </summary>
        /// <returns>The path.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="urlDetails">URL details.</param>
        public static RestPipeRequest Path(this RestPipeRequest restPipe, params string[] urlDetails)
        {
            return restPipe.Pipe(p => p.Config.Path = urlDetails.ToList());
        }

        /// <summary>
        /// Header the specified restPipe, key and value.
        /// </summary>
        /// <returns>The header.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public static RestPipeRequest Header(this RestPipeRequest restPipe, string key, string value)
        {
            return restPipe.Pipe(p => p.Config.Headers[key] = value);
        }

        /// <summary>
        /// Parameter the specified restPipe, key and value.
        /// </summary>
        /// <returns>The parameter.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public static RestPipeRequest Parameter(this RestPipeRequest restPipe, string key, string value)
        {
            return restPipe.Pipe(p => p.Config.Parameters[key] = value);
        }

        /// <summary>
        /// Body the specified restPipe, body and mimeType.
        /// </summary>
        /// <returns>The body.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="body">Body.</param>
        /// <param name="mimeType">MIME type.</param>
        public static RestPipeRequest BodyAsMimeType(this RestPipeRequest restPipe, object body, RestMimeType mimeType)
        {
            return restPipe.Pipe((p) =>
            {
                p.Config.Body = body;
                p.Config.MimeType = mimeType;
            });
        }

        /// <summary>
        /// Bodies as text.
        /// </summary>
        /// <returns>The as text.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="text">Text.</param>
        public static RestPipeRequest BodyAsTextPlain(this RestPipeRequest restPipe, string text)
        {
            return BodyAsMimeType(restPipe, text, RestMimeType.TextPlain);
        }

        /// <summary>
        /// Adds an object as json to the body
        /// </summary>
        /// <returns>The as json.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="body">Body.</param>
        public static RestPipeRequest BodyAsJson(this RestPipeRequest restPipe, object body)
        {
            return BodyAsMimeType(restPipe, JsonConvert.SerializeObject(body), RestMimeType.ApplicationJson);
        }
        #endregion

        #region Execute
        private static List<string> RestMimeTypeConvertJson = new List<string>
        {
            RestMimeType.ApplicationJson.AsText,
        };

        private static List<string> RestMimeTypeConvertText = new List<string>
        {
            RestMimeType.TextHtlm.AsText,
            RestMimeType.TextPlain.AsText,
        };

        private static List<string> RestMimeTypeConvertRaw = new List<string>
        {
            RestMimeType.ImagePng.AsText,
        };

        /// <summary>
        /// Befores the execute.
        /// </summary>
        /// <returns>The execute.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="afterPrepare">After prepare.</param>
        public static RestPipeRequest BeforeExecute(this RestPipeRequest restPipe, Action<RestPipeRequest> afterPrepare)
        {
            return restPipe.Pipe(p => p.Config.BeforeExecute = afterPrepare);
        }

        /// <summary>
        /// Executes the aync.
        /// </summary>
        /// <param name="restPipe">Rest pipe.</param>
        private static async Task<RestPipeResponse> ExecuteAsync(this RestPipeRequest restPipe)
        {
            try
            {
                if (restPipe.Config.HttpClient == null)
                {
                    if (restPipe.Config.HttpClientHandler == null)
                    {
                        restPipe.Config.HttpClientHandler = new HttpClientHandler
                        {
                            CookieContainer = restPipe.Config.CookieContainer,
                            UseCookies = restPipe.Config.CookieContainer != null,
                            UseDefaultCredentials = false,
                            Proxy = restPipe.Config.Proxy,
                            UseProxy = restPipe.Config.Proxy != null,
                        };

                        if (restPipe.Config.HttpClientHandler.SupportsAutomaticDecompression)
                        {
                            restPipe.Config.HttpClientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                        }
                    }
                    restPipe.Config.HttpClient = new HttpClient(restPipe.Config.HttpClientHandler);
                }
                var uriString = restPipe.Config.BaseAddress;
                foreach (var path in restPipe.Config.Path)
                    if (!string.IsNullOrEmpty(path))
                        uriString = $"{uriString.Trim().TrimEnd('/')}/{path.Trim().TrimStart('/')}";

                foreach (var kv in restPipe.Config.Parameters)
                    uriString = uriString.Replace($"{{{kv.Key}}}", kv.Value);

                restPipe.Config.HttpRequestMessage = new HttpRequestMessage(restPipe.Config.HttpMethod, new Uri(uriString));

                var mediaTye = restPipe.Config.MimeType.AsText;
                var body = restPipe.Config.Body;
                if (body != null)
                {
                    if (RestMimeTypeConvertJson.Contains(mediaTye))
                    {
                        restPipe.Config.HttpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(body),
                                                                                      restPipe.Config.Encoding,
                                                                                      restPipe.Config.MimeType.AsText);
                    }
                    else if (RestMimeTypeConvertText.Contains(mediaTye))
                    {
                        restPipe.Config.HttpRequestMessage.Content = new StringContent(body.ToString(),
                                                                               restPipe.Config.Encoding,
                                                                               restPipe.Config.MimeType.AsText);
                    }
                    else if (RestMimeTypeConvertRaw.Contains(mediaTye))
                    {
                        if (typeof(byte[]).GetTypeInfo().IsAssignableFrom(body.GetType()))
                        {
                            restPipe.Config.HttpRequestMessage.Content = new ByteArrayContent((byte[])body);
                        }
                        else if (typeof(Stream).GetTypeInfo().IsAssignableFrom(body.GetType()))
                        {
                            restPipe.Config.HttpRequestMessage.Content = new StreamContent((Stream)body);
                        }
                    }
                    else
                    {
                    }
                }

                foreach (var kv in restPipe.Config.Headers)
                    restPipe.Config.HttpRequestMessage.Headers.Add(kv.Key, kv.Value);

                restPipe.Config.CancellationTokenSource = new CancellationTokenSource();

                restPipe.Config.BeforeExecute?.Invoke(restPipe);

                restPipe.Config.HttpResponseMessage = await restPipe.Config.HttpClient.SendAsync(restPipe.Config.HttpRequestMessage,
                                                                                                 restPipe.Config.CancellationTokenSource.Token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                restPipe.Config.Exception = ex;
                restPipe.Config.OnException?.Invoke(restPipe, ex);
            }
            return new RestPipeResponse(restPipe);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="httpMethod">Http method.</param>
        /// <param name="onResponse">On answer.</param>
        private static async Task<RestPipeResponse> ExecuteAsync(this RestPipeRequest restPipe, HttpMethod httpMethod, Action<RestPipeResponse> onResponse = null)
        {
            restPipe.Config.HttpMethod = httpMethod;
            var result = await restPipe.ExecuteAsync().ConfigureAwait(false);
            onResponse?.Invoke(result);
            return new RestPipeResponse(result);
        }

        /// <summary>
        /// Executes the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="httpMethod">Http method.</param>
        /// <param name="onResponse">On response.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        /// <exception cref="T:System.NotImplementedException"></exception>
        private static async Task<RestPipeResponse<T>> ExecuteAsync<T>(this RestPipeRequest restPipe, HttpMethod httpMethod, Action<RestPipeResponse<T>, T> onResponse = null)
        {
            restPipe.Config.HttpMethod = httpMethod;
            var result = new RestPipeResponse<T>(await restPipe.ExecuteAsync().ConfigureAwait(false));
            result.Data = default(T);
            if (result.Config.HttpResponseMessage != null && result.Config.HttpResponseMessage.IsSuccessStatusCode)
            {
                #region convert To T
                try
                {
                    var mediaTye = restPipe.Config.HttpResponseMessage.Content.Headers.ContentType.MediaType;
                    if (RestMimeTypeConvertJson.Contains(mediaTye))
                    {
                        result.Data = JsonConvert.DeserializeObject<T>(await restPipe.Config.HttpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false));
                    }
                    else if (RestMimeTypeConvertText.Contains(mediaTye))
                    {
                        if (typeof(string).GetTypeInfo().IsAssignableFrom(typeof(T)))
                            result.Data = (T)(object)(await restPipe.Config.HttpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false));
                    }
                    else if (RestMimeTypeConvertRaw.Contains(mediaTye))
                    {
                        if (typeof(byte[]).GetTypeInfo().IsAssignableFrom(typeof(T)))
                            result.Data = (T)(object)(await restPipe.Config.HttpResponseMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false));
                        else if (typeof(Stream).GetTypeInfo().IsAssignableFrom(typeof(T)))
                            result.Data = (T)(object)(await restPipe.Config.HttpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false));
                    }
                    else
                    {
                    }
                }
                catch (Exception ex)
                {
                    result.Config.Exception = ex;
                }
                #endregion
            }
            onResponse?.Invoke(result, result.Data);
            return result;
        }

        #endregion

        #region Http Executes

        #region Get
        /// <summary>
        /// Gets the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="onResponse">On response.</param>
        public static async Task<RestPipeResponse> GetResponseAsync(this RestPipeRequest restPipe, Action<RestPipeResponse> onResponse = null)
        {
            return await restPipe.ExecuteAsync(HttpMethod.Get, onResponse).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="onResponse">On response.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static async Task<RestPipeResponse<T>> GetResponseAsync<T>(this RestPipeRequest restPipe, Action<RestPipeResponse<T>, T> onResponse = null)
        {
            return await restPipe.ExecuteAsync<T>(HttpMethod.Get, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="onResponse">On response.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static async Task<T> GetValueAsync<T>(this RestPipeRequest restPipe, Action<RestPipeResponse<T>, T> onResponse = null)
        {
            return (await restPipe.ExecuteAsync<T>(HttpMethod.Get, onResponse).ConfigureAwait(false)).Data;
        }
        #endregion

        #region Post
        /// <summary>
        /// Posts the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="onResponse">On response.</param>
        public static async Task<RestPipeResponse> PostResponseAsync(this RestPipeRequest restPipe, Action<RestPipeResponse> onResponse = null)
        {
            return await restPipe.ExecuteAsync(HttpMethod.Post, onResponse).ConfigureAwait(false);
        }

        /// <summary>
        /// Posts the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="onResponse">On response.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static async Task<RestPipeResponse<T>> PostResponseAsync<T>(this RestPipeRequest restPipe, Action<RestPipeResponse<T>, T> onResponse = null)
        {
            return await restPipe.ExecuteAsync<T>(HttpMethod.Post, onResponse).ConfigureAwait(false);
        }

        /// <summary>
        /// Posts the value async.
        /// </summary>
        /// <returns>The value async.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="onResponse">On response.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static async Task<T> PostValueAsync<T>(this RestPipeRequest restPipe, Action<RestPipeResponse<T>, T> onResponse = null)
        {
            return (await restPipe.ExecuteAsync<T>(HttpMethod.Post, onResponse).ConfigureAwait(false)).Data;
        }
        #endregion

        #region Put
        /// <summary>
        /// Puts the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="onResponse">On response.</param>
        public static async Task<RestPipeResponse> PutResponseAsync(this RestPipeRequest restPipe, Action<RestPipeResponse> onResponse = null)
        {
            return await restPipe.ExecuteAsync(HttpMethod.Put, onResponse).ConfigureAwait(false);
        }

        /// <summary>
        /// Puts the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="onResponse">On response.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static async Task<RestPipeResponse<T>> PutResponseAsync<T>(this RestPipeRequest restPipe, Action<RestPipeResponse<T>, T> onResponse = null)
        {
            return await restPipe.ExecuteAsync<T>(HttpMethod.Put, onResponse).ConfigureAwait(false);
        }

        /// <summary>
        /// Puts the value async.
        /// </summary>
        /// <returns>The value async.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="onResponse">On response.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static async Task<T> PutValueAsync<T>(this RestPipeRequest restPipe, Action<RestPipeResponse<T>, T> onResponse = null)
        {
            return (await restPipe.ExecuteAsync<T>(HttpMethod.Put, onResponse).ConfigureAwait(false)).Data;
        }
        #endregion

        #region Delete
        /// <summary>
        /// Deletes the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="onResponse">On response.</param>
        public static async Task<RestPipeResponse> DeleteResponseAsync(this RestPipeRequest restPipe, Action<RestPipeResponse> onResponse = null)
        {
            return await restPipe.ExecuteAsync(HttpMethod.Delete, onResponse).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="onResponse">On response.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static async Task<RestPipeResponse<T>> DeleteResponseAsync<T>(this RestPipeRequest restPipe, Action<RestPipeResponse<T>, T> onResponse = null)
        {
            return await restPipe.ExecuteAsync<T>(HttpMethod.Delete, onResponse).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="onResponse">On response.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static async Task<T> DeleteValueAsync<T>(this RestPipeRequest restPipe, Action<RestPipeResponse<T>, T> onResponse = null)
        {
            return (await restPipe.ExecuteAsync<T>(HttpMethod.Delete, onResponse).ConfigureAwait(false)).Data;
        }
        #endregion

        #region Head
        /// <summary>
        /// Heads the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="onResponse">On response.</param>
        public static async Task<RestPipeResponse> HeadResponseAsync(this RestPipeRequest restPipe, Action<RestPipeResponse> onResponse = null)
        {
            return await restPipe.ExecuteAsync(HttpMethod.Head, onResponse).ConfigureAwait(false);
        }

        /// <summary>
        /// Heads the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="onResponse">On response.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static async Task<RestPipeResponse<T>> HeadResponseAsync<T>(this RestPipeRequest restPipe, Action<RestPipeResponse<T>, T> onResponse = null)
        {
            return await restPipe.ExecuteAsync<T>(HttpMethod.Head, onResponse).ConfigureAwait(false);
        }

        /// <summary>
        /// Heads the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="onResponse">On response.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static async Task<T> HeadValueAsync<T>(this RestPipeRequest restPipe, Action<RestPipeResponse<T>, T> onResponse = null)
        {
            return (await restPipe.ExecuteAsync<T>(HttpMethod.Head, onResponse).ConfigureAwait(false)).Data;
        }
        #endregion

        #region Options
        /// <summary>
        /// Optionses the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="onResponse">On response.</param>
        public static async Task<RestPipeResponse> OptionsResponseAsync(this RestPipeRequest restPipe, Action<RestPipeResponse> onResponse = null)
        {
            return await restPipe.ExecuteAsync(HttpMethod.Options, onResponse).ConfigureAwait(false);
        }

        /// <summary>
        /// Optionses the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="onResponse">On response.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static async Task<RestPipeResponse<T>> OptionsResponseAsync<T>(this RestPipeRequest restPipe, Action<RestPipeResponse<T>, T> onResponse = null)
        {
            return await restPipe.ExecuteAsync<T>(HttpMethod.Options, onResponse).ConfigureAwait(false);
        }

        /// <summary>
        /// Optionses the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="onResponse">On response.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static async Task<T> OptionsValueAsync<T>(this RestPipeRequest restPipe, Action<RestPipeResponse<T>, T> onResponse = null)
        {
            return (await restPipe.ExecuteAsync<T>(HttpMethod.Options, onResponse).ConfigureAwait(false)).Data;
        }
        #endregion

        #region Trace
        /// <summary>
        /// Traces the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="onResponse">On response.</param>
        public static async Task<RestPipeResponse> TraceAsync(this RestPipeRequest restPipe, Action<RestPipeResponse> onResponse = null)
        {
            return await restPipe.ExecuteAsync(HttpMethod.Trace, onResponse).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="onResponse">On response.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static async Task<RestPipeResponse<T>> TraceAsync<T>(this RestPipeRequest restPipe, Action<RestPipeResponse<T>, T> onResponse = null)
        {
            return await restPipe.ExecuteAsync<T>(HttpMethod.Trace, onResponse).ConfigureAwait(false);
        }

        /// <summary>
        /// Traces the value async.
        /// </summary>
        /// <returns>The value async.</returns>
        /// <param name="restPipe">Rest pipe.</param>
        /// <param name="onResponse">On response.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static async Task<T> TraceValueAsync<T>(this RestPipeRequest restPipe, Action<RestPipeResponse<T>, T> onResponse = null)
        {
            return (await restPipe.ExecuteAsync<T>(HttpMethod.Trace, onResponse).ConfigureAwait(false)).Data;
        }
        #endregion

        #endregion
    }
}
