using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MusicMashup.Responses;
using Newtonsoft.Json;
using WebApi.OutputCache.V2;

namespace MusicMashup.Controllers
{
    public class ArtistController : ApiController
    {
        static readonly string MusicBrainzUrl = ConfigurationManager.AppSettings["MusicBrainzUrl"];
        static readonly string WikipediaUrl = ConfigurationManager.AppSettings["WikipediaUrl"];
        static readonly string CoverArtArchiveUrl = ConfigurationManager.AppSettings["CoverArtArchiveUrl"];

        [CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100)]
        public async Task<object> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException("MBID");

            var musicBrainzResponse = await GetResponseAsync<MusicBrainz>(string.Format(MusicBrainzUrl, id));
            
            var albumsTask = GetAlbumsFromCoverArtArchiveAsync(musicBrainzResponse.ReleaseGroups);
            var wikipediaExtract = await GetExtractFromWikipediaAsync(musicBrainzResponse.ParseWikiArtistName());

            await albumsTask;

            return new {MbId = id, Description = wikipediaExtract, Albums = albumsTask.Result};
        }

        static async Task<string> GetExtractFromWikipediaAsync(string wikiArtistName)
        {
            var response = await GetResponseAsync<Wikipedia>(string.Format(WikipediaUrl, wikiArtistName));
            return response.ParseExtract();
        }

        static async Task<IEnumerable<object>> GetAlbumsFromCoverArtArchiveAsync(IEnumerable<ReleaseGroup> releaseGroups)
        {
            var getAlbumTasks = releaseGroups.Select(x => GetAlbumFromCoverArtArchiveAsync(x.Id, x.Title));
            return await Task.WhenAll(getAlbumTasks);
        }

        static async Task<object> GetAlbumFromCoverArtArchiveAsync(string id, string title)
        {
            try
            {
                var response = await GetResponseAsync<CoverArtArchive>(string.Format(CoverArtArchiveUrl, id));
                return new {Id = id, Image = response.ParseImageUrl(), Title = title};
            }
            catch (Exception e)
            {
                Debug.WriteLine($"MbId: {id}, Exception: {e}");
                return new {Id = id, Image = string.Empty, Title = title};
            }
        }

        static async Task<T> GetResponseAsync<T>(string url)
            where T : IResponse
        {
            using (var handler = CreateHandler())
            {
                using (var client = new HttpClient(handler))
                {
                    using (var httpResponse = await client.GetAsync(new Uri(url)))
                    {
                        httpResponse.EnsureSuccessStatusCode();

                        var stringResponse = await httpResponse.Content.ReadAsStringAsync();
                        var objectResponse = JsonConvert.DeserializeObject<T>(stringResponse);
                        objectResponse.EnsureValidResponse();

                        return objectResponse;
                    }
                }
            }
        }

        static HttpClientHandler CreateHandler()
        {
            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            return handler;
        }
    }
}
