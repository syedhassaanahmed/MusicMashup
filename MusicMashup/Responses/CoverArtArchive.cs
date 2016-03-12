using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicMashup.Responses
{
    public class CoverArtArchive : IResponse
    {
        public IList<Images> Images { get; set; }

        public void EnsureValidResponse()
        {
            if(Images == null || !Images.Any())
                throw new HttpParseException("CoverArtArchive response does not contain 'images'");

            Uri imageUri;
            if (!Uri.TryCreate(Images.First().Image, UriKind.Absolute, out imageUri))
                throw new HttpParseException("'image' does not contain valid uri in CoverArtArchive response");
        }

        public string ParseImageUrl()
        {
            return Images.First().Image;
        }
    }
    
    public class Images
    {
        public string Image { get; set; }
    }
}