namespace Microsoft.Web.Http.OData.Samples
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web.Http.OData.Routing;

    /// <summary>
    /// Provides extension methods for the <see cref="HttpRequestMessage"/> class.
    /// </summary>
    public static class HttpRequestMessageExtensions
    {
        private const string HttpMediaType = @"application/http";
        private const string MessageTypeHeaderParameter = "msgtype";
        private const string HttpRequestHeaderParameter = "request";
        private const string HostHeader = "Host";
        private const string HttpRequestPattern = @"(\S+\s)(.+)(\sHTTP.*)";

        private static bool FixUpHttpRequest( string requestLine, out string fixedUpRequestLine )
        {
            Contract.Requires( !string.IsNullOrEmpty( requestLine ) );

            fixedUpRequestLine = null;
            var match = Regex.Match( requestLine, HttpRequestPattern, RegexOptions.Singleline );

            if ( !match.Success )
                return false;

            var url = match.Groups[2].Value;
            Uri uri;

            if ( !Uri.TryCreate( url, UriKind.RelativeOrAbsolute, out uri ) )
                return false;

            // the request url must be either absolute or start with a '/'
            if ( uri.IsAbsoluteUri || url[0] == '/' )
                return false;

            fixedUpRequestLine = string.Format( CultureInfo.InvariantCulture, "{0}/{1}{2}", match.Groups[1], url, match.Groups[3] );

            return true;
        }

        /// <summary>
        /// Returns a value indicating whether the specified request is for an OData action.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequestMessage">request</see> to evaluate.</param>
        /// <returns>True if the <paramref name="request"/> is for an OData action; otherwise, false.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static bool IsODataAction( this HttpRequestMessage request )
        {
            Contract.Requires( request != null );

            if ( request.Method != HttpMethod.Post )
                return false;

            var config = request.GetConfiguration();

            if ( config == null )
                return false;

            // HACK: because of the HttpRouteCollection implementation OfType yields no results and ToArray and ToList
            // throw exceptions. Skip(0) does the trick although there is no logical difference.
            var url = ( request.RequestUri.IsAbsoluteUri ? request.RequestUri.LocalPath : request.RequestUri.OriginalString ).TrimStart( '/' );
            var routes = config.Routes.Skip( 0 ).OfType<ODataRoute>();
            var actions = from route in routes
                          from constraint in route.Constraints.Values.OfType<ODataPathRouteConstraint>()
                          let segment = url.Substring( ( route.RoutePrefix ?? string.Empty ).Length )
                          let path = constraint.PathHandler.Parse( constraint.EdmModel, segment )
                          where path != null && path.Segments.Last() is ActionPathSegment
                          select true;
            var matched = actions.Any();

            return matched;
        }

        /// <summary>
        /// Normalizes the content of a nested HTTP message.
        /// </summary>
        /// <param name="content">The nested <see cref="HttpContent">HTTP message</see> to normalize.</param>
        /// <param name="host">The value applied to the HTTP Host header, if necessary. This value should be derived
        /// from a parent MIME multipart <see cref="HttpRequestMessage">request</see>.</param>
        /// <returns>The normalized <see cref="HttpContent">content</see>.</returns>
        public static async Task<HttpContent> NormalizeNestedHttpMessageContentAsync( this HttpContent content, string host )
        {
            Contract.Requires( content != null );
            Contract.Requires( !string.IsNullOrEmpty( host ) );
            Contract.Ensures( Contract.Result<Task<HttpContent>>() != null );

            // HACK: ReadAsHttpRequestMessageAsync normalization:
            // --------------------------------------------------------------------------------------------------------------
            // 1. the Host header must be present in the content and the ContentHttpHeaders will not allow the Host header
            //    to be added. we could have tried using some sleazy Reflection, but it is just as easy to read the content,
            //    inject the host header, and reassemble the content.
            //
            // 2. the url for a nested request is often relative; however, the method fails if the url either isn't
            //    absolute or the url doesn't start with a '/'

            var text = await content.ReadAsStringAsync();
            var lines = text.Split( new[] { Environment.NewLine }, StringSplitOptions.None ).ToList();

            // defense; this should never happen
            if ( lines.Count == 0 )
                return content;

            var hasHostHeader = false;
            var hostKey = HostHeader + ":";
            var comparison = StringComparison.OrdinalIgnoreCase;
            string requestLine;

            // fix up request line if necessary
            if ( FixUpHttpRequest( lines[0], out requestLine ) )
            {
                // replace line one with fixed up request
                lines[0] = requestLine;

                // check if the host header is present
                hasHostHeader = lines.Any( l => l.StartsWith( hostKey, comparison ) );
            }
            else if ( lines.Count > 1 )
            {
                // if there's only one line, it can't have a host header; check
                // remaining content. if the header is present and we didn't
                // perform a fix up, then we can leave the content as is
                if ( hasHostHeader = lines.Any( l => l.StartsWith( hostKey, comparison ) ) )
                    return content;
            }

            // inject as necessary; order isn't really important here
            if ( !hasHostHeader )
                lines.Insert( 1, string.Format( CultureInfo.InvariantCulture, "{0}:{1}", HostHeader, host ) );

            // reassemble the content
            text = string.Join( Environment.NewLine, lines );
            content = new StringContent( text );
            content.Headers.ContentType = new MediaTypeHeaderValue( HttpMediaType )
            {
                Parameters =
                {
                    new NameValueHeaderValue( MessageTypeHeaderParameter, HttpRequestHeaderParameter )
                }
            };

            return content;
        }
    }
}
