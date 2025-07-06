namespace Demo.Model
{
    public class ImageHashSimilarityCalculator : IImageHashSimilarityCalculator
    {
        public double Calculate(byte[] imageHash1, byte[] imageHash2)
        {
            return CoenM.ImageHash.CompareHash.Similarity(imageHash1, imageHash2);
        }
    }
}
