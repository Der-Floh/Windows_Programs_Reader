using Moq;
using Windows_Programs_Reader.Data;
using Windows_Programs_Reader.Service.EmbeddedResource;
using Windows_Programs_Reader.Service.WindowsLanguage;
using Windows_Programs_Reader.Test.Data.Faker;

namespace Windows_Programs_Reader.Test.Service.WindowsLanguage;

public sealed class WindowsLanguageServiceTest
{
    [Fact]
    public void GetAll_LanguageLoaded_Success()
    {
        var numberOfLanguages = 2;

        var orgLanguagesList = new WindowsLanguageDataFaker().Generate(numberOfLanguages);
        var csvData = string.Empty;
        foreach (var language in orgLanguagesList)
        {
            csvData += language.ToCsv() + Environment.NewLine;
        }

        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(csvData);
        writer.Flush();
        stream.Position = 0;

        var mockEmbeddedResources = new Mock<IEmbeddedResourceService>();
        mockEmbeddedResources.Setup(m => m.GetResourceStream("languages.csv")).Returns(stream);

        var service = new WindowsLanguageService(mockEmbeddedResources.Object);

        var result = service.GetAll();
        Assert.NotNull(result);

        var newLanguagesList = new List<WindowsLanguageData>(result);
        Assert.Equal(numberOfLanguages, newLanguagesList.Count);
        for (var i = 0; i < numberOfLanguages; i++)
        {
            foreach (var property in typeof(WindowsLanguageData).GetProperties())
            {
                var orgValue = property.GetValue(orgLanguagesList[i]);
                var newValue = property.GetValue(newLanguagesList[i]);
                Assert.Equal(newValue, orgValue);
            }
        }
    }
}
