using System.Diagnostics.CodeAnalysis;
using AutoInputPlus.Core.Constants;
using AutoInputPlus.Core.Metadata;

namespace AutoInputPlus.Core.Tests.Metadata;

/// <summary>
/// Tests for app metadata.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class AppMetadataTests
{
    /// <summary>
    /// Version should be read from Directory.Build.props when assembly lookup is disabled.
    /// </summary>
    [Fact]
    public void AppVersionShouldCorrectlyBeRetrievedFromProps()
    {
        using var testDirectory = CreateTestDirectoryWithProps(
            """
            <Project>
              <PropertyGroup>
                <Version>9.8.7</Version>
              </PropertyGroup>
            </Project>
            """);

        string actual = AppMetadata.GetVersion(testDirectory.SearchStartDirectory, preferAssembly: false);

        Assert.Equal("9.8.7", actual);
    }

    /// <summary>
    /// Version should remove build metadata and trailing revision when read from props.
    /// </summary>
    [Fact]
    public void AppVersionShouldBeCleanedForDisplay()
    {
        using var testDirectory = CreateTestDirectoryWithProps(
            """
            <Project>
              <PropertyGroup>
                <Version>1.2.3.0+abc123</Version>
              </PropertyGroup>
            </Project>
            """);

        string actual = AppMetadata.GetVersion(testDirectory.SearchStartDirectory, preferAssembly: false);

        Assert.Equal("1.2.3", actual);
    }

    /// <summary>
    /// Version should preserve prerelease text when read from props.
    /// </summary>
    [Fact]
    public void AppVersionShouldPreservePrerelease()
    {
        using var testDirectory = CreateTestDirectoryWithProps(
            """
            <Project>
              <PropertyGroup>
                <Version>2.5.0-beta.1+build45</Version>
              </PropertyGroup>
            </Project>
            """);

        string actual = AppMetadata.GetVersion(testDirectory.SearchStartDirectory, preferAssembly: false);

        Assert.Equal("2.5.0-beta.1", actual);
    }

    /// <summary>
    /// Metadata value should be read from Directory.Build.props.
    /// </summary>
    [Fact]
    public void GetValueShouldReturnPropertyValueFromProps()
    {
        using var testDirectory = CreateTestDirectoryWithProps(
            """
            <Project>
              <PropertyGroup>
                <Authors>Some Name</Authors>
              </PropertyGroup>
            </Project>
            """);

        string actual = AppMetadata.GetValue("Authors", testDirectory.SearchStartDirectory);

        Assert.Equal("Some Name", actual);
    }

    /// <summary>
    /// Metadata value should return fallback when property is missing.
    /// </summary>
    [Fact]
    public void GetValueShouldReturnFallbackWhenPropertyDoesNotExist()
    {
        using var testDirectory = CreateTestDirectoryWithProps(
            """
            <Project>
              <PropertyGroup>
                <Authors>Some Name</Authors>
              </PropertyGroup>
            </Project>
            """);

        string actual = AppMetadata.GetValue("RepositoryUrl", testDirectory.SearchStartDirectory);

        Assert.Equal(AppConstants.MetadataFallback, actual);
    }

    /// <summary>
    /// Metadata value should return fallback when property name is empty.
    /// </summary>
    [Fact]
    public void GetValueShouldReturnFallbackWhenPropertyNameIsEmpty()
    {
        using var testDirectory = CreateTestDirectoryWithProps(
            """
            <Project>
              <PropertyGroup>
                <Authors>Some Name</Authors>
              </PropertyGroup>
            </Project>
            """);

        string actual = AppMetadata.GetValue(string.Empty, testDirectory.SearchStartDirectory);

        Assert.Equal(AppConstants.MetadataFallback, actual);
    }

    private static TestDirectoryContext CreateTestDirectoryWithProps(string propsContent)
    {
        string rootPath = Path.Combine(Path.GetTempPath(), $"AppMetadataTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(rootPath);

        string nestedPath = Path.Combine(rootPath, "bin", "Debug", "net9.0");
        Directory.CreateDirectory(nestedPath);

        string propsPath = Path.Combine(rootPath, AppConstants.DirectoryBuildPropsFileName);
        File.WriteAllText(propsPath, propsContent);

        return new TestDirectoryContext(rootPath, nestedPath);
    }

    private sealed class TestDirectoryContext(string rootDirectory, string searchStartDirectory) : IDisposable
    {
        public string RootDirectory { get; } = rootDirectory;

        public string SearchStartDirectory { get; } = searchStartDirectory;

        public void Dispose()
        {
            if (Directory.Exists(RootDirectory))
                Directory.Delete(RootDirectory, recursive: true);
        }
    }
}