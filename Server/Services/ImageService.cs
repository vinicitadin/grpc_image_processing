using Grpc.Core;
using Server;

namespace Server.Services
{
    public class ImageService : Server.ImageService.ImageServiceBase
    {
        private readonly ILogger<ImageService> _logger;
        
        public ImageService(ILogger<ImageService> logger)
        {
            _logger = logger;
        }

        public override Task<ApplyFilterResponse> ApplyFilter(ApplyFilterRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Aplicando filtro do tipo {request.FilterCase}");

            return Task.FromResult(new ApplyFilterResponse
            {
                Image = request.Image
            });
        }
    }
}
