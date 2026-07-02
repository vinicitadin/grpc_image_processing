using Google.Protobuf;
using Grpc.Core;
using SkiaSharp;

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

            using var stream = new MemoryStream(imageBytes);

            using var codec = SKCodec.Create(stream) ?? throw new InvalidOperationException("Imagem inválida.");

            var format = codec.EncodedFormat;

            stream.Position = 0;

            using var bitmap = SKBitmap.Decode(stream) ?? throw new InvalidOperationException("Não foi possível decodificar a imagem.");

            float inv = 1f - intensity;

            const float r = 0.2126f;
            const float g = 0.7152f;
            const float b = 0.0722f;

            float[] matrix =
            {
                inv + intensity * r, intensity * g,       intensity * b,       0, 0,
                intensity * r,       inv + intensity * g, intensity * b,       0, 0,
                intensity * r,       intensity * g,       inv + intensity * b, 0, 0,
                0,                   0,                   0,                   1, 0
            };

            using var paint = new SKPaint
            {
                ColorFilter = SKColorFilter.CreateColorMatrix(matrix)
            };

            var info = new SKImageInfo(bitmap.Width, bitmap.Height);

            using var surface = SKSurface.Create(info);

            surface.Canvas.Clear(SKColors.Transparent);

            surface.Canvas.DrawBitmap(bitmap, 0, 0, paint);

            using var image = surface.Snapshot();

            using var data = image.Encode(format, 100);

            return data.ToArray();
        }
    }
}
