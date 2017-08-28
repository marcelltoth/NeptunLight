namespace NeptunLight.Models
{
    public struct MessageLoadingProgress
    {
        public MessageLoadingProgress(int current, int total)
        {
            Current = current;
            Total = total;
        }

        public int Current { get; }

        public int Total { get; }
    }
}