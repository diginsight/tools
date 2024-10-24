﻿using Diginsight.Options;
using System.Net;

namespace DiginsightCopilotApi.Models;

public class HttpContextOptions : IDynamicallyConfigurable, IVolatilelyConfigurable
{
    public string Method { get; set; }
    public string Url { get; set; }
    public string Uri { get; set; }
    public string Path { get; set; }
    public string Query { get; set; }
    public string Host { get; set; }
    public string Port { get; set; }
    public string Scheme { get; set; }
    public string Referer { get; set; }
    public string RefererHost { get; set; }
    public string Authority { get; set; }
    public string Curl { get; set; }
    public List<HttpRequestHeader> Headers { get; set; }
}

