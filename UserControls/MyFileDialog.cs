using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaModel.UserControls
{
	class MyFileDialog : UserControl
	{
		public string selectedPath = string.Empty;
		public async Task<Tuple<byte[], DateTime, string>?> OpenFileButton_Clicked(Control control)
		{
			var topLevel = TopLevel.GetTopLevel(control);

			var fileType = new FilePickerFileType("Comma Delimited File")
			{
				Patterns = ["*.csv"],
				AppleUniformTypeIdentifiers = ["public.plain-text"],
				MimeTypes = ["text/plain"]
			};


			var filters = new List<FilePickerFileType>
			{
				fileType
			};

			// Start async operation to open the dialog.
			var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
			{
				Title = "Open File",
				AllowMultiple = false,
				FileTypeFilter = filters
			});
			
			if (files.Count >= 1)
			{
				DateTime creationTime = File.GetCreationTime(files[0].Path.LocalPath);
				selectedPath = files[0].Path.LocalPath;
				// Open reading stream from the first file.
				await using var stream = await files[0].OpenReadAsync();
				using var streamReader = new StreamReader(stream);
				using var memoryStream = new MemoryStream();

				await stream.CopyToAsync(memoryStream);
				return new Tuple<byte[], DateTime, string>(memoryStream.ToArray(), creationTime, selectedPath);
			}
			return null;
		}

		public async Task<List<Tuple<byte[], DateTime, string>>> OpenFolderButton_Clicked(Control control)
		{
			// Get top level from the current control. Alternatively, you can use Window reference instead.
			var topLevel = TopLevel.GetTopLevel(control);

			// Start async operation to open the dialog.
			var folder = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
			{
				Title = "Open folder",
				AllowMultiple = false

			});

			List<Tuple<byte[], DateTime, string>> files = new List<Tuple<byte[], DateTime, string>>();

			if (folder.Count >= 1)
			{
				var filesPath = Directory.GetFiles(folder[0].Path.LocalPath);

				selectedPath = folder[0].Path.LocalPath;
				var tasks = new List<Task>();
				foreach (var path in filesPath)
				{
					tasks.Add(Task.Run(async () =>
					{
						byte[] data = await File.ReadAllBytesAsync(path);
						DateTime creationTime = File.GetCreationTime(path);
						files.Add(new Tuple<byte[], DateTime, string>(data, creationTime, path));
					}));
				}

				await Task.WhenAll(tasks);

				return files;

			}
			return new List<Tuple<byte[], DateTime, string>>();
		}

		public async void SaveFileButton_Clicked(Control control)
		{
			// Get top level from the current control. Alternatively, you can use Window reference instead.
			var topLevel = TopLevel.GetTopLevel(control);

			// Start async operation to open the dialog.
			var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
			{
				Title = "Save File"
			});

			if (file is not null)
			{
				// Open writing stream from the file.
				await using var stream = await file.OpenWriteAsync();
				using var streamWriter = new StreamWriter(stream);
			}
		}
	}
}
