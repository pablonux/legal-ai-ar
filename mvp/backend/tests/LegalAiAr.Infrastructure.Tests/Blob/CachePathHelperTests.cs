using LegalAiAr.Infrastructure.Blob;

namespace LegalAiAr.Infrastructure.Tests.Blob;

public class CachePathHelperTests
{
    [Theory]
    [InlineData(1, "8048522", "_cache/csjn/pdf/8048522.pdf")]
    [InlineData(2, "DOC001", "_cache/saij/pdf/DOC001.pdf")]
    [InlineData(3, "12345", "_cache/pjn/pdf/12345.pdf")]
    [InlineData(4, "ABC", "_cache/scba/pdf/ABC.pdf")]
    [InlineData(99, "X", "_cache/unknown/pdf/X.pdf")]
    public void PdfCacheKey_BuildsCorrectPath(int sourceId, string documentId, string expected)
    {
        var result = CachePathHelper.PdfCacheKey(sourceId, documentId);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1, "abrirAnalisis", "804852", "_cache/csjn/api/abrirAnalisis/804852.json")]
    [InlineData(1, "getCitas", "8048522", "_cache/csjn/api/getCitas/8048522.json")]
    [InlineData(1, "getCitantes", "8048522", "_cache/csjn/api/getCitantes/8048522.json")]
    [InlineData(1, "getAllDocumentos", "804852", "_cache/csjn/api/getAllDocumentos/804852.json")]
    [InlineData(1, "getSumariosAnalisis", "804852", "_cache/csjn/api/getSumariosAnalisis/804852.json")]
    [InlineData(2, "someEndpoint", "DOC001", "_cache/saij/api/someEndpoint/DOC001.json")]
    public void ApiCacheKey_BuildsCorrectPath(int sourceId, string endpoint, string id, string expected)
    {
        var result = CachePathHelper.ApiCacheKey(sourceId, endpoint, id);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void PdfCacheKey_PathDoesNotOverlapWithBlobPathHelper()
    {
        var cacheKey = CachePathHelper.PdfCacheKey(1, "8048522");
        var blobPath = BlobPathHelper.BuildPdfPath("ruling", 1, "8048522");

        Assert.StartsWith("_cache/", cacheKey);
        Assert.StartsWith("legal-ai-ar-kb/", blobPath);
        Assert.NotEqual(cacheKey, blobPath);
    }
}
