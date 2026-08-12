// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSystemListerServiceTests.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   A class to test the <see cref="FileSystemListerService" /> class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileSystemLister.Tests;

/// <summary>
/// A class to test the <see cref="FileSystemListerService"/> class.
/// </summary>
[TestClass]
public class FileSystemListerServiceTests
{
    /// <summary>
    /// The service under test.
    /// </summary>
    private readonly IFileSystemListerService fileSystemListerService = new FileSystemListerService();

    /// <summary>
    /// The directory the files of a single test are created in.
    /// </summary>
    private string testDirectory = string.Empty;

    /// <summary>
    /// Creates an empty directory outside of the repository for the files of the running test.
    /// </summary>
    [TestInitialize]
    public void CreateTestDirectory()
    {
        this.testDirectory = Path.Combine(Path.GetTempPath(), $"FileSystemLister_{Guid.NewGuid():N}");
        Directory.CreateDirectory(this.testDirectory);
    }

    /// <summary>
    /// Removes the directory of the finished test.
    /// </summary>
    [TestCleanup]
    public void DeleteTestDirectory()
    {
        if (Directory.Exists(this.testDirectory))
        {
            Directory.Delete(this.testDirectory, true);
        }
    }

    /// <summary>
    /// Checks whether the file names are returned without their path, which is what the program is for.
    /// </summary>
    [TestMethod]
    public void ListFileNamesReturnsTheFileNamesWithoutTheirPath()
    {
        this.CreateFile("First.txt");
        this.CreateFile("Second.txt");

        var fileNames = this.fileSystemListerService.ListFileNames(this.testDirectory, false);

        CollectionAssert.AreEquivalent(new[] { "First.txt", "Second.txt" }, fileNames.ToArray());
    }

    /// <summary>
    /// Checks whether the subdirectories are walked as well, not only the directory that was given.
    /// </summary>
    [TestMethod]
    public void ListFileNamesFindsTheFilesOfTheSubDirectories()
    {
        this.CreateFile("Root.txt");
        this.CreateFile(Path.Combine("Sub", "Child.txt"));
        this.CreateFile(Path.Combine("Sub", "Deeper", "GrandChild.txt"));

        var fileNames = this.fileSystemListerService.ListFileNames(this.testDirectory, false);

        CollectionAssert.AreEquivalent(new[] { "Root.txt", "Child.txt", "GrandChild.txt" }, fileNames.ToArray());
    }

    /// <summary>
    /// Checks whether a directory is written before its subdirectories, so that the result file follows
    /// the structure of the scanned folder instead of jumping around.
    /// </summary>
    [TestMethod]
    public void ListFileNamesReturnsTheFilesOfADirectoryBeforeTheFilesOfItsSubDirectories()
    {
        this.CreateFile("Root.txt");
        this.CreateFile(Path.Combine("Sub", "Child.txt"));

        var fileNames = this.fileSystemListerService.ListFileNames(this.testDirectory, false);

        Assert.IsTrue(fileNames.IndexOf("Root.txt") < fileNames.IndexOf("Child.txt"), "The file of the root directory has to come first.");
    }

    /// <summary>
    /// Checks whether two files of the same name in different directories both end up in the result,
    /// because only the name is kept and nothing is deduplicated.
    /// </summary>
    [TestMethod]
    public void ListFileNamesReturnsTheSameNameTwiceForTwoDirectories()
    {
        this.CreateFile("Same.txt");
        this.CreateFile(Path.Combine("Sub", "Same.txt"));

        var fileNames = this.fileSystemListerService.ListFileNames(this.testDirectory, false);

        CollectionAssert.AreEquivalent(new[] { "Same.txt", "Same.txt" }, fileNames.ToArray());
    }

