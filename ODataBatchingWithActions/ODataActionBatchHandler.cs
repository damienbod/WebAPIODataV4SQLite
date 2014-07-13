namespace Microsoft.Web.Http.OData.Samples
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Batch;
    using System.Web.Http.OData.Batch;

    /// <summary>
    /// Represents an OData batch handler that also supports invoking OData actions.
    /// </summary>
    public class ODataActionBatchHandler : DefaultODataBatchHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ODataActionBatchHandler"/> class.
        /// </summary>
        /// <param name="httpServer">The <see cref="HttpServer">HTTP server</see> that invokes batch requests.</param>
        public ODataActionBatchHandler( HttpServer httpServer )
            : base( httpServer )
        {
            Contract.Requires( httpServer != null );
        }

        private static async Task SegregateODataActionsAsync(
            HttpRequestMessage request,
            IEnumerable<HttpContent> contents,
            ICollection<ODataBatchRequestItem> requestItems,
            ICollection<HttpContent> normalContents,
            CancellationToken cancellationToken )
        {
            Contract.Requires( request != null );
            Contract.Requires( contents != null );
            Contract.Requires( requestItems != null );
            Contract.Requires( normalContents != null );
            Contract.Ensures( Contract.Result<Task>() != null );

            var host = request.Headers.Host;
            var config = request.GetConfiguration();

            foreach ( var content in contents )
            {
                if ( content.IsMimeMultipartContent() )
                {
                    // the content is a change set; add to 'normal' content
                    normalContents.Add( content );

                    // add a placeholder to reinsert the item to maintain execution order
                    requestItems.Add( null );
                }
                else
                {
                    // normalize content and read as nested request
                    var normalizedContent = await content.NormalizeNestedHttpMessageContentAsync( host );
                    var nestedRequest = await normalizedContent.ReadAsHttpRequestMessageAsync( cancellationToken );

                    nestedRequest.CopyBatchRequestProperties( request );
                    nestedRequest.SetConfiguration( config );

                    if ( nestedRequest.IsODataAction() )
                    {
                        // enqueue operation for normal action execution
                        requestItems.Add( new OperationRequestItem( nestedRequest ) );
                    }
                    else
                    {
                        // the content is a query; add to 'normal' content
                        normalContents.Add( content );

                        // add a placeholder to reinsert the item to maintain execution order
                        requestItems.Add( null );
                    }
                }
            }
        }

        private async Task MergeBaseODataRequestItems(
            HttpRequestMessage request,
            IList<ODataBatchRequestItem> requestItems,
            IEnumerable<HttpContent> normalContents,
            CancellationToken cancellationToken )
        {
            Contract.Requires( request != null );
            Contract.Requires( requestItems != null );
            Contract.Requires( normalContents != null );
            Contract.Ensures( Contract.Result<Task>() != null );

            if ( !normalContents.Any() )
                return;

            // rebuild content without actions
            var parameters = request.Content.Headers.ContentType.Parameters;
            var boundary = parameters.Single( p => p.Name.Equals( "boundary", StringComparison.OrdinalIgnoreCase ) ).Value.Trim( '"' );
            var content = new MultipartContent( "mixed", boundary );

            // the base implementation will not allow the parameter "msgtype=request"
            foreach ( var nestedContent in normalContents )
            {
                // do not clear content type for change sets
                if ( !nestedContent.IsMimeMultipartContent() )
                    nestedContent.Headers.ContentType.Parameters.Clear();

                content.Add( nestedContent );
            }

            // replace content
            request.Content = content;

            // get the items the base implementation would normally produce
            var normalRequestItems = await base.ParseBatchRequestsAsync( request, cancellationToken );

            // sanity check
            Debug.Assert( normalRequestItems.Count == requestItems.Count( i => i == null ), string.Format( "The parsed {0} request items, but expected {1} request items.", normalRequestItems.Count, requestItems.Count( i => i == null ) ) );

            // fill in the normal items to preserve execution order
            for ( int i = 0, j = 0; i < normalRequestItems.Count; i++, j++ )
            {
                // find the next null item
                while ( requestItems[j] != null )
                    ++j;

                // fill in the placeholder
                requestItems[j] = normalRequestItems[i];
            }
        }

        /// <summary>
        /// Parses the specified HTTP request and returns a list of requests to execute.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequestMessage">HTTP request</see> representing the batched request to parse.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken">cancellation token</see> used to cancel asynchronous processing.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing a <see cref="IList{T}">list</see> of the parsed <see cref="ODataBatchRequestItem">request items</see>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "The framework will never provide a null request." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "The framework will never provide a null cancellation token." )]
        public async override Task<IList<ODataBatchRequestItem>> ParseBatchRequestsAsync( HttpRequestMessage request, CancellationToken cancellationToken )
        {
            Debug.Assert( request != null, "The request should not be null." );
            Debug.Assert( cancellationToken != null, "The cancellation token should not be null." );

            // if the request is not multipart content, punt to the base implementation
            if ( !request.Content.IsMimeMultipartContent() )
                return await base.ParseBatchRequestsAsync( request, cancellationToken );

            var provider = await request.Content.ReadAsMultipartAsync( cancellationToken );
            var requestItems = new List<ODataBatchRequestItem>();
            var getOrChangeSetContent = new List<HttpContent>();

            // split content into action and normal buckets
            await SegregateODataActionsAsync( request, provider.Contents, requestItems, getOrChangeSetContent, cancellationToken );

            // execute base implementation to get default request items and then remerge them with actions
            await this.MergeBaseODataRequestItems( request, requestItems, getOrChangeSetContent, cancellationToken );

            return requestItems;
        }
    }
}
