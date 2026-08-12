// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSystemListerService.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   A service to collect the file names below a directory and to write them to a file.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileSystemLister.Services;

/// <inheritdoc cref="IFileSystemListerService"/>
/// <summary>
/// A service to collect the file names below a directory and to write them to a file.
/// </summary>
/// <seealso cref="IFileSystemListerService"/>
public class FileSystemListerService : IFileSystemListerService
{
    /// <summary>
    /// The bulletin board code that opens the list.
    /// </summary>
    private const string ListStart = "[list]";

    /// <summary>
    /// The bulletin board code that closes the list.
    /// </summary>
    private const string ListEnd = "[/list]";

    /// <summary>
    /// The bulletin board code that marks a single list entry.
    /// </summary>
    private const string ListEntry = "[*]";

    /// <inheritdoc cref="IFileSystemListerService"/>
    /// <summary>
    /// Lists the names of all files below the given directory, subdirectories included.
    /// </summary>
    /// <param name="directory">The directory to start at.</param>
    /// <param name="useBulletinCode">A value indicating whether the output should be formatted as bulletin code or not.</param>
    /// <returns>The file names as <see cref="IList{T}"/> of <see cref="string"/>.</returns>
    /// <seealso cref="IFileSystemListerService"/>
    public IList<string> ListFileNames(string directory, bool useBulletinCode)
    {
        var fileNames = new List<string>();

        if (useBulletinCode)
        {
            fileNames.Add(ListStart);
        }

        // The given directory is read without a guard on purpose, a directory the user picked and
        // that cannot be read is an error the caller has to see. Everything below it is skipped
        // silently instead, otherwise a scan over a folder with protected subfolders never finishes.
        AddFileNamesOfDirectory(directory, useBulletinCode, fileNames);
        AddFileNamesOfSubDirectories(directory, useBulletinCode, fileNames);

        if (useBulletinCode)
        {
            fileNames.Add(ListEnd);
        }

        return fileNames;
    }

    /// <inheritdoc cref="IFileSystemListerService"/>
    /// <summary>
    /// Writes the given lines to the result file, replacing an already existing file.
    /// </summary>
    /// <param name="fileName">The name of the file to write.</param>
    /// <param name="lines">The lines to write.</param>
    /// <seealso cref="IFileSystemListerService"/>
    public void WriteResultFile(string fileName, IEnumerable<string> lines)
    {
        File.WriteAllLines(fileName, lines);
    }

    /// <summary>
    /// Adds the names of the files that lie directly in the given directory.
    /// </summary>
    /// <param name="directory">The directory to read.</param>
    /// <param name="useBulletinCode">A value indicating whether the output should be formatted as bulletin code or not.</param>
    /// <param name="fileNames">The list the names are added to.</param>
    private static void AddFileNamesOfDirectory(string directory, bool useBulletinCode, List<string> fileNames)
    {
        foreach (var file in Directory.EnumerateFiles(directory))
        {
            fileNames.Add(GetFileName(file, useBulletinCode));
        }
    }

    /// <summary>
    /// Adds the names of the files of all subdirectories of the given directory. A subdirectory that
    /// cannot be read is skipped, the scan continues with the next one.
    /// </summary>
    /// <param name="directory">The directory whose subdirectories are read.</param>
    /// <param name="useBulletinCode">A value indicating whether the output should be formatted as bulletin code or not.</param>
    /// <param name="fileNames">The list the names are added to.</param>
    private static void AddFileNamesOfSubDirectories(string directory, bool useBulletinCode, List<string> fileNames)
    {
        foreach (var subDirectory in Directory.EnumerateDirectories(directory))
        {
            try
            {
                AddFileNamesOfDirectory(subDirectory, useBulletinCode, fileNames);
                AddFileNamesOfSubDirectories(subDirectory, useBulletinCode, fileNames);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }

    /// <summary>
    /// Gets the name of the given file without its path, as bulletin code entry if asked for.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <param name="useBulletinCode">A value indicating whether the output should be formatted as bulletin code or not.</param>
    /// <returns>The file name as <see cref="string"/>.</returns>
    private static string GetFileName(string file, bool useBulletinCode)
    {
        return useBulletinCode
            ? $"{ListEntry}{Path.GetFileName(file)}"
            : Path.GetFileName(file);
    }
}
