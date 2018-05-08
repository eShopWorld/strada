using System;

namespace Eshopworld.Strada.Clients.Core
{
    public static class DataSegmentor
    {
        public static float CalculateDataSizeInKilobytes(string data)
        {
            var preciseDataSizeInKilobytes = data.Length / 1024f;
            return (int) Math.Ceiling(preciseDataSizeInKilobytes);
        }
    }
}