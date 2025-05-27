using System;

namespace Egzaminas.Services.Interfaces;

public interface IImageService
{
    Task<byte[]> ProcessProfilePicture(IFormFile file);
}
