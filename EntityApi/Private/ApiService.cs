using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EntityApi.Public;
using EntityApi.Public.EventArgs;
using Newtonsoft.Json;

namespace EntityApi.Private
{
    internal class ApiService
    {
        private static ApiService _instance;
        private readonly HttpClient _client;
        private readonly Queue<Action> _callQueue;
        private readonly Queue<Action> _priorityQueue;
        private bool _active;

        public static ApiService GetInstance()
        {
            return _instance ?? (_instance = new ApiService());
        }

        protected ApiService()
        {
            _callQueue = new Queue<Action>();
            _priorityQueue = new Queue<Action>();
            _client = new HttpClient();
        }

        public void SendRequest(ApiCall call, bool prioritize = false)
        {
            SendRequest(call,  prioritize: prioritize,
                onSuccess: (statusCode, reasonPhrase, content) =>
            {
                var args = new ApiResponseArgs((int)statusCode, reasonPhrase);

                call.NotifySuccess(args);
            },
            onFail: (statusCode, reasonPhrase, content) =>
            {
                var args = new ApiResponseArgs((int)statusCode, reasonPhrase);

                call.NotifyFailure(args);
            });
        }

        public void SendRequest<T>(ApiCall<T> call, bool prioritize = false)
        {
            SendRequest(call, prioritize: prioritize,
                onSuccess: (statusCode, reasonPhrase, content) =>
                {
                    var args = new ApiResponseArgs<T>((int)statusCode, reasonPhrase);
                    args.Content = JsonConvert.DeserializeObject<T>(content);
                    call.NotifySuccess(args);
                },
                onFail: (statusCode, reasonPhrase, content) =>
                {
                    var args = new ApiResponseArgs<T>((int)statusCode, reasonPhrase);
                    if (content != null)
                    {
                        try
                        {
                            args.Content = JsonConvert.DeserializeObject<T>(content);
                        }
                        catch (Exception e)
                        {
                            
                        }
                    }
                        
                    call.NotifyFailure(args);
                });
        }

        private void SendRequest(ApiCall call, Action<int, string, string> onSuccess, Action<int, string, string> onFail, bool prioritize = false)
        {
            var config = call.Configuration;

            var requestAction = new Action(async () =>
            {
                var uri = new Uri(config.Url);
                var request = new HttpRequestMessage(config.RequestMethod, uri);

                if (config.HasIdentity && config.AuthenticationLevel > 0)
                {
                    if (!config.Identity.IsAuthenticated(config.AuthenticationLevel))
                    {
                        Task.Run(() =>
                        {
                            call.NotifyFailure(new ApiResponseArgs(401, "Unauthorized")); //!!! hardcode
                        });

                        NextCall();
                    }

                    request.Headers.Add(config.Identity.AuthenticationHeader.Key, config.Identity.AuthenticationHeader.Value);
                }

                foreach (var kv in config.RequestHeaders)
                {
                    request.Headers.Add(kv.Key, kv.Value);

                }

                if (config.HasBody)
                {
                    request.Content = config.Body;
                }

                try
                {
                    var response = await _client.SendAsync(request);
                    var content = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        Task.Run(() => { onSuccess((int)response.StatusCode, response.ReasonPhrase, content); });
                    }
                    else
                    {
                        Task.Run(() => { onFail((int)response.StatusCode, response.ReasonPhrase, content); });
                    }

                }
                catch (Exception ex)
                {
                    Task.Run(() =>
                    {
                        call.NotifyFailure(new ApiResponseArgs(400, ex.Message));
                    });
                }
                finally
                {
                    NextCall();
                }

            });

            EnqueueApiCall(requestAction, prioritize);
        }

        private void EnqueueApiCall(Action call, bool prioritize)
        {
            if (prioritize)
            {
                _priorityQueue.Enqueue(call);
            }
            else
            {
                _callQueue.Enqueue(call);
            }

            ActivateCallQueue();
        }



        private void ActivateCallQueue()
        {
            if (_active || !_callQueue.Any())
                return;

            _active = true;
            NextCall();
        }


        private void NextCall()
        {
            Action call;

            if (!_priorityQueue.Any() && !_callQueue.Any())
            {
                _active = false;
                return;
            }

            if (_priorityQueue.Any())
            {
                call = _priorityQueue.Dequeue();
                call.Invoke();
                return;
            }

            call = _callQueue.Dequeue();
            Task.Run(call);
        }
    }
}