using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.Client
{
    using System.ServiceModel;
    using Contract;

    internal sealed class AutoRecoveryClient : IInventoryServiceClient
    {
        private readonly InventoryServiceClientFactory factory;
        private readonly string endpointUri;
        private InventoryServiceClient internalClient;

        public AutoRecoveryClient(InventoryServiceClientFactory factory, string endpointUri)
        {
            this.factory = factory;
            this.endpointUri = endpointUri;
            this.internalClient = factory.CreateClient(endpointUri);
        }

        public async Task<CheckUpdateResult> CheckUpdateAsync()
        {
            var client = this.internalClient;
            try
            {
                return await client.CheckUpdateAsync().ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is CommunicationException || ex is TimeoutException)
            {
                client.Abort();
                this.internalClient = factory.CreateClient(endpointUri);

                return new CheckUpdateResult { ErrorCode = ErrorCode.Disconnected };
            }
        }

        public async Task<GetInventoryInfoResult> GetInventoryInfoAsync()
        {
            try
            {
                return await this.internalClient.GetInventoryInfoAsync().ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is CommunicationException || ex is TimeoutException)
            {
                this.internalClient.Abort();
                this.internalClient = factory.CreateClient(endpointUri);

                return new GetInventoryInfoResult() { ErrorCode = ErrorCode.Disconnected };
            }
        }

        public async Task<ProcessOrderResult> ProcessOrderAsync(OrderTransaction orderTransaction)
        {
            try
            {
                return await this.internalClient.ProcessOrderAsync(orderTransaction).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is CommunicationException || ex is TimeoutException)
            {
                this.internalClient.Abort();
                this.internalClient = factory.CreateClient(endpointUri);

                return new ProcessOrderResult() { ErrorCode = ErrorCode.Disconnected };
            }
        }

        public void Dispose()
        {
            try
            {
                if (this.internalClient.State != CommunicationState.Faulted)
                {
                    this.internalClient.Close();
                }
                else
                {
                    this.internalClient.Abort();
                }
            }
            catch (CommunicationException)
            {
                // Communication exceptions are normal when
                // closing the connection.
                this.internalClient.Abort();
            }
            catch (TimeoutException)
            {
                // Timeout exceptions are normal when closing
                // the connection.
                this.internalClient.Abort();
            }
            catch (Exception)
            {
                // Any other exception and you should 
                // abort the connection and rethrow to 
                // allow the exception to bubble upwards.
                this.internalClient.Abort();
                throw;
            }
        }
    }
}
