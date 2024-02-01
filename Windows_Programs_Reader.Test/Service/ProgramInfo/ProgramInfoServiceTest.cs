using Moq;
using System.Reflection;
using Windows_Programs_Reader.Data;
using Windows_Programs_Reader.Service.ProgramInfo;
using Windows_Programs_Reader.Service.RegJump;
using Windows_Programs_Reader.Test.Data.Faker;

namespace Windows_Programs_Reader.Test.Service.ProgramInfo;

public sealed class ProgramInfoServiceTest
{
    [Fact]
    public void UpdateFromDifferent_WhenTheyDiffer_Success()
    {
        var mockRegJumpService = new Mock<IRegJumpService>();
        var service = new ProgramInfoService(mockRegJumpService.Object);
        var originalProgramInfo = new ProgramInfoDataFaker().Generate();
        originalProgramInfo.InstallLocation = null;
        originalProgramInfo.Language = null;
        originalProgramInfo.Comments = null;
        originalProgramInfo.UrlInfoAbout = null;
        originalProgramInfo.DisplayIcon = null;
        var updateProgramInfo = new ProgramInfoDataFaker().Generate();

        service.UpdateFromDifferent(originalProgramInfo, updateProgramInfo);

        foreach (PropertyInfo property in typeof(ProgramInfoData).GetProperties())
        {
            var originalValue = property.GetValue(originalProgramInfo);
            var updatedValue = property.GetValue(updateProgramInfo);
            Assert.Equal(updatedValue, originalValue);
        }
    }

    [Fact]
    public void UpdateFromDifferent_WhenSecondNull_Success()
    {
        var mockRegJumpService = new Mock<IRegJumpService>();
        var service = new ProgramInfoService(mockRegJumpService.Object);
        var originalProgramInfo = new ProgramInfoDataFaker().Generate();
        var updateProgramInfo = new ProgramInfoData(service);

        service.UpdateFromDifferent(originalProgramInfo, updateProgramInfo);

        foreach (PropertyInfo property in typeof(ProgramInfoData).GetProperties())
        {
            var originalValue = property.GetValue(originalProgramInfo);
            Assert.NotNull(originalValue);
        }
    }
}
