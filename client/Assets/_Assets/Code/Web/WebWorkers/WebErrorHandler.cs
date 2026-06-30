using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebErrorHandler
{
    public static void Handle(long responseCode, string error, string url, string downloadHandlerText, bool failSilently, System.Action onComplete)
    {
        WebError webError = new WebError()
        {
            responseCode = responseCode,
            error = error,
            requestUrl = url,
            downloadHandlerText = downloadHandlerText
        };

        Handle(webError, failSilently, onComplete);
    }

    public static void Handle(UnityWebRequest request, bool failSilently, System.Action onComplete)
    {
        WebError error = new WebError(request);

        Handle(error, failSilently, onComplete);
    }

    private static void Handle(WebError error, bool failSilently, System.Action onComplete)
    {
        string msg = string.Empty;

        switch(error.responseCode)
        {
            default:
                //Unknown Error
                msg = $"Unknown Error, Try again or contact support.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

             case 0:

                //Unable to reach server.
                //Server did not respond
                //Not connected to internet

                if(error.error.Equals("Request timeout"))
                {
                    msg = $"Request timeout. There might be a problem with your internet connection or the server is not responding. Please try again.\nError code: #{error.responseCode}";
                }
                else
                {
                    msg = $"Unable to reach server. There might be a problem with your internet connection or the server is not responding. Please try again.\nError code: #{error.responseCode}";
                }

                
                PresentToPlayer(msg, onComplete, failSilently);
                return;




            case 200:
                //OK
                msg = $"Server returned success, but did not provide the expected response.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 201:
                //Created
                msg = $"Server fulfilled the request, but did not provide the expected response.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 202:
                //Accepted
                msg = $"The request has been accepted for processing, but the processing has not been completed. The server did not provide the expected response.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 203:
                //Non-Authoritative Information
                msg = $"The server is a transforming proxy (e.g. a Web accelerator) that received a 200 OK from its origin, but is returning a modified version of the origin's response. The server did not provide the expected response.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 204:
                //No Content
                msg = $"The server successfully processed the request and is not returning any content. The server did not provide the expected response.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 205:
                //Reset Content
                msg = $"The server successfully processed the request, but is not returning any content. The server did not provide the expected response.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 206:
                //Partial Content
                msg = $"The server is delivering only part of the resource (byte serving) due to a range header sent by the client. The server did not provide the expected response.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 207:
                //Multi-Status
                msg = $"The message body that follows is by default an XML message and can contain a number of separate response codes, depending on how many sub-requests were made. The server did not provide the expected response.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 208:
                //Already Reported
                msg = $"The members of a DAV binding have already been enumerated in a preceding part of the (multistatus) response, and are not being included again. The server did not provide the expected response.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 226:
                //IM Used
                msg = $"The server has fulfilled a request for the resource, and the response is a representation of the result of one or more instance-manipulations applied to the current instance. The server did not provide the expected response.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;
            



            case 300:
                //Multiple Choices
                msg = $"There are multiple options for the resource from which the client may choose.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 301:
                //Moved Permanently
                msg = $"This and all future requests should be directed to the given URI.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 302:
                //Found
                msg = $"Redirects the client to browse to another URL.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 303:
                //See Other
                msg = $"The response to the request can be found under another URI using the GET method.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 304:
                //Not Modified
                msg = $"The resource has not been modified since the version specified by the request headers If-Modified-Since or If-None-Match.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 305:
                //The requested resource is available only through a proxy.
                msg = $"The requested resource is available only through a proxy.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 306:
                //Switch Proxy
                msg = $"Subsequent requests should use the specified proxy.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 307:
                //Temporary Redirect
                msg = $"The request should be repeated with another URI; however, future requests should still use the original URI.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 308:
                //Permanent Redirect
                msg = $"The request and all future requests should be repeated using another URI.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;




            case 400:
                //Bad request
                msg = $"The server cannot or will not process the request due to an apparent client error.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 401:
                //Unauthorized
                msg = $"Authentication is required and has failed or has not yet been provided.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 402:
                //Payment Required
                msg = $"Payment Required.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;
            
            case 403:
                //Forbidden
                msg = $"The request contained valid data and was understood by the server, but the server is refusing action. Necessary permissions for a resource missing.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 404:
                //Not found
                msg = $"The requested resource could not be found but may be available in the future.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 405:
                //Method Not Allowed
                msg = $"Method Not Allowed.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 406:
                //Not Acceptable
                msg = $"Not Acceptable.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 407:
                //Proxy Authentication Required
                msg = $"Proxy Authentication Required.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 408:
                //Request Timeout
                msg = $"Request Timeout.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 409:
                //Conflict
                msg = $"Request Conflict.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 410:
                //Gone
                msg = $"Resouce no longer available.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 411:
                //Length Required
                msg = $"The request did not specify the length of its content.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 412:
                //Precondition Failed
                msg = $"The server does not meet one of the preconditions that the requester put on the request header fields.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;
            
            case 413:
                //Payload Too Large
                msg = $"The request is larger than the server is willing or able to process.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 414:
                //URI Too Long
                msg = $"The URI provided was too long for the server to process. Often the result of too much data being encoded as a query-string of a GET request, in which case it should be converted to a POST request.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 415:
                //Unsupported Media Type
                msg = $"The request entity has a media type which the server or resource does not support.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 417:
                //Expectation Failed
                msg = $"The server cannot meet the requirements of the Expect request-header field.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 418:
                //I'm a teapot
                msg = $"Returned by teapots requested to brew coffee.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 421:
                //Misdirected Request
                msg = $"The request was directed at a server that is not able to produce a response.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 422:
                //Unprocessable Entity
                msg = $"The request was well-formed but was unable to be followed due to semantic errors.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 423:
                //Locked
                msg = $"The resource that is being accessed is locked.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 424:
                //Failed Dependency
                msg = $"The request failed because it depended on another request and that request failed.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 425:
                //Too Early
                msg = $"Indicates that the server is unwilling to risk processing a request that might be replayed.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 426:
                //Upgrade Required
                msg = $"The client should switch to a different protocol such as TLS/1.0, given in the Upgrade header field.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 428:
                //Precondition Required
                msg = $"The origin server requires the request to be conditional.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 429:
                //Too Many Requests
                msg = $"The user has sent too many requests in a given amount of time. Intended for use with rate-limiting schemes.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 431:
                //Request Header Fields Too Large
                msg = $"The server is unwilling to process the request because either an individual header field, or all the header fields collectively, are too large.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 451:
                //Unavailable For Legal Reasons
                msg = $"A server operator has received a legal demand to deny access to a resource or to a set of resources that includes the requested resource.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;




            case 500:
                //Internal Server Error
                msg = $"Internal Server Error.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 501:
                //Not Implemented
                msg = $"Not Implemented.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 502:
                //Bad Gateway
                msg = $"The server was acting as a gateway or proxy and received an invalid response from the upstream server.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 503:
                //Service Unavailable
                msg = $"The server cannot handle the request because it is overloaded or down for maintenance.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;
                
            case 504:
                //Gateway Timeout
                msg = $"The server was acting as a gateway or proxy and did not receive a timely response from the upstream server.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 505:
                //HTTP Version Not Supported
                msg = $"The server does not support the HTTP protocol version used in the request.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 506:
                //Variant Also Negotiates
                msg = $"Transparent content negotiation for the request results in a circular reference.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 507:
                //Insufficient Storage
                msg = $"The server is unable to store the representation needed to complete the request.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 508:
                //Loop Detected
                msg = $"The server detected an infinite loop while processing the request.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 510:
                //Not Extended
                msg = $"Further extensions to the request are required for the server to fulfil it.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 511:
                //Network Authentication Required
                msg = $"The client needs to authenticate to gain network access.\nError code: #{error.responseCode}";
                PresentToPlayer(msg, onComplete, failSilently);
                return;

            case 600:
                //Custom Error
                msg = error.downloadHandlerText;
                PresentToPlayer(msg, onComplete, failSilently);
                return;
        }
    }

    private static void PresentToPlayer(string errorMsg, System.Action onComplete, bool failSilently)
    {
        if(failSilently)
        {
            if(Globals.debugConstants.verboseLogging)
                Debug.LogWarning("Failed Silently: " + errorMsg);
                
            onComplete?.Invoke();
            return;
        }
        else
        {
            if(Globals.debugConstants.verboseLogging)
                Debug.LogError(errorMsg);
        }

        DialogCanvas.instance.HideLoading();
        DialogCanvas.instance.HideProgress();

        new Dialog(TranslationKey.Generic_Error, errorMsg, Globals.localizationConstants.defaultLanguage, false)
        .AddNeutralButton(TranslationKey.Generic_Ok, ()=>{
            onComplete?.Invoke();
        })
        .Show();
    }
}
