namespace Eshopworld.Strada.Clients.Core
{
    public static class DataSegmentor
    {
        public static float CalculateSizeInKilobytes(string data)
        {
            return data.Length / 1024f;
        }
    }
}