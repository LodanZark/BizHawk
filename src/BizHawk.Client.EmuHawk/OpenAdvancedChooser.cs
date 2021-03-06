﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using BizHawk.Emulation.Cores.Libretro;
using BizHawk.Client.Common;

// these match strings from OpenAdvance. should we make them constants in there?
namespace BizHawk.Client.EmuHawk
{
	/// <summary>
	/// The Advanced ROM Loader type in MainForm/RomLoader/OpenAdvancedChooser
	/// </summary>
	public enum AdvancedRomLoaderType
	{
		None,
		LibretroLaunchNoGame,
		LibretroLaunchGame,
		ClassicLaunchGame,
		MameLaunchGame
	}

	public partial class OpenAdvancedChooser : Form
	{
		private readonly MainForm _mainForm;
		private readonly Config _config;

		public AdvancedRomLoaderType Result;
		public string SuggestedExtensionFilter;
		private RetroDescription _currentDescription;

		public OpenAdvancedChooser(MainForm mainForm, Config config)
		{
			_mainForm = mainForm;
			_config = config;

			InitializeComponent();

			RefreshLibretroCore(true);
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void btnSetLibretroCore_Click(object sender, EventArgs e)
		{
			if(_mainForm.RunLibretroCoreChooser())
				RefreshLibretroCore(false);
		}

		private void RefreshLibretroCore(bool bootstrap)
		{
			txtLibretroCore.Text = "";
			btnLibretroLaunchNoGame.Enabled = false;
			btnLibretroLaunchGame.Enabled = false;

			var core = _config.LibretroCore;
			if (string.IsNullOrEmpty(core))
			{
				return;
			}

			txtLibretroCore.Text = core;
			_currentDescription = null;

			// scan the current libretro core to see if it can be launched with NoGame,and other stuff
			try
			{
				var coreComm = _mainForm.CreateCoreComm();
				using var retro = new LibretroCore(coreComm, GlobalWin.Game, core);
				btnLibretroLaunchGame.Enabled = true;
				if (retro.Description.SupportsNoGame)
					btnLibretroLaunchNoGame.Enabled = true;

				//print descriptive information
				var descr = retro.Description;
				_currentDescription = descr;
				Console.WriteLine($"core name: {descr.LibraryName} version {descr.LibraryVersion}");
				Console.WriteLine($"extensions: {descr.ValidExtensions}");
				Console.WriteLine($"{nameof(descr.NeedsRomAsPath)}: {descr.NeedsRomAsPath}");
				Console.WriteLine($"{nameof(descr.NeedsArchives)}: {descr.NeedsArchives}");
				Console.WriteLine($"{nameof(descr.SupportsNoGame)}: {descr.SupportsNoGame}");
					
				foreach (var v in descr.Variables.Values)
					Console.WriteLine(v);
			}
			catch (Exception ex)
			{
				if (!bootstrap)
				{
					MessageBox.Show($"Couldn't load the selected Libretro core for analysis. It won't be available.\n\nError:\n\n{ex}");
				}
			}
		}

		private void btnLibretroLaunchGame_Click(object sender, EventArgs e)
		{
			var entries = new List<FilesystemFilter> { new FilesystemFilter("ROMs", _currentDescription.ValidExtensions.Split('|')) };
			if (!_currentDescription.NeedsArchives) entries.Add(FilesystemFilter.Archives); // "needs archives" means the relevant archive extensions are already in the list, and we shouldn't scan archives for roms
			SuggestedExtensionFilter = new FilesystemFilterSet(entries.ToArray()).ToString();
			Result = AdvancedRomLoaderType.LibretroLaunchGame;
			DialogResult = DialogResult.OK;
			Close();
		}

		private void btnMAMELaunchGame_Click(object sender, EventArgs e)
		{
			Result = AdvancedRomLoaderType.MameLaunchGame;
			DialogResult = DialogResult.OK;
			Close();
		}

		private void btnClassicLaunchGame_Click(object sender, EventArgs e)
		{
			Result = AdvancedRomLoaderType.ClassicLaunchGame;
			DialogResult = DialogResult.OK;
			Close();
		}

		private void btnLibretroLaunchNoGame_Click(object sender, EventArgs e)
		{
			Result = AdvancedRomLoaderType.LibretroLaunchNoGame;
			DialogResult = DialogResult.OK;
			Close();
		}

		private void txtLibretroCore_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				var filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
				if (Path.GetExtension(filePaths[0]).ToUpper() == ".DLL")
				{
					e.Effect = DragDropEffects.Copy;
					return;
				}
			}

			e.Effect = DragDropEffects.None;
		}

		private void txtLibretroCore_DragDrop(object sender, DragEventArgs e)
		{
			var filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
			_config.LibretroCore = filePaths[0];
			RefreshLibretroCore(false);
		}
	}
}
