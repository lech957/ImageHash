namespace Demo.Model
{
    public interface IDemoImageHash
    {
        byte[] CalculateAverageHash(string filename);

        byte[] CalculateExtendedAverageHash(string filename);

        byte[] CalculateDifferenceHash(string filename);

        byte[] CalculatePerceptualHash(string filename);
    }
}
