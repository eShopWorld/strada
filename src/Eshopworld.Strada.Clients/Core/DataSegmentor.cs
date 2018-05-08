namespace Eshopworld.Strada.Clients.Core
{
    public static class DataSegmentor
    {
        public static float CalculateDataSizeInKilobytes(string data)
        {
            return data.Length / 1024f;
        }
    }
}