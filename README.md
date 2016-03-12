#Music Mashup

REST API that simply provides a mashup of some underlying APIs. APIs that will be combined are MusicBrainz, Wikipedia and Cover Art Archive. 

* MusicBrainz provides an API, including detailed information about music artists (artist name, year of birth, country of birth, etc.). 
* Wikipedia is a community wiki that contains descriptive information, including the right music artists. 
* Cover Art Archive is a sister project to the MusicBrainz that includes cover images to different albums, singles, eps etc. released by an artist.

Music Mashup API receives an MBID (MusicBrainz Identifier) and returns data consisting of
* Description of the artist is taken from Wikipedia. Wikipedia does not contain any MBID without mapping between MBID and Wikipedia identifiers available through MusicBrainz API.
* List of all albums released by artist and links to images for each album. List of albums is available in MusicBrainz but pictures are on Cover Art Archive.

##External APIs
###MusicBrainz 
* Documentation: http://musicbrainz.org/doc/Development/XML_Web_Service/Version_2 
* URL: http://musicbrainz.org/ws/2 
* Example: http://musicbrainz.org/ws/2/artist/5b11f4ce-a62d-471e-81fca69a8278c7da?&fmt=json&inc=url-rels+release-groups

##Wikipedia
* Documentation: https://www.mediawiki.org/wiki/API:Main_page 
* URL: https://en.wikipedia.org/w/api.php 
* Example: https://en.wikipedia.org/w/api.php?action=query&format=json&prop=extracts&exintro=true&redirects=true&titles=Nirvana_(band)

Note: In response of MusicBrainz you will find an entity named *Relations*. Where type is *wikipedia*, you will find the string that can be used to lookup Wikipedia API i.e *Nirvana_(band)*

###Cover Art Archive
* Documentation: https://wiki.musicbrainz.org/Cover_Art_Archive/API
* URL: http://coverartarchive.org/
* Example: http://coverartarchive.org/release-group/1b022e01-4da6387b-8658-8678046e4cef

Note: In response of MusicBrainz you will find an entity named *release - groups*. It includes album's title (title) and its MBID (ID). This MBID is then used to query Cover Art Archive.

Since the underlying APIs can be quite slow, Music Mashup is designed to handle high loads through output caching, HTTP compression and Async requests where possible.

###Example response
```json
{"mbid" : "5b11f4ce-­a62d-­471e-­81fc-­a69a8278c7da",
 "description" : "<p><b>Nirvana</b> was an American rock band that was formed ... ",
 "albums" : [ { "title" : "Nevermind", "id": "1b022e01-­4da6-­387b-­8658-­8678046e4cef",
 "image": "http://coverartarchive.org/release/a146429a-­cedc-­3ab0-­9e41-­1aaf5f6cdc2d/3012495605.jpg" },
 { ... more albums... }]}
```
