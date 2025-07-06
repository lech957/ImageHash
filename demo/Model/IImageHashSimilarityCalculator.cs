namespace Demo.Model
{
    public interface IImageHashSimilarityCalculator
    {
        double Calculate(byte[] imageHash1, byte[] imageHash2);
    }
}
