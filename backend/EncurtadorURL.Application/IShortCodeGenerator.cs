namespace EncurtadorURL.Application.Interfaces
{
    public interface IShortCodeGenerator
    {
        string Generate(int uniqueId);
    }
}
