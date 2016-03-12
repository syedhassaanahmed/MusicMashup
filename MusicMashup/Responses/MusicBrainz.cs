using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace MusicMashup.Responses
{
    public class MusicBrainz : IResponse
    {
        public IList<Relation> Relations { get; set; }

        [JsonProperty("release-groups")]
        public IList<ReleaseGroup> ReleaseGroups { get; set; }

        public void EnsureValidResponse()
        {
            if(Relations == null || !Relations.Any())
                throw new HttpParseException("MusicBrainz response does not contain 'relations'");

            var wikipediaRelation = Relations.FirstOrDefault(x => x.Type.Equals("wikipedia", StringComparison.OrdinalIgnoreCase));

            if (wikipediaRelation == null)
                throw new HttpParseException("'relation' with 'type' 'wikipedia' was not found");

            if (wikipediaRelation.Url == null)
                throw new HttpParseException("'relation' with 'type' 'wikipedia' does not contain 'url'");

            if (wikipediaRelation.Url.Resource == null)
                throw new HttpParseException("'relation' with 'type' 'wikipedia' does not contain 'url.resource'");

            Uri resourceUri;
            if (!Uri.TryCreate(wikipediaRelation.Url.Resource, UriKind.Absolute, out resourceUri))
                throw new HttpParseException("'relation' with 'type' 'wikipedia' does not contain valid uri in 'url.resource'");

            if(ReleaseGroups == null || !ReleaseGroups.Any())
                throw new HttpParseException("MusicBrainz response does not contain 'release-groups'");
        }

        public string ParseWikiArtistName()
        {
            var wikipediaRelation = Relations.First(x => x.Type.Equals("wikipedia", StringComparison.OrdinalIgnoreCase));

            var resource = wikipediaRelation.Url.Resource;
            var lastSlashIndex = resource.LastIndexOf("/", StringComparison.Ordinal);
            return resource.Substring(lastSlashIndex + 1);
        }
    }

    public class Relation
    {
        public string Type { get; set; }
        public Url Url { get; set; }
    }

    public class Url
    {
        public string Resource { get; set; }
    }

    public class ReleaseGroup
    {
        public string Title { get; set; }
        public string Id { get; set; }
    }
}