    /// <summary>
    /// Checks whether the bulletin board code wraps the whole list and marks every single entry.
    /// </summary>
    [TestMethod]
    public void ListFileNamesWithBulletinCodeWrapsTheListAndMarksEveryEntry()
    {
        this.CreateFile("Root.txt");
        this.CreateFile(Path.Combine("Sub", "Child.txt"));

        var fileNames = this.fileSystemListerService.ListFileNames(this.testDirectory, true);

        Assert.AreEqual(4, fileNames.Count);
        Assert.AreEqual("[list]", fileNames[0]);
        Assert.AreEqual("[/list]", fileNames[3]);
        CollectionAssert.Contains(fileNames.ToArray(), "[*]Root.txt");
        CollectionAssert.Contains(fileNames.ToArray(), "[*]Child.txt");
    }

    /// <summary>
    /// Checks whether nothing at all is added to the names without the bulletin board code.
    /// </summary>
    [TestMethod]
    public void ListFileNamesWithoutBulletinCodeWritesNoMarkup()
    {
        this.CreateFile("Root.txt");

        var fileNames = this.fileSystemListerService.ListFileNames(this.testDirectory, false);

        CollectionAssert.AreEqual(new[] { "Root.txt" }, fileNames.ToArray());
    }

    /// <summary>
    /// Checks whether an empty directory results in an empty list instead of a null reference.
    /// </summary>
    [TestMethod]
    public void ListFileNamesOfAnEmptyDirectoryReturnsAnEmptyList()
    {
        var fileNames = this.fileSystemListerService.ListFileNames(this.testDirectory, false);

        Assert.AreEqual(0, fileNames.Count);
    }

    /// <summary>
    /// Checks whether an empty directory scanned with the bulletin board code returns the wrapper only.
    /// </summary>
    [TestMethod]
    public void ListFileNamesOfAnEmptyDirectoryWithBulletinCodeReturnsOnlyTheWrapper()
    {
        var fileNames = this.fileSystemListerService.ListFileNames(this.testDirectory, true);

        CollectionAssert.AreEqual(new[] { "[list]", "[/list]" }, fileNames.ToArray());
    }

    /// <summary>
    /// Checks whether a directory that does not exist is reported instead of returning an empty list.
    /// Only the directory the user picked behaves that way, subdirectories that cannot be read are skipped.
    /// </summary>
    [TestMethod]
    public void ListFileNamesOfAMissingDirectoryThrowsADirectoryNotFoundException()
    {
        var missingDirectory = Path.Combine(this.testDirectory, "DoesNotExist");

        Assert.ThrowsExactly<DirectoryNotFoundException>(() => this.fileSystemListerService.ListFileNames(missingDirectory, false));
    }

    /// <summary>
    /// Checks whether every line reaches the result file.
    /// </summary>
    [TestMethod]
    public void WriteResultFileWritesEveryLine()
    {
        var resultFile = Path.Combine(this.testDirectory, "Result.txt");

        this.fileSystemListerService.WriteResultFile(resultFile, new[] { "[list]", "[*]Root.txt", "[/list]" });

        CollectionAssert.AreEqual(new[] { "[list]", "[*]Root.txt", "[/list]" }, File.ReadAllLines(resultFile));
    }

    /// <summary>
    /// Checks whether a second scan replaces the content of the result file instead of appending to it.
    /// </summary>
    [TestMethod]
    public void WriteResultFileReplacesAnExistingFile()
    {
        var resultFile = Path.Combine(this.testDirectory, "Result.txt");
        this.fileSystemListerService.WriteResultFile(resultFile, new[] { "First.txt", "Second.txt" });

        this.fileSystemListerService.WriteResultFile(resultFile, new[] { "Third.txt" });

        CollectionAssert.AreEqual(new[] { "Third.txt" }, File.ReadAllLines(resultFile));
    }

    /// <summary>
    /// Creates an empty file below the directory of the running test, including the directories on its way.
    /// </summary>
    /// <param name="relativePath">The path of the file relative to the test directory.</param>
    private void CreateFile(string relativePath)
    {
        var fileName = Path.Combine(this.testDirectory, relativePath);
        var directory = Path.GetDirectoryName(fileName);

        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(fileName, string.Empty);
    }
}
