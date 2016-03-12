namespace MusicMashup.Responses
{
    public interface IResponse
    {
        /// <summary>
        /// Throws an Exception if response content is not parseable
        /// </summary>
        void EnsureValidResponse();
    }
}