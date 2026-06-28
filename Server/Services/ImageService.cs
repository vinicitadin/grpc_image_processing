using Google.Protobuf;
using Grpc.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

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
            if (request.Image.IsEmpty)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "A imagem é obrigatória."));
            }

            byte[] imageBytes = request.Image.ToByteArray();
            byte[] result;

            switch (request.FilterCase)
            {
                case ApplyFilterRequest.FilterOneofCase.GrayFilter:

                    if (request.GrayFilter.Intensity < 0 || request.GrayFilter.Intensity > 100)
                    {
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "A intensidade deve estar entre 0 e 100."));
                    }
                    _logger.LogInformation("Aplicando filtro cinza");
                    result = ApplyGrayScale(imageBytes, request.GrayFilter.Intensity);
                    break;

                case ApplyFilterRequest.FilterOneofCase.None:
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Nenhum filtro foi informado."));
                
                default:
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Filtro não suportado."));
            }

            return Task.FromResult(new ApplyFilterResponse
            {
                Image = ByteString.CopyFrom(result)
            });
        }

        public static byte[] ApplyGrayScale(byte[] imageBytes, float intensity)
        {
            intensity = Math.Clamp(intensity / 100f, 0f, 1f);

            using Image<Rgba32> image = Image.Load<Rgba32>(imageBytes);

            image.Mutate(x => x.Grayscale(intensity));

            using MemoryStream ms = new MemoryStream();

            image.SaveAsPng(ms);

            return ms.ToArray();
        }
    }
}
