﻿#region Imports

using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using CrmCodeGenerator.VSPackage.Helpers;
using CrmCodeGenerator.VSPackage.Model;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Query;
using Xceed.Wpf.Toolkit;
using Application = System.Windows.Forms.Application;
using MessageBox = System.Windows.MessageBox;
using MultiSelectComboBoxClass = CrmCodeGenerator.Controls.MultiSelectComboBox;
using WindowStartupLocation = System.Windows.WindowStartupLocation;

#endregion

namespace CrmCodeGenerator.VSPackage.Dialogs
{
	/// <summary>
	///     Interaction logic for Filter.xaml
	/// </summary>
	public partial class PopupSelector : INotifyPropertyChanged
	{
		#region Hide close button stuff

		private const int GWL_STYLE = -16;
		private const int WS_SYSMENU = 0x80000;

		[DllImport("user32.dll", SetLastError = true)]
		private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		#endregion

		#region Properties

		public ObservableCollection<string> Data
		{
			get => data;
			set
			{
				data = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<string> SelectedData
		{
			get => selectedData;
			set
			{
				selectedData = value;
				OnPropertyChanged();
			}
		}

		#endregion

		private ObservableCollection<string> data;
		private ObservableCollection<string> selectedData;

		private readonly Action<IEnumerable<string>> callback;

		#region Property events

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		#region Init

		public PopupSelector(Window parentWindow, IEnumerable<string> data, IEnumerable<string> selectedData,
			Action<IEnumerable<string>> callback, double? x = null, double? y = null)
		{
			InitializeComponent();

			Owner = parentWindow;

			Actions.DataContext = this;
			Data = new ObservableCollection<string>(data);
			SelectedData = new ObservableCollection<string>(selectedData);

			this.callback = callback;

			if (x == null || y == null)
			{
				WindowStartupLocation = WindowStartupLocation.CenterOwner;
			}
			else
			{
				Left = x.Value - 248;
				Top = y.Value - 20;
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			// hide close button
			var hwnd = new WindowInteropHelper(this).Handle;
			SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			this.HideMinimizeAndMaximizeButtons();
		}

		#endregion

		#region UI events

		private void Close_Click(object sender, RoutedEventArgs e)
		{
			callback(SelectedData);
			Dispatcher.InvokeAsync(Close);
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			Dispatcher.InvokeAsync(Close);
		}

		#endregion
	}
}
