using System;
using System.Collections;
using Core.Common.Extensions.String;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using Logger = UnityLogger.Logger;

namespace Core.Tools.Networking
{
    public enum RequestType
    {
        Get,

        // Head,//TODO
        // Post,//TODO
        Put
    }

    /// <summary>
    /// Class handles requests between client and server.
    /// </summary>
    public static class RestRequest
    {
        /// <summary>
        /// Observable wrapper for RequestAsync
        /// </summary>
        /// <param name="requestType"></param>
        /// <param name="uri"></param>
        /// <param name="putData"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IObservable<T> Request<T>(RequestType requestType, string uri, string putData = "")
        {
            return Observable.Create(
                (IObserver<T> requestObserver) =>
                {
                    return Observable.FromCoroutine<string>(observer =>
                            RequestAsync(observer, requestType, uri, putData))
                        .CatchIgnore((Exception ex) => { requestObserver.OnError(ex); })
                        .Subscribe(json =>
                        {
                            try
                            {
                                requestObserver.OnNext(JsonUtility.FromJson<T>(json));
                                requestObserver.OnCompleted();
                            }
                            catch (Exception e)
                            {
                                Logger.LogError(e);
                                throw e;
                            }
                        });
                });
        }

        /// <summary>
        /// Facilitates communication with Put and Get requests from server API.
        /// </summary>
        /// <param name="observer"></param>
        /// <param name="requestType"></param>
        /// <param name="uri"></param>
        /// <param name="putData"></param>
        /// <returns></returns>
        private static IEnumerator RequestAsync(IObserver<string> observer, RequestType requestType, string uri, string putData)
        {
            var request = CreateRequest(requestType, uri, putData);
            
            request.SetRequestHeader("poop","pappa");
            
            yield return request.SendWebRequest();

            if (IsError(request, out var error))
            {
                observer.OnError(new Exception(error));
            }
            else
            {
                if (requestType == RequestType.Put)
                    Logger.Log($"{uri} | ".ColoredLog(Colors.Aqua) + $"{putData} | {request.downloadHandler.text}".ColoredLog(Colors.Coral));
                else
                    Logger.Log($"{uri} | ".ColoredLog(Colors.Aqua) + $"{request.downloadHandler.text}".Colored(Colors.Coral));

                observer.OnNext(request.downloadHandler.text);
                observer.OnCompleted();
            }
        }

        private static UnityWebRequest CreateRequest(RequestType requestType, string uri, string putData = "")
        {
            UnityWebRequest request = null;
            switch (requestType)
            {
                case RequestType.Get:
                    request = UnityWebRequest.Get(uri);
                    break;
                case RequestType.Put:
                    request = UnityWebRequest.Put(uri, putData);
                    break;
                // case RequestType.Head://TODO
                //     break;
                // case RequestType.Post://TODO
                //     break;
            }

            return request;
        }

        private static bool IsError(UnityWebRequest request, out string error, bool isBinary = false)
        {
            if (!isBinary && request.downloadHandler.text.Contains("error"))
            {
                Logger.LogError($"URI: {request.uri} | {request.downloadHandler.text}".ColoredLog(Colors.Red));
                error = $"URI: {request.uri} - {request.downloadHandler.text}";
                return true;
            }

            if (request.isNetworkError || request.error != null)
            {
                Logger.LogError($"URI: {request.uri} | {request.error}".ColoredLog(Colors.Red));
                error = request.error;
                return true;
            }

            error = string.Empty;
            return false;
        }
    }
}