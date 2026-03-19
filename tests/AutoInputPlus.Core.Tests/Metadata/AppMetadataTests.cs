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

    /// <summary>
    /// Version should return fallback when Directory.Build.props does not exist and assembly lookup is disabled.
    /// </summary>
    [Fact]
    public void GetVersionShouldReturnFallbackWhenPropsDoesNotExist()
    {
        using var testDirectory = CreateEmptyTestDirectory();

        string actual = AppMetadata.GetVersion(testDirectory.SearchStartDirectory, preferAssembly: false);

        Assert.Equal(AppConstants.MetadataFallback, actual);
    }

    /// <summary>
    /// Metadata value should return fallback when Directory.Build.props does not exist.
    /// </summary>
    [Fact]
    public void GetValueShouldReturnFallbackWhenPropsDoesNotExist()
    {
        using var testDirectory = CreateEmptyTestDirectory();

        string actual = AppMetadata.GetValue("Authors", testDirectory.SearchStartDirectory);

        Assert.Equal(AppConstants.MetadataFallback, actual);
    }

    /// <summary>
    /// Metadata lookup should walk parent directories until Directory.Build.props is found.
    /// </summary>
    [Fact]
    public void GetValueShouldWalkUpDirectoryTree()
    {
        using var testDirectory = CreateTestDirectoryWithProps(
            """
            <Project>
              <PropertyGroup>
                <Product>AutoInput Plus</Product>
              </PropertyGroup>
            </Project>
            """);

        string deepPath = Path.Combine(
            testDirectory.SearchStartDirectory,
            "deeper",
            "child",
            "folder");

        Directory.CreateDirectory(deepPath);

        string actual = AppMetadata.GetValue("Product", deepPath);

        Assert.Equal("AutoInput Plus", actual);
    }

    /// <summary>
    /// Metadata value should trim surrounding whitespace from props values.
    /// </summary>
    [Fact]
    public void GetValueShouldTrimWhitespaceFromPropertyValue()
    {
        using var testDirectory = CreateTestDirectoryWithProps(
            """
            <Project>
              <PropertyGroup>
                <Authors>
                    Some Name
                </Authors>
              </PropertyGroup>
            </Project>
            """);

        string actual = AppMetadata.GetValue("Authors", testDirectory.SearchStartDirectory);

        Assert.Equal("Some Name", actual);
    }

    /// <summary>
    /// Version should return fallback when version element is blank.
    /// </summary>
    [Fact]
    public void GetVersionShouldReturnFallbackWhenVersionIsBlank()
    {
        using var testDirectory = CreateTestDirectoryWithProps(
            """
            <Project>
              <PropertyGroup>
                <Version>   </Version>
              </PropertyGroup>
            </Project>
            """);

        string actual = AppMetadata.GetVersion(testDirectory.SearchStartDirectory, preferAssembly: false);

        Assert.Equal(AppConstants.MetadataFallback, actual);
    }

    /// <summary>
    /// Version should preserve a non-zero fourth version part.
    /// </summary>
    [Fact]
    public void GetVersionShouldPreserveNonZeroRevision()
    {
        using var testDirectory = CreateTestDirectoryWithProps(
            """
            <Project>
              <PropertyGroup>
                <Version>1.2.3.4</Version>
              </PropertyGroup>
            </Project>
            """);

        string actual = AppMetadata.GetVersion(testDirectory.SearchStartDirectory, preferAssembly: false);

        Assert.Equal("1.2.3.4", actual);
    }

    /// <summary>
    /// Version should remove build metadata even when no prerelease exists.
    /// </summary>
    [Fact]
    public void GetVersionShouldRemoveBuildMetadata()
    {
        using var testDirectory = CreateTestDirectoryWithProps(
            """
            <Project>
              <PropertyGroup>
                <Version>3.4.5+sha999</Version>
              </PropertyGroup>
            </Project>
            """);

        string actual = AppMetadata.GetVersion(testDirectory.SearchStartDirectory, preferAssembly: false);

        Assert.Equal("3.4.5", actual);
    }

    /// <summary>
    /// First matching property group value should be returned.
    /// </summary>
    [Fact]
    public void GetValueShouldReturnFirstMatchingPropertyGroupValue()
    {
        using var testDirectory = CreateTestDirectoryWithProps(
            """
            <Project>
              <PropertyGroup>
                <Authors>First Author</Authors>
              </PropertyGroup>
              <PropertyGroup>
                <Authors>Second Author</Authors>
              </PropertyGroup>
            </Project>
            """);

        string actual = AppMetadata.GetValue("Authors", testDirectory.SearchStartDirectory);

        Assert.Equal("First Author", actual);
    }

    /// <summary>
    /// Metadata value should return fallback when props XML is invalid.
    /// </summary>
    [Fact]
    public void GetValueShouldReturnFallbackWhenPropsXmlIsInvalid()
    {
        using var testDirectory = CreateTestDirectoryWithProps("<Project><PropertyGroup><Authors>Broken");

        string actual = AppMetadata.GetValue("Authors", testDirectory.SearchStartDirectory);

        Assert.Equal(AppConstants.MetadataFallback, actual);
    }

    /// <summary>
    /// Version should return fallback when props XML is invalid.
    /// </summary>
    [Fact]
    public void GetVersionShouldReturnFallbackWhenPropsXmlIsInvalid()
    {
        using var testDirectory = CreateTestDirectoryWithProps("<Project><PropertyGroup><Version>Broken");

        string actual = AppMetadata.GetVersion(testDirectory.SearchStartDirectory, preferAssembly: false);

        Assert.Equal(AppConstants.MetadataFallback, actual);
    }

    /// <summary>
    /// Version should be retrieved from assembly when preferAssembly is true.
    /// </summary>
    [Fact]
    public void GetVersionShouldReturnAssemblyVersionWhenPreferred()
    {
        string actual = AppMetadata.GetVersion(AppContext.BaseDirectory, preferAssembly: true);

        Assert.False(string.IsNullOrWhiteSpace(actual));
        Assert.NotEqual(AppConstants.MetadataFallback, actual);
    }

    /// <summary>
    /// Metadata value should return fallback when property name is whitespace.
    /// </summary>
    [Fact]
    public void GetValueShouldReturnFallbackWhenPropertyNameIsWhitespace()
    {
        using var testDirectory = CreateTestDirectoryWithProps(
            """
        <Project>
          <PropertyGroup>
            <Authors>Some Name</Authors>
          </PropertyGroup>
        </Project>
        """);

        string actual = AppMetadata.GetValue("   ", testDirectory.SearchStartDirectory);

        Assert.Equal(AppConstants.MetadataFallback, actual);
    }

    /// <summary>
    /// Metadata value should return fallback when property value is whitespace.
    /// </summary>
    [Fact]
    public void GetValueShouldReturnFallbackWhenPropertyValueIsWhitespace()
    {
        using var testDirectory = CreateTestDirectoryWithProps(
            """
        <Project>
          <PropertyGroup>
            <Authors>   </Authors>
          </PropertyGroup>
        </Project>
        """);

        string actual = AppMetadata.GetValue("Authors", testDirectory.SearchStartDirectory);

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

    private static TestDirectoryContext CreateEmptyTestDirectory()
    {
        string rootPath = Path.Combine(Path.GetTempPath(), $"AppMetadataTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(rootPath);

        string nestedPath = Path.Combine(rootPath, "bin", "Debug", "net9.0");
        Directory.CreateDirectory(nestedPath);

